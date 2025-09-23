using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Text.Json;
using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;
using FootballAPI.Repository.Interfaces;

namespace FootballAPI.Services
{
    public interface IFirebaseService
    {
        Task<List<Dictionary<string, object>>> GetCollectionDataAsync(string collectionName);
        Task<Dictionary<string, object>?> GetDocumentDataAsync(string collectionName, string documentId);
        Task<bool> TestConnectionAsync();
        Task<List<string>> PopulateTestDataAsync(string collectionName, int count);

        Task<ImportRatingsResultDto> ImportRatingsFromFirebaseAsync(string collectionName = "ratings");
        Task<IEnumerable<object>> PreviewRatingsImportAsync(string collectionName = "ratings");
    }

    public class FirebaseService : IFirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<FirebaseService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRatingHistoryRepository _ratingHistoryRepository;

        public FirebaseService(
            ILogger<FirebaseService> logger,
            IConfiguration configuration,
            IUserRepository userRepository,
            IRatingHistoryRepository ratingHistoryRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _userRepository = userRepository;
            _ratingHistoryRepository = ratingHistoryRepository;

            try
            {
                var projectId = _configuration["Firebase:ProjectId"];
                var serviceAccountKeyPath = _configuration["Firebase:ServiceAccountKeyPath"];

                if (string.IsNullOrEmpty(projectId))
                {
                    throw new InvalidOperationException("Firebase ProjectId is not configured");
                }

                if (string.IsNullOrEmpty(serviceAccountKeyPath) || !File.Exists(serviceAccountKeyPath))
                {
                    throw new InvalidOperationException($"Firebase service account key file not found at: {serviceAccountKeyPath}");
                }

                var credential = GoogleCredential.FromFile(serviceAccountKeyPath);

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = credential,
                        ProjectId = projectId
                    });
                }

                var firestoreDbBuilder = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    Credential = credential
                };
                _firestoreDb = firestoreDbBuilder.Build();

                _logger.LogInformation("Firebase initialized successfully with project: {ProjectId}", projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase");
                throw;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var collections = _firestoreDb.ListRootCollectionsAsync();
                await foreach (var collection in collections)
                {
                    _logger.LogInformation("Found collection: {CollectionId}", collection.Id);
                    return true;
                }

                _logger.LogInformation("No collections found, but connection is working");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firebase connection test failed");
                return false;
            }
        }

        public async Task<List<Dictionary<string, object>>> GetCollectionDataAsync(string collectionName)
        {
            try
            {
                _logger.LogInformation("Retrieving data from collection: {CollectionName}", collectionName);

                var collection = _firestoreDb.Collection(collectionName);
                var snapshot = await collection.GetSnapshotAsync();

                var documents = new List<Dictionary<string, object>>();

                foreach (var document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        var data = new Dictionary<string, object>(document.ToDictionary());
                        data["_documentId"] = document.Id;
                        documents.Add(data);
                    }
                }

                _logger.LogInformation("Retrieved {Count} documents from collection: {CollectionName}",
                    documents.Count, collectionName);

                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve data from collection: {CollectionName}", collectionName);
                throw;
            }
        }

        public async Task<Dictionary<string, object>?> GetDocumentDataAsync(string collectionName, string documentId)
        {
            try
            {
                _logger.LogInformation("Retrieving document {DocumentId} from collection: {CollectionName}",
                    documentId, collectionName);

                var document = await _firestoreDb.Collection(collectionName).Document(documentId).GetSnapshotAsync();

                if (!document.Exists)
                {
                    _logger.LogWarning("Document {DocumentId} not found in collection: {CollectionName}",
                        documentId, collectionName);
                    return null;
                }

                var data = new Dictionary<string, object>(document.ToDictionary());
                data["_documentId"] = document.Id;

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve document {DocumentId} from collection: {CollectionName}",
                    documentId, collectionName);
                throw;
            }
        }

        public async Task<List<string>> PopulateTestDataAsync(string collectionName, int count)
        {
            try
            {
                _logger.LogInformation("Populating {Count} test documents in collection: {CollectionName}", count, collectionName);

                var collection = _firestoreDb.Collection(collectionName);
                var createdDocuments = new List<string>();

                var playerNames = new[] { "John Doe", "Jane Smith", "Mike Johnson", "Sarah Connor", "Alex Rodriguez",
                                        "Maria Garcia", "David Wilson", "Lisa Anderson", "Chris Brown", "Emma Davis",
                                        "Ryan Martinez", "Anna Thompson", "Kevin White", "Sophie Taylor", "Daniel Lee",
                                        "Nicole Miller", "Jason Clark", "Rachel Green", "Tom Harris", "Amy Walker" };

                var positions = new[] { "defender", "midfielder", "forward", "goalkeeper" };
                var random = new Random();

                for (int i = 0; i < count; i++)
                {
                    var date = DateTime.Now.AddDays(-random.Next(0, 365)).ToString("yyyy-MM-dd");
                    var label = $"player_{i + 1}";
                    var documentId = $"{date}_{label}";

                    var playerName = playerNames[random.Next(playerNames.Length)];
                    var position = positions[random.Next(positions.Length)];

                    var documentData = new Dictionary<string, object>
                    {
                        ["players"] = new Dictionary<string, object>
                        {
                            ["id"] = 100 + i,
                            ["keywords"] = position,
                            ["name"] = playerName,
                            ["mostRecentMatches"] = new[]
                            {
                                new Dictionary<string, object>
                                {
                                    ["diff"] = random.Next(-5, 6),
                                    ["date"] = DateTime.Now.AddDays(-random.Next(1, 30)).ToString("yyyy-MM-dd")
                                }
                            },
                            ["stars"] = random.Next(1, 6),
                            ["isArchived"] = random.Next(0, 10) < 2,
                            ["displayName"] = playerName,
                            ["rating"] = random.Next(5, 11),
                            ["affinity"] = random.Next(50, 101)
                        }
                    };

                    await collection.Document(documentId).SetAsync(documentData);
                    createdDocuments.Add(documentId);

                    if (i % 20 == 0)
                    {
                        _logger.LogInformation("Created {Current}/{Total} documents", i + 1, count);
                    }
                }

                _logger.LogInformation("Successfully populated {Count} documents in collection: {CollectionName}",
                    createdDocuments.Count, collectionName);

                return createdDocuments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to populate test data in collection: {CollectionName}", collectionName);
                throw;
            }
        }

        public async Task<ImportRatingsResultDto> ImportRatingsFromFirebaseAsync(string collectionName = "ratings")
        {
            try
            {
                _logger.LogInformation("Starting import from Firebase collection: {CollectionName}", collectionName);

                var firebaseData = await GetCollectionDataAsync(collectionName);
                var result = new ImportRatingsResultDto();

                foreach (var document in firebaseData)
                {
                    try
                    {
                        await ProcessRatingDocument(document, result);
                    }
                    catch (Exception ex)
                    {
                        var documentId = document.GetValueOrDefault("_documentId", "unknown").ToString();
                        var error = $"Error processing document {documentId}: {ex.Message}";
                        _logger.LogError(ex, "Error processing document {DocumentId}", documentId);
                        result.Errors.Add(error);
                    }
                }

                _logger.LogInformation("Import completed. Users: {UsersImported}, RatingHistory: {RatingHistoryImported}, Updated: {UsersUpdated}, Errors: {ErrorCount}",
                    result.UsersImported, result.RatingHistoryEntriesImported, result.ExistingUsersUpdated, result.Errors.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ratings import from collection: {CollectionName}", collectionName);
                throw;
            }
        }

        public async Task<IEnumerable<object>> PreviewRatingsImportAsync(string collectionName = "ratings")
        {
            try
            {
                var firebaseData = await GetCollectionDataAsync(collectionName);
                var previewData = new List<object>();

                foreach (var document in firebaseData)
                {
                    var preview = CreateRatingPreview(document);
                    if (preview != null)
                        previewData.Add(preview);
                }

                return previewData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing ratings import from collection: {CollectionName}", collectionName);
                throw;
            }
        }

        private async Task ProcessRatingDocument(Dictionary<string, object> document, ImportRatingsResultDto result)
        {
            var documentId = document.GetValueOrDefault("_documentId", "").ToString();

            Dictionary<string, object>? playerData = null;

            if (document.ContainsKey("players") && document["players"] is Dictionary<string, object> playersDict)
            {
                playerData = playersDict;
            }
            else if (document.ContainsKey("players") && document["players"] is System.Text.Json.JsonElement playersElement)
            {
                var jsonString = playersElement.GetRawText();
                playerData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
            }
            else
            {
                playerData = document;
            }

            if (playerData == null)
            {
                result.Warnings.Add($"Document {documentId}: No player data found");
                return;
            }

            var displayName = playerData.GetValueOrDefault("displayName", "").ToString();
            var name = playerData.GetValueOrDefault("name", "").ToString();
            var firebaseId = playerData.GetValueOrDefault("id", "").ToString();

            if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(name))
            {
                result.Warnings.Add($"Document {documentId}: Missing displayName or name in player data");
                return;
            }

            var (firstName, lastName) = ParseFullName(name);

            // Extract date from document ID (e.g., "2024-09-24_player_36")
            var documentDate = ExtractDateFromDocumentId(documentId) ?? DateTime.UtcNow;

            var existingUser = await _userRepository.GetByUsernameAsync(displayName);
            User user;

            if (existingUser != null)
            {
                user = existingUser;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                result.ExistingUsersUpdated++;
            }
            else
            {
                // Create new user with date from document ID
                user = new User
                {
                    Username = displayName,
                    FirstName = firstName,
                    LastName = lastName,
                    Role = UserRole.PLAYER,
                    Rating = 0.0f,
                    CreatedAt = documentDate,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.CreateAsync(user);
                result.UsersImported++;
            }

            await ProcessUserRatings(playerData, user, result, documentDate);
        }

        private async Task ProcessUserRatings(Dictionary<string, object> document, User user, ImportRatingsResultDto result, DateTime documentDate)
        {
            float latestRating = 0.0f;
            DateTime latestDate = DateTime.MinValue;

            if (document.ContainsKey("rating"))
            {
                if (float.TryParse(document["rating"].ToString(), out var rating))
                {
                    var createdAt = ParseDateTime(document.GetValueOrDefault("createdAt")) ?? documentDate;

                    var ratingHistory = new RatingHistory
                    {
                        UserId = user.Id,
                        NewRating = rating,
                        ChangeReason = "Import",
                        CreatedAt = createdAt
                    };

                    await _ratingHistoryRepository.CreateAsync(ratingHistory);
                    result.RatingHistoryEntriesImported++;

                    if (createdAt > latestDate)
                    {
                        latestRating = rating;
                        latestDate = createdAt;
                    }
                }
            }

            if (document.ContainsKey("ratings") && document["ratings"] is System.Text.Json.JsonElement ratingsElement)
            {
                if (ratingsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var ratingElement in ratingsElement.EnumerateArray())
                    {
                        if (ratingElement.TryGetProperty("rating", out var ratingProp) &&
                            ratingProp.TryGetSingle(out var rating))
                        {
                            var createdAt = ParseDateTime(ratingElement.GetProperty("createdAt").GetString()) ?? documentDate;
                            var reason = "Import";

                            if (ratingElement.TryGetProperty("reason", out var reasonProp))
                            {
                                reason = reasonProp.GetString() ?? "Import";
                            }

                            var ratingHistory = new RatingHistory
                            {
                                UserId = user.Id,
                                NewRating = rating,
                                ChangeReason = reason,
                                CreatedAt = createdAt
                            };

                            await _ratingHistoryRepository.CreateAsync(ratingHistory);
                            result.RatingHistoryEntriesImported++;

                            if (createdAt > latestDate)
                            {
                                latestRating = rating;
                                latestDate = createdAt;
                            }
                        }
                    }
                }
            }

            if (latestRating > 0)
            {
                user.Rating = latestRating;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }

        private object? CreateRatingPreview(Dictionary<string, object> document)
        {
            var documentId = document.GetValueOrDefault("_documentId", "").ToString();

            Dictionary<string, object>? playerData = null;

            if (document.ContainsKey("players") && document["players"] is Dictionary<string, object> playersDict)
            {
                playerData = playersDict;
            }
            else if (document.ContainsKey("players") && document["players"] is System.Text.Json.JsonElement playersElement)
            {
                var jsonString = playersElement.GetRawText();
                playerData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
            }
            else
            {
                playerData = document;
            }

            if (playerData == null)
                return new { Error = $"Document {documentId}: No player data found" };

            var displayName = playerData.GetValueOrDefault("displayName", "").ToString();
            var name = playerData.GetValueOrDefault("name", "").ToString();
            var firebaseId = playerData.GetValueOrDefault("id", "").ToString();

            if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(name))
                return new { Warning = $"Document {documentId}: Missing displayName or name in player data" };

            var (firstName, lastName) = ParseFullName(name);

            return new
            {
                FirebaseDocumentId = documentId,
                FirebasePlayerId = firebaseId,
                Username = displayName,
                FirstName = firstName,
                LastName = lastName,
                OriginalName = name,
                RatingData = playerData.ContainsKey("rating") ? playerData["rating"] : null,
                Stars = playerData.GetValueOrDefault("stars", ""),
                Keywords = playerData.GetValueOrDefault("keywords", ""),
                Affinity = playerData.GetValueOrDefault("affinity", ""),
                IsArchived = playerData.GetValueOrDefault("isArchived", false),
                HasRatingsArray = playerData.ContainsKey("ratings"),
                HasMostRecentMatches = playerData.ContainsKey("mostRecentMatches"),
                WouldCreateUser = true
            };
        }

        private (string firstName, string lastName) ParseFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return ("Unknown", "Player");

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return ("Unknown", "Player");

            if (parts.Length == 1)
                return (parts[0], "");

            if (parts.Length == 2)
                return (parts[0], parts[1]);

            var firstName = parts[0];
            var lastName = string.Join(" ", parts.Skip(1));

            return (firstName, lastName);
        }

        private DateTime? ParseDateTime(object? dateValue)
        {
            if (dateValue == null)
                return null;

            var dateString = dateValue.ToString();
            if (string.IsNullOrEmpty(dateString))
                return null;

            if (DateTime.TryParse(dateString, out var date))
                return date;

            if (long.TryParse(dateString, out var unixTimestamp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
            }

            return null;
        }

        private DateTime? ExtractDateFromDocumentId(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
                return null;

            var parts = documentId.Split('_');
            if (parts.Length < 1)
                return null;

            var datePart = parts[0];

            if (DateTime.TryParseExact(datePart, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }

            if (DateTime.TryParse(datePart, out var parsedDate))
            {
                return parsedDate;
            }

            return null;
        }
    }
}