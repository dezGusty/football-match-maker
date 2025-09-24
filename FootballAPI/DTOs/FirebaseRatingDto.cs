using System.Text.Json.Serialization;

namespace FootballAPI.DTOs
{
  /// <summary>
  /// DTO for deserializing rating data from Firebase
  /// </summary>
  public class FirebaseRatingDto
  {
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public float Rating { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
  }

  /// <summary>
  /// DTO for Firebase document structure that includes multiple ratings for a player
  /// </summary>
  public class FirebasePlayerRatingsDto
  {
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("ratings")]
    public List<FirebaseRatingHistoryDto> Ratings { get; set; } = new List<FirebaseRatingHistoryDto>();
  }

  /// <summary>
  /// DTO for individual rating entries in history
  /// </summary>
  public class FirebaseRatingHistoryDto
  {
    [JsonPropertyName("rating")]
    public float Rating { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("matchId")]
    public string? MatchId { get; set; }
  }

  /// <summary>
  /// Result DTO for import operation
  /// </summary>
  public class ImportRatingsResultDto
  {
    public int UsersImported { get; set; }
    public int RatingHistoryEntriesImported { get; set; }
    public int ExistingUsersUpdated { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
  }
}