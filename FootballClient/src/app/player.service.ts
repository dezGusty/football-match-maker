import { Injectable } from '@angular/core';
import { Player } from './player.interface';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  private readonly url: string = 'http://localhost:5145/api/players';
  constructor() { }
  async getPlayers(): Promise<Player[]> {
    const response = await fetch(`${this.url}/with-images`);
    if (!response.ok) {
      throw new Error('Failed to fetch players');
    }
    const players = await response.json();
    return players;
  }
  async addPlayer(player: { firstName: string; lastName: string; rating: number; imageUrl?: string }): Promise<Player> {
    const response = await fetch(this.url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(player)
    });

    if (!response.ok) {
      throw new Error('Failed to add player');
    }

    return await response.json();
  }

  async updatePlayer(playerId: number, updateData: Partial<Player>): Promise<Player> {
    const response = await fetch(`${this.url}/${playerId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updateData)
    });

    if (!response.ok) {
      throw new Error('Failed to update player');
    }

    return await response.json();
  }

  async editPlayer(player: Player): Promise<Player> {
    const response = await fetch(`${this.url}/${player.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(player)
    });

    if (!response.ok) {
      throw new Error('Failed to edit player');
    }

    return await response.json();
  }
  async deletePlayer(playerId: number): Promise<boolean> {
    const response = await fetch(`${this.url}/${playerId}`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
      throw new Error('Failed to delete player');
    }

    return true;
  }

  async enablePlayer(playerId: number): Promise<boolean> {
    const response = await fetch(`${this.url}/${playerId}/enable`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
      throw new Error('Failed to enable player');
    }

    return true;
  }
  async getPlayerWithImage(playerId: number): Promise<Player> {
    const response = await fetch(`${this.url}/${playerId}/with-image`);

    if (!response.ok) {
      throw new Error('Failed to get player with image');
    }

    return await response.json();
  }
  // Noi metode pentru gestionarea disponibilității jucătorilor
  async setPlayerAvailable(playerId: number): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/${playerId}/set-available`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' }
      });

      if (!response.ok) {
        throw new Error('Failed to set player available');
      }

      return true;
    } catch (error) {
      console.error('Error setting player available:', error);
      return false;
    }
  }

  async setPlayerUnavailable(playerId: number): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/${playerId}/set-unavailable`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' }
      });

      if (!response.ok) {
        throw new Error('Failed to set player unavailable');
      }

      return true;
    } catch (error) {
      console.error('Error setting player unavailable:', error);
      return false;
    }
  }

  async setMultiplePlayersAvailable(playerIds: number[]): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/set-multiple-available`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(playerIds)
      });

      if (!response.ok) {
        throw new Error('Failed to set multiple players available');
      }

      return true;
    } catch (error) {
      console.error('Error setting multiple players available:', error);
      return false;
    }
  }

  async setMultiplePlayersUnavailable(playerIds: number[]): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/set-multiple-unavailable`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(playerIds)
      });

      if (!response.ok) {
        throw new Error('Failed to set multiple players unavailable');
      }

      return true;
    } catch (error) {
      console.error('Error setting multiple players unavailable:', error);
      return false;
    }
  }

  async clearAllAvailablePlayers(): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/clear-all-available`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' }
      });

      if (!response.ok) {
        throw new Error('Failed to clear all available players');
      }

      return true;
    } catch (error) {
      console.error('Error clearing all available players:', error);
      return false;
    }
  }

  async getAvailablePlayers(): Promise<Player[]> {
    try {
      const response = await fetch(`${this.url}/available`);

      if (!response.ok) {
        throw new Error('Failed to fetch available players');
      }

      const players = await response.json();
      return players;
    } catch (error) {
      console.error('Error fetching available players:', error);
      throw error;
    }
  }

  async searchPlayers(searchTerm: string): Promise<Player[]> {
    try {
      const response = await fetch(`${this.url}/search?searchTerm=${encodeURIComponent(searchTerm)}`);

      if (!response.ok) {
        throw new Error('Failed to search players');
      }

      const players = await response.json();
      return players;
    } catch (error) {
      console.error('Error searching players:', error);
      throw error;
    }
  }
}
