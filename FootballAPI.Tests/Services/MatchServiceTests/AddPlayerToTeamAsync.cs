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
    public class AddPlayerToTeamAsyncTests : IDisposable
    {
        private readonly Mock<IMatchRepository> _mockMatchRepository;
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly Mock<IMatchTeamsService> _mockMatchTeamsService;
        private readonly Mock<ITeamPlayersService> _mockTeamPlayersService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly FootballDbContext _context;
        private readonly MatchService _matchService;

        public AddPlayerToTeamAsyncTests()
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
        public async Task AddPlayerToTeamAsync_WhenMatchNotFound_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync((Models.Match)null);

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenUserNotAuthorized_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 10;
            int teamId = 1;
            int organizerId = 5;

            var match = new Models.Match { Id = matchId, OrganiserId = organizerId };
            var organiserPlayers = new List<User>
                  {
                      new User { Id = 1 },
                      new User { Id = 2 },
                      new User { Id = 3 }
                  };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockUserService.Setup(x => x.GetPlayersByOrganiserAsync(organizerId))
                .ReturnsAsync(organiserPlayers);

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenOrganizerAddsHimself_ReturnsTrue()
        {
            // Arrange
            int matchId = 1;
            int userId = 5;
            int teamId = 1;
            int organizerId = 5;

            var match = new Models.Match { Id = matchId, OrganiserId = organizerId };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var existingPlayers = new List<TeamPlayersDto>();

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockUserService.Setup(x => x.GetPlayersByOrganiserAsync(organizerId))
                .ReturnsAsync(new List<User>());

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync((TeamPlayersDto?)null);

            _mockTeamPlayersService.Setup(x => x.CreateTeamPlayerAsync(It.IsAny<CreateTeamPlayersDto>()))
                .ReturnsAsync(new TeamPlayersDto { Id = 1 });

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.True(result);

        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenMatchTeamNotFound_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;
            int organizerId = 5;

            var match = new Models.Match { Id = matchId, OrganiserId = organizerId };
            var organiserPlayers = new List<User> { new User { Id = userId } };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockUserService.Setup(x => x.GetPlayersByOrganiserAsync(organizerId))
                .ReturnsAsync(organiserPlayers);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync((MatchTeamsDto?)null);

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenTeamIsFull_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;
            int organizerId = 5;

            var match = new Models.Match { Id = matchId, OrganiserId = organizerId };
            var organiserPlayers = new List<User> { new User { Id = userId } };
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

            _mockUserService.Setup(x => x.GetPlayersByOrganiserAsync(organizerId))
                .ReturnsAsync(organiserPlayers);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenPlayerAlreadyInTeam_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;
            int organizerId = 5;

            var match = new Models.Match { Id = matchId, OrganiserId = organizerId };
            var organiserPlayers = new List<User> { new User { Id = userId } };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var existingPlayers = new List<TeamPlayersDto>
                     {
                         new TeamPlayersDto { UserId = 2 }
                     };
            var existingPlayer = new TeamPlayersDto { UserId = userId, MatchTeamId = matchTeam.Id };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockUserService.Setup(x => x.GetPlayersByOrganiserAsync(organizerId))
                .ReturnsAsync(organiserPlayers);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync(existingPlayer);

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenValidRequest_ReturnsTrue()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;
            int organizerId = 5;

            var match = new Models.Match { Id = matchId, OrganiserId = organizerId };
            var organiserPlayers = new List<User> { new User { Id = userId } };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var existingPlayers = new List<TeamPlayersDto>
                     {
                         new TeamPlayersDto { UserId = 2 },
                         new TeamPlayersDto { UserId = 3 }
                     };
            var createdPlayer = new TeamPlayersDto { Id = 1, UserId = userId };

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockUserService.Setup(x => x.GetPlayersByOrganiserAsync(organizerId))
                .ReturnsAsync(organiserPlayers);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync((TeamPlayersDto?)null);

            _mockTeamPlayersService.Setup(x => x.CreateTeamPlayerAsync(It.IsAny<CreateTeamPlayersDto>()))
                .ReturnsAsync(createdPlayer);

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.True(result);

        }

        [Fact]
        public async Task AddPlayerToTeamAsync_WhenCreateTeamPlayerFails_ReturnsFalse()
        {
            // Arrange
            int matchId = 1;
            int userId = 1;
            int teamId = 1;
            int organizerId = 5;

            var match = new Models.Match { Id = matchId, OrganiserId = organizerId };
            var organiserPlayers = new List<User> { new User { Id = userId } };
            var matchTeam = new MatchTeamsDto { Id = 10, MatchId = matchId, TeamId = teamId };
            var existingPlayers = new List<TeamPlayersDto>();

            _mockMatchRepository.Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockUserService.Setup(x => x.GetPlayersByOrganiserAsync(organizerId))
                .ReturnsAsync(organiserPlayers);

            _mockMatchTeamsService.Setup(x => x.GetMatchTeamByMatchIdAndTeamIdAsync(matchId, teamId))
                .ReturnsAsync(matchTeam);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayersByMatchTeamIdAsync(matchTeam.Id))
                .ReturnsAsync(existingPlayers);

            _mockTeamPlayersService.Setup(x => x.GetTeamPlayerByMatchTeamIdAndUserIdAsync(matchTeam.Id, userId))
                .ReturnsAsync((TeamPlayersDto?)null);

            _mockTeamPlayersService.Setup(x => x.CreateTeamPlayerAsync(It.IsAny<CreateTeamPlayersDto>()))
                .ReturnsAsync((TeamPlayersDto?)null);

            // Act
            var result = await _matchService.AddPlayerToTeamAsync(matchId, userId, teamId);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}