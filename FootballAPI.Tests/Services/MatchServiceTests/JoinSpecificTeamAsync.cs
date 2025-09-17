using FootballAPI.Service;
using FootballAPI.Repository;
using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Data;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace FootballAPI.Tests.Services.MatchServiceTests
{
    public class JoinSpecificTeamAsyncTests : IDisposable
    {
        private readonly Mock<IMatchRepository> _mockMatchRepository;
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly Mock<IMatchTeamsService> _mockMatchTeamsService;
        private readonly Mock<ITeamPlayersService> _mockTeamPlayersService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly FootballDbContext _context;
        private readonly MatchService _matchService;

        public JoinSpecificTeamAsyncTests()
        {
            _mockMatchRepository = new Mock<IMatchRepository>();
            _mockTeamService = new Mock<ITeamService>();
            _mockMatchTeamsService = new Mock<IMatchTeamsService>();
            _mockTeamPlayersService = new Mock<ITeamPlayersService>();
            _mockUserService = new Mock<IUserService>();

            var options = new DbContextOptionsBuilder<FootballDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new FootballDbContext(options);

            _matchService = new MatchService(
                _mockMatchRepository.Object,
                _mockTeamService.Object,
                _mockMatchTeamsService.Object,
                _mockTeamPlayersService.Object,
                _mockUserService.Object,
                _context
            );
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenMatchNotFound_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync((Models.Match)null);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenMatchIsNotPublic_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            var match = new Models.Match { Id = matchId, IsPublic = false };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenMatchTeamNotFound_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            var match = new Models.Match { Id = matchId, IsPublic = true };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync((MatchTeamsDto)null);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenTeamIsFull_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            var match = new Models.Match { Id = matchId, IsPublic = true };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };

            var existingPlayers = new List<TeamPlayersDto>
            {
                new TeamPlayersDto { UserId = 2 },
                new TeamPlayersDto { UserId = 3 },
                new TeamPlayersDto { UserId = 4 },
                new TeamPlayersDto { UserId = 5 },
                new TeamPlayersDto { UserId = 6 },
                new TeamPlayersDto { UserId = 7 }
            };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenPlayerAlreadyInSameTeam_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            var match = new Models.Match { Id = matchId, IsPublic = true };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var existingPlayers = new List<TeamPlayersDto>
            {
                new TeamPlayersDto { UserId = 2 }
            };
            var existingPlayer = new TeamPlayersDto { UserId = userId, MatchTeamId = matchTeam.Id };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync(existingPlayer);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenPlayerAlreadyInDifferentTeam_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            var match = new Models.Match { Id = matchId, IsPublic = true };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var otherMatchTeam = new MatchTeamsDto { Id = 11, MatchId = matchId, TeamId = 2 };

            var existingPlayers = new List<TeamPlayersDto>
            {
                new TeamPlayersDto { UserId = 2 }
            };

            var allMatchTeams = new List<MatchTeamsDto> { matchTeam, otherMatchTeam };
            var playersInOtherTeam = new List<TeamPlayersDto>
            {
                new TeamPlayersDto { UserId = userId, MatchTeamId = otherMatchTeam.Id }
            };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync((TeamPlayersDto)null);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamsByMatchIdAsync(matchId))
                .ReturnsAsync(allMatchTeams);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(otherMatchTeam.Id))
                .ReturnsAsync(playersInOtherTeam);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenCreateTeamPlayerFails_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            var match = new Models.Match { Id = matchId, IsPublic = true };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var existingPlayers = new List<TeamPlayersDto>();
            var allMatchTeams = new List<MatchTeamsDto> { matchTeam };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync((TeamPlayersDto)null);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamsByMatchIdAsync(matchId))
                .ReturnsAsync(allMatchTeams);

            _mockTeamPlayersService.Setup(x => x.CreateTeamPlayerAsync(It.IsAny<CreateTeamPlayersDto>()))
                .ReturnsAsync((TeamPlayersDto)null);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinSpecificTeamAsync_WhenValidRequest_ReturnsTrue()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            var match = new Models.Match { Id = matchId, IsPublic = true };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var existingPlayers = new List<TeamPlayersDto>
            {
                new TeamPlayersDto { UserId = 2 },
                new TeamPlayersDto { UserId = 3 }
            };
            var allMatchTeams = new List<MatchTeamsDto> { matchTeam };
            var createdPlayer = new TeamPlayersDto { Id = 1, UserId = userId, Status = PlayerStatus.Joined };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync((TeamPlayersDto)null);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamsByMatchIdAsync(matchId))
                .ReturnsAsync(allMatchTeams);

            _mockTeamPlayersService.Setup(x => x.CreateTeamPlayerAsync(It.Is<CreateTeamPlayersDto>(dto =>
                dto.MatchTeamId == matchTeam.Id &&
                dto.UserId == userId &&
                dto.Status == PlayerStatus.Joined)))
                .ReturnsAsync(createdPlayer);

            // Act
            var result = await _matchService.JoinSpecificTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.True(result);
            _mockTeamPlayersService.Verify(x => x.CreateTeamPlayerAsync(It.Is<CreateTeamPlayersDto>(dto =>
                dto.MatchTeamId == matchTeam.Id &&
                dto.UserId == userId &&
                dto.Status == PlayerStatus.Joined)), Times.Once);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}