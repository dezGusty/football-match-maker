using System.Diagnostics.Tracing;
using FootballAPI.Data;
using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
using FootballAPI.Service.Interfaces;
namespace FootballAPI.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordGeneratorService _passwordGeneratorService;
        private readonly IFileService _fileService;

        public PlayerService(
            IPlayerRepository playerRepository,
            IUserRepository userRepository,
            IPasswordGeneratorService passwordGeneratorService,
            IFileService fileService)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository;
            _passwordGeneratorService = passwordGeneratorService;
            _fileService = fileService;
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

        public async Task<PlayerDto?> GetPlayerByUserIdAsync(int userId)
        {
            var player = await _playerRepository.GetByUserIdAsync(userId);
            return player != null ? MapToDto(player) : null;
        }

        public async Task<IEnumerable<PlayerDto>> GetAvailablePlayersAsync()
        {
            var players = await _playerRepository.GetAvailablePlayersAsync();
            return players.Select(MapToDto);
        }

        public async Task<IEnumerable<PlayerDto>> GetAvailablePlayersByOrganiserAsync(int organiserId)
        {
            var organiserPlayers = await _userRepository.GetPlayersByOrganiserAsync(organiserId);
            var availablePlayers = organiserPlayers.Where(p => p.IsAvailable && p.IsEnabled);
            return availablePlayers.Select(MapToDto);
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
            existingPlayer.IsEnabled = updatePlayerDto.IsEnabled;
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


        private PlayerDto MapToDto(Player player)
        {
            if (!player.IsEnabled)
            {
                return new PlayerDto
                {
                    Id = player.Id,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    Rating = 0.0f,
                    UserEmail = player.User?.Email ?? "",
                    Username = player.User?.Username ?? "",
                    IsAvailable = false,
                    IsEnabled = false,
                    Speed = 1,
                    Stamina = 1,
                    Errors = 1,
                    ProfileImageUrl = "http://localhost:5145/assets/default-avatar.png"
                };
            }
            return new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Rating = player.Rating,
                IsAvailable = player.IsAvailable,
                IsEnabled = true,
                UserEmail = player.User?.Email ?? "",
                Username = player.User?.Username ?? "",
                Speed = player.Speed,
                Stamina = player.Stamina,
                Errors = player.Errors,
                ProfileImageUrl = _fileService.GetProfileImageUrl(player.ProfileImagePath)
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
            var organiser = await _userRepository.GetByIdAsync(organiserId);
            if (organiser == null || organiser.Role != UserRole.ORGANISER)
                throw new InvalidOperationException("OrganiserId does not correspond to a valid organiser user.");

            var relation = new PlayerOrganiser
            {
                PlayerId = playerId,
                OrganiserId = organiserId
            };

            await _playerRepository.AddPlayerOrganiserRelationAsync(relation);
        }

        public async Task<string> UpdatePlayerProfileImageAsync(int playerId, IFormFile imageFile)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null || !player.IsEnabled)
                throw new ArgumentException("Player not found or not enabled");

            if (player.User == null)
            {
                var user = await _userRepository.GetByIdAsync(player.UserId);
                player.User = user;
            }

            if (!string.IsNullOrEmpty(player.ProfileImagePath))
            {
                await _fileService.DeleteProfileImageAsync(player.ProfileImagePath);
            }

            var imagePath = await _fileService.SaveProfileImageAsync(imageFile, player.User.Email);

            player.ProfileImagePath = imagePath;
            await _playerRepository.UpdateAsync(player);

            return _fileService.GetProfileImageUrl(imagePath);
        }

        public async Task<bool> DeletePlayerProfileImageAsync(int playerId)
        {
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null || !player.IsEnabled)
                return false;

            if (!string.IsNullOrEmpty(player.ProfileImagePath))
            {
                await _fileService.DeleteProfileImageAsync(player.ProfileImagePath);
                player.ProfileImagePath = null;
                await _playerRepository.UpdateAsync(player);
            }

            return true;
        }
    }

}