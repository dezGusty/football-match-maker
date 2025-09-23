using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Text.Json;

namespace FootballAPI.Services
{
    public interface IFirebaseService
    {
        Task<List<Dictionary<string, object>>> GetCollectionDataAsync(string collectionName);
        Task<Dictionary<string, object>?> GetDocumentDataAsync(string collectionName, string documentId);
        Task<bool> TestConnectionAsync();
        Task<List<string>> PopulateTestDataAsync(string collectionName, int count);
    }

    public class FirebaseService : IFirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<FirebaseService> _logger;
        private readonly IConfiguration _configuration;

        public FirebaseService(ILogger<FirebaseService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

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
    }
}