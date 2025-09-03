using FootballAPI.Models.Enums;
using FootballAPI.DTOs;
using FootballAPI.Repository;

namespace FootballAPI.Service
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetAllMatchesAsync();
        Task<MatchDto> GetMatchByIdAsync(int id);
        Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto, int organiserId);
        Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto);
        Task<bool> DeleteMatchAsync(int id);
        Task<IEnumerable<MatchDto>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MatchDto>> GetPublicMatchesAsync();
        Task<IEnumerable<MatchDto>> GetMatchesByStatusAsync(Status status);

        Task<IEnumerable<MatchDto>> GetPastMatchesByParticipantAsync(int userId);
        Task<IEnumerable<MatchDto>> GetMyPublicMatchesAsync(int id);

        Task<IEnumerable<MatchDto>> GetFutureMatchesAsync();
        Task<IEnumerable<MatchDto>> GetPastMatchesAsync(int id);
        Task<IEnumerable<MatchDto>> GetMatchesByOrganiserAsync(int organiserId);
        Task<IEnumerable<MatchDto>> GetMatchesByLocationAsync(string location);
        Task<IEnumerable<MatchDto>> GetMatchesByCostRangeAsync(decimal? minCost, decimal? maxCost);
        Task<MatchDto> MakeMatchPublicAsync(int matchId);
        Task<MatchDto> MakeMatchPrivateAsync(int matchId);

        Task<bool> AddPlayerToTeamAsync(int matchId, int userId, int teamId);
        Task<bool> JoinSpecificTeamAsync(int matchId, int userId, int teamId);
        Task<bool> JoinPublicMatchAsync(int matchId, int userId);
        Task<bool> MovePlayerBetweenTeamsAsync(int matchId, int userId, int newTeamId);
        Task<MatchDto> PublishMatchAsync(int matchId);
        Task<MatchDetailsDto> GetMatchDetailsAsync(int matchId);
        Task<bool> LeaveMatchAsync(int matchId, int userId);
        Task<IEnumerable<MatchDto>> GetPlayerMatchesAsync(int userId);
        Task<IEnumerable<MatchDto>> GetAvailableMatchesForPlayerAsync(int userId);
        Task<bool> RemovePlayerFromMatchAsync(int matchId, int userId);

        Task<int> GetEffectiveOrganizerId(int userId);

        Task<MatchDto> CloseMatchAsync(int matchId);
        Task<MatchDto> CancelMatchAsync(int matchId);
        Task<MatchDto> FinalizeMatchAsync(int matchId, FinalizeMatchDto finalizeMatchDto);
        Task<List<RatingPreviewDto>> CalculateRatingPreviewAsync(int matchId, CalculateRatingPreviewDto dto);
    }
}