using System.Diagnostics.Tracing;
using FootballAPI.Data;
using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
namespace FootballAPI.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordGeneratorService _passwordGeneratorService;

        public PlayerService(
            IPlayerRepository playerRepository,
            IUserRepository userRepository,
            IPasswordGeneratorService passwordGeneratorService)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository;
            _passwordGeneratorService = passwordGeneratorService;
        }

        public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            return players.Select(MapToDto);
        }

        public async Task<PlayerDto?> GetPlayerByIdAsync(int id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            return player != null ? MapToDto(player) : null;
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersByTeamIdAsync(int teamId)
        {
            var players = await _playerRepository.GetByTeamIdAsync(teamId);
            return players.Select(MapToDto);
        }

        public async Task<IEnumerable<PlayerDto>> GetAvailablePlayersAsync()
        {
            var players = await _playerRepository.GetAvailablePlayersAsync();
            return players.Select(MapToDto);
        }


        public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto)
        {
            var emailService = new EmailService();
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser == null)
            {
                var password = _passwordGeneratorService.Generate();
                var user = new User
                {
                    Email = dto.Email,
                    Username = dto.FirstName + dto.LastName,
                    Password = password,
                    Role = UserRole.PLAYER,
                    ImageUrl = dto.ImageUrl
                };
                await _userRepository.CreateAsync(user);
                await emailService.SendNewPasswordPlayerEmailAsync(
                    user.Email,
                    user.Username,
                    password
                );
            }

            var existingPlayer = (await _playerRepository.GetAllAsync())
                .FirstOrDefault(p => p.Email == dto.Email);
            if (existingPlayer != null)
                throw new InvalidOperationException("Player with this email already exists.");

            var player = new Player
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Rating = dto.Rating,
                Email = dto.Email,
                IsAvailable = false,
                IsEnabled = true,
                ImageUrl = dto.ImageUrl,
                Speed = dto.Speed,
                Stamina = dto.Stamina,
                Errors = dto.Errors
            };
            var createdPlayer = await _playerRepository.CreateAsync(player);

            return MapToDto(createdPlayer);
        }


        public async Task<PlayerDto?> UpdatePlayerAsync(int id, UpdatePlayerDto updatePlayerDto)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
                return null;

            existingPlayer.FirstName = updatePlayerDto.FirstName;
            existingPlayer.LastName = updatePlayerDto.LastName;
            existingPlayer.Rating = updatePlayerDto.Rating;
            existingPlayer.IsAvailable = updatePlayerDto.IsAvailable;
            existingPlayer.CurrentTeamId = updatePlayerDto.CurrentTeamId;
            existingPlayer.IsEnabled = updatePlayerDto.IsEnabled;
            existingPlayer.ImageUrl = updatePlayerDto.ImageUrl;
            existingPlayer.Speed = updatePlayerDto.Speed;
            existingPlayer.Stamina = updatePlayerDto.Stamina;
            existingPlayer.Errors = updatePlayerDto.Errors;

            var updatedPlayer = await _playerRepository.UpdateAsync(existingPlayer);
            return MapToDto(updatedPlayer);
        }
        public async Task<bool> DeletePlayerAsync(int id)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
                return false;

            existingPlayer.IsEnabled = false;
            await _playerRepository.UpdateAsync(existingPlayer);
            return true;
        }

        public async Task<bool> EnablePlayerAsync(int id)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
                return false;

            existingPlayer.IsEnabled = true;
            await _playerRepository.UpdateAsync(existingPlayer);
            return true;
        }

        public async Task<bool> HardDeletePlayerAsync(int id)
        {
            return await _playerRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PlayerDto>> SearchPlayersByNameAsync(string searchTerm)
        {
            var players = await _playerRepository.SearchByNameAsync(searchTerm);
            return players.Select(MapToDto);
        }

        public async Task<bool> PlayerExistsAsync(int id)
        {
            return await _playerRepository.ExistsAsync(id);
        }

        // Disponibilitate
        public async Task<bool> SetPlayerAvailableAsync(int playerId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null || !player.IsEnabled)
                return false;

            player.IsAvailable = true;
            await _playerRepository.UpdateAsync(player);
            return true;
        }

        public async Task<bool> SetPlayerUnavailableAsync(int playerId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null)
                return false;

            player.IsAvailable = false;
            await _playerRepository.UpdateAsync(player);
            return true;
        }

        public async Task<bool> SetMultiplePlayersAvailableAsync(int[] playerIds)
        {
            try
            {
                foreach (var playerId in playerIds)
                {
                    var player = await _playerRepository.GetByIdAsync(playerId);
                    if (player != null && player.IsEnabled)
                    {
                        player.IsAvailable = true;
                        await _playerRepository.UpdateAsync(player);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetMultiplePlayersUnavailableAsync(int[] playerIds)
        {
            try
            {
                foreach (var playerId in playerIds)
                {
                    var player = await _playerRepository.GetByIdAsync(playerId);
                    if (player != null)
                    {
                        player.IsAvailable = false;
                        await _playerRepository.UpdateAsync(player);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ClearAllAvailablePlayersAsync()
        {
            try
            {
                var availablePlayers = await _playerRepository.GetAvailablePlayersAsync();
                foreach (var player in availablePlayers)
                {
                    player.IsAvailable = false;
                    await _playerRepository.UpdateAsync(player);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        private static PlayerDto MapToDto(Player player)
        {
            if (!player.IsEnabled)
            {
                return new PlayerDto
                {
                    Id = player.Id,
                    FirstName = $"Player{player.Id}",
                    LastName = "",
                    Rating = 0.0f,
                    Email = "",
                    IsAvailable = false,
                    CurrentTeamId = null,
                    IsEnabled = false,
                    ImageUrl = null,
                    Speed = 1,
                    Stamina = 1,
                    Errors = 1
                };



            }
            return new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Rating = player.Rating,
                IsAvailable = player.IsAvailable,
                CurrentTeamId = player.CurrentTeamId,
                IsEnabled = true,
                Email = player.Email,
                ImageUrl = player.ImageUrl,
                Speed = player.Speed,
                Stamina = player.Stamina,
                Errors = player.Errors
            };
        }

        public async Task<bool> UpdatePlayerRatingAsync(int playerId, float ratingChange)
        {
            try
            {
                var player = await _playerRepository.GetByIdAsync(playerId);
                if (player == null || !player.IsEnabled)
                    return false;

                var newRating = Math.Max(0.0f, Math.Min(10000.0f, player.Rating + ratingChange));
                player.Rating = newRating;

                await _playerRepository.UpdateAsync(player);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateMultiplePlayerRatingsAsync(List<PlayerRatingUpdateDto> playerRatingUpdates)
        {
            try
            {
                foreach (var update in playerRatingUpdates)
                {
                    var player = await _playerRepository.GetByIdAsync(update.PlayerId);
                    if (player != null && player.IsEnabled)
                    {
                        var newRating = Math.Max(0.0f, Math.Min(10000.0f, player.Rating + update.RatingChange));
                        player.Rating = newRating;
                        await _playerRepository.UpdateAsync(player);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task AddPlayerOrganiserRelationAsync(int playerId, int organiserId)
    {
        var relation = new PlayerOrganiser
        {
            PlayerId = playerId,
            OrganiserId = organiserId
        };

        await _playerRepository.AddPlayerOrganiserRelationAsync(relation);
    }
    }

}