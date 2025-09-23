using Microsoft.AspNetCore.Mvc;
using FootballAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using FootballAPI.DTOs;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(
            IFirebaseService firebaseService,
            ILogger<ImportController> logger)
        {
            _firebaseService = firebaseService;
            _logger = logger;
        }

        /// <summary>
        /// Test Firebase connection
        /// </summary>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var isConnected = await _firebaseService.TestConnectionAsync();

                if (isConnected)
                {
                    return Ok(new { message = "Firebase connection successful", connected = true });
                }
                else
                {
                    return BadRequest(new { message = "Firebase connection failed", connected = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Firebase connection");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all data from a specific Firebase collection
        /// </summary>
        [HttpGet("collection/{collectionName}")]
        public async Task<IActionResult> GetCollectionData(string collectionName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(collectionName))
                {
                    return BadRequest(new { message = "Collection name is required" });
                }

                var data = await _firebaseService.GetCollectionDataAsync(collectionName);

                return Ok(new
                {
                    message = $"Successfully retrieved data from collection: {collectionName}",
                    collectionName = collectionName,
                    documentCount = data.Count,
                    data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from collection: {CollectionName}", collectionName);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific document from a Firebase collection
        /// </summary>
        [HttpGet("collection/{collectionName}/document/{documentId}")]
        public async Task<IActionResult> GetDocumentData(string collectionName, string documentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(collectionName))
                {
                    return BadRequest(new { message = "Collection name is required" });
                }

                if (string.IsNullOrWhiteSpace(documentId))
                {
                    return BadRequest(new { message = "Document ID is required" });
                }

                var data = await _firebaseService.GetDocumentDataAsync(collectionName, documentId);

                if (data == null)
                {
                    return NotFound(new { message = $"Document {documentId} not found in collection {collectionName}" });
                }

                return Ok(new
                {
                    message = $"Successfully retrieved document: {documentId}",
                    collectionName = collectionName,
                    documentId = documentId,
                    data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document {DocumentId} from collection: {CollectionName}",
                    documentId, collectionName);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import players data from Firebase and preview what would be imported
        /// </summary>
        [HttpGet("preview/players")]
        public async Task<IActionResult> PreviewPlayersImport([FromQuery] string collectionName = "players")
        {
            try
            {
                var data = await _firebaseService.GetCollectionDataAsync(collectionName);

                // Transform Firebase data to show what would be imported
                var previewData = data.Select(doc => new
                {
                    FirebaseDocumentId = doc.GetValueOrDefault("_documentId"),
                    PotentialPlayerData = doc.Where(kvp => kvp.Key != "_documentId").ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    WouldCreateNewPlayer = true // Since this is just preview
                }).ToList();

                return Ok(new
                {
                    message = $"Preview of players import from collection: {collectionName}",
                    collectionName = collectionName,
                    totalDocuments = data.Count,
                    previewData = previewData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing players import from collection: {CollectionName}", collectionName);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import matches data from Firebase and preview what would be imported
        /// </summary>
        [HttpGet("preview/matches")]
        public async Task<IActionResult> PreviewMatchesImport([FromQuery] string collectionName = "matches")
        {
            try
            {
                var data = await _firebaseService.GetCollectionDataAsync(collectionName);

                // Transform Firebase data to show what would be imported
                var previewData = data.Select(doc => new
                {
                    FirebaseDocumentId = doc.GetValueOrDefault("_documentId"),
                    PotentialMatchData = doc.Where(kvp => kvp.Key != "_documentId").ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    WouldCreateNewMatch = true // Since this is just preview
                }).ToList();

                return Ok(new
                {
                    message = $"Preview of matches import from collection: {collectionName}",
                    collectionName = collectionName,
                    totalDocuments = data.Count,
                    previewData = previewData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing matches import from collection: {CollectionName}", collectionName);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get raw data from any Firebase collection for inspection
        /// </summary>
        [HttpGet("raw/{collectionName}")]
        public async Task<IActionResult> GetRawCollectionData(string collectionName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(collectionName))
                {
                    return BadRequest(new { message = "Collection name is required" });
                }

                var data = await _firebaseService.GetCollectionDataAsync(collectionName);

                // Return data as raw JSON for inspection
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving raw data from collection: {CollectionName}", collectionName);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Populate test data in Firebase
        /// </summary>
        [HttpGet("populate-test-data")]
        [HttpPost("populate-test-data")]
        public async Task<IActionResult> PopulateTestData([FromQuery] string collectionName = "ratings", [FromQuery] int count = 100)
        {
            try
            {
                var results = await _firebaseService.PopulateTestDataAsync(collectionName, count);

                return Ok(new
                {
                    message = $"Successfully populated {results.Count} documents in collection: {collectionName}",
                    collectionName = collectionName,
                    documentsCreated = results.Count,
                    documentIds = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating test data in collection: {CollectionName}", collectionName);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import users and rating history from Firebase 'ratings' collection
        /// </summary>
        [HttpPost("ratings")]
        public async Task<IActionResult> ImportRatingsFromFirebase([FromQuery] string collectionName = "ratings")
        {
            try
            {
                var result = await _firebaseService.ImportRatingsFromFirebaseAsync(collectionName);

                return Ok(new
                {
                    message = "Import completed",
                    result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ratings import from collection: {CollectionName}", collectionName);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

    }
}


