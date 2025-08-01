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
        ImageUrl = createPlayerDto.ImageUrl,
        IsAvailable = false,
        IsEnabled = true,
        CurrentTeamId = null,
        Speed = createPlayerDto.Speed,
        Stamina = createPlayerDto.Stamina,
        Errors = createPlayerDto.Errors
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
        public async Task<PlayerWithImageDto?> GetPlayerWithImageByIdAsync(int id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            return player != null ? MapToPlayerWithImageDto(player) : null;
        }

        public async Task<IEnumerable<PlayerWithImageDto>> GetAllPlayersWithImagesAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            return players.Select(p => new PlayerWithImageDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Rating = p.Rating,
                IsAvailable = p.IsAvailable,
                CurrentTeamId = p.CurrentTeamId,
                IsEnabled = p.IsEnabled,
                ImageUrl = p.ImageUrl
            });
        }
        // Noi metode pentru gestionarea disponibilității jucătorilor
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
            IsAvailable = false,
            CurrentTeamId = null,
            IsEnabled = false,
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
        Speed = player.Speed,
        Stamina = player.Stamina,
        Errors = player.Errors
    };
}

private static PlayerWithImageDto MapToPlayerWithImageDto(Player player)
{
    if (!player.IsEnabled)
    {
        return new PlayerWithImageDto
        {
            Id = player.Id,
            FirstName = $"Player{player.Id}",
            LastName = "",
            Rating = 0.0f,
            IsAvailable = false,
            CurrentTeamId = null,
            IsEnabled = false,
            ImageUrl = null,
            Speed = 1,
            Stamina = 1,
            Errors = 1
        };
    }
    return new PlayerWithImageDto
    {
        Id = player.Id,
        FirstName = player.FirstName,
        LastName = player.LastName,
        Rating = player.Rating,
        IsAvailable = player.IsAvailable,
        CurrentTeamId = player.CurrentTeamId,
        IsEnabled = true,
        ImageUrl = player.ImageUrl,
        Speed = player.Speed,
        Stamina = player.Stamina,
        Errors = player.Errors
    };
}
}

}