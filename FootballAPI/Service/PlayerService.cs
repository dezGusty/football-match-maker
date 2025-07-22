using FootballAPI.DTOs;
using FootballAPI.Models;
using FootballAPI.Repository;
namespace FootballAPI.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
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

        public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto createPlayerDto)
        {
            var player = new Player
            {
                FirstName = createPlayerDto.FirstName,
                LastName = createPlayerDto.LastName,
                Rating = createPlayerDto.Rating,
                IsAvailable = false,
                IsEnabled = true,
                CurrentTeamId = null
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
            var updatedPlayer = await _playerRepository.UpdateAsync(existingPlayer);
            return MapToDto(updatedPlayer);
        }

        public async Task<bool> DeletePlayerAsync(int id)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
                return false;

            existingPlayer.IsEnabled = false; // Soft delete
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
                    IsAvailable = false,
                    CurrentTeamId = null,
                    IsEnabled = false
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
                IsEnabled = true

            };
        }
    }

}