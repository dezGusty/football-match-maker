using System.Diagnostics.Tracing;
using FootballAPI.Data;
using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Models.Enums;
using FootballAPI.Repository;
using FootballAPI.Service.Interfaces;
namespace FootballAPI.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;

        public PlayerService(
            IPlayerRepository playerRepository,
            IUserRepository userRepository,
            IFileService fileService)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository; _fileService = fileService;
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
        public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto)
        {
            var player = new Player
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Rating = dto.Rating,
                Speed = dto.Speed,
                Stamina = dto.Stamina,
                Errors = dto.Errors,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdPlayer = await _playerRepository.CreateAsync(player);
            return MapToDto(createdPlayer);
        }

        public async Task<PlayerDto?> GetPlayerByUserIdAsync(int userId)
        {
            var player = await _playerRepository.GetByUserIdAsync(userId);
            return player != null ? MapToDto(player) : null;
        }

        public async Task<int?> GetPlayerIdByUserIdAsync(int userId)
        {
            var player = await _playerRepository.GetByUserIdAsync(userId);
            return player?.Id;
        }

        public async Task<IEnumerable<PlayerDto>> GetAvailablePlayersAsync()
        {
            var players = await _playerRepository.GetAvailablePlayersAsync();
            return players.Select(MapToDto);
        }

        public async Task<IEnumerable<PlayerDto>> GetAvailablePlayersByOrganiserAsync(int organiserId)
        {
            var organiserUsers = await _userRepository.GetPlayersByOrganiserAsync(organiserId);
            var playerDtos = new List<PlayerDto>();

            foreach (var user in organiserUsers)
            {
                var player = await _playerRepository.GetByUserIdAsync(user.Id);
                if (player != null && player.DeletedAt == null)
                {
                    playerDtos.Add(MapToDto(player));
                }
            }

            return playerDtos;
        }




        public async Task<PlayerDto?> UpdatePlayerAsync(int id, UpdatePlayerDto updatePlayerDto)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
                return null;

            existingPlayer.FirstName = updatePlayerDto.FirstName;
            existingPlayer.LastName = updatePlayerDto.LastName;
            existingPlayer.Rating = updatePlayerDto.Rating;
            existingPlayer.Speed = updatePlayerDto.Speed;
            existingPlayer.Stamina = updatePlayerDto.Stamina;
            existingPlayer.Errors = updatePlayerDto.Errors;

            var updatedPlayer = await _playerRepository.UpdateAsync(existingPlayer);
            return MapToDto(updatedPlayer);
        }
        public async Task<bool> DeletePlayerAsync(int id)
        {
            return await _playerRepository.DeleteAsync(id);
        }

        public async Task<bool> RestorePlayerAsync(int id)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null || existingPlayer.DeletedAt == null)
                return false;

            existingPlayer.DeletedAt = null;
            existingPlayer.UpdatedAt = DateTime.UtcNow;
            await _playerRepository.UpdateAsync(existingPlayer);
            return true;
        }

        public async Task<bool> HardDeletePlayerAsync(int id)
        {
            return await _playerRepository.HardDeleteAsync(id);
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



        private PlayerDto MapToDto(Player player)
        {
            if (player.DeletedAt != null)
            {
                return new PlayerDto
                {
                    Id = player.Id,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    Rating = 0.0f,
                    UserEmail = player.User?.Email ?? "",
                    Username = player.User?.Username ?? "",
                    IsDeleted = true,
                    Speed = 1,
                    Stamina = 1,
                    Errors = 1,
                    ProfileImageUrl = "http://localhost:5145/assets/default-avatar.png",
                    CreatedAt = player.CreatedAt,
                    UpdatedAt = player.UpdatedAt,
                    DeletedAt = player.DeletedAt
                };
            }
            return new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Rating = player.Rating,
                IsDeleted = false,
                CreatedAt = player.CreatedAt,
                UpdatedAt = player.UpdatedAt,
                DeletedAt = player.DeletedAt,
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
                if (player == null || player.DeletedAt != null)
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
                    if (player != null && player.DeletedAt == null)
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

            var player = await _userRepository.GetByIdAsync(playerId);
            if (player == null || player.Role != UserRole.PLAYER)
                throw new InvalidOperationException("PlayerId does not correspond to a valid player user.");

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
            if (player == null || player.DeletedAt != null)
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
            if (player == null || player.DeletedAt != null)
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