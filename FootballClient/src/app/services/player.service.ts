import { Injectable } from '@angular/core';
import { Player } from '../models/player.interface';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class PlayerService {
  private readonly url: string = 'http://localhost:5145/api/players';
  private readonly MAX_RATING = 10000;
  private readonly MIN_RATING = 0;

  constructor(private authService: AuthService) {}

  private getAuthHeaders(): HeadersInit {
    const token = this.authService.getToken();
    return {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
    };
  }

  private validateRating(rating: number): void {
    if (rating < this.MIN_RATING || rating > this.MAX_RATING) {
      throw new Error(
        `Rating must be between ${this.MIN_RATING} and ${this.MAX_RATING}`
      );
    }
  }

  async getPlayers(): Promise<Player[]> {
    const response = await fetch(`${this.url}`);
    if (!response.ok) {
      throw new Error('Failed to fetch players');
    }
    const players = await response.json();
    return players;
  }

  async getPlayersForOrganiser(organiserId: number): Promise<Player[]> {
    const response = await fetch(
      `http://localhost:5145/api/user/${organiserId}/players`
    );
    if (!response.ok) {
      throw new Error('Failed to fetch players for organiser');
    }
    return await response.json();
  }

  async addPlayer(player: {
    firstName: string;
    lastName: string;
    email?: string;
    rating: number;
    username: string;
  }): Promise<Player> {
    this.validateRating(player.rating);

    const response = await fetch(
      'http://localhost:5145/api/Auth/create-player-account',
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(player),
      }
    );

    if (!response.ok) {
      throw new Error('Failed to add player');
    }

    return await response.json();
  }

  async updatePlayer(
    playerId: number,
    updateData: Partial<Player>
  ): Promise<Player> {
    if (updateData.rating !== undefined) {
      this.validateRating(updateData.rating);
    }

    const response = await fetch(`${this.url}/${playerId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updateData),
    });

    if (!response.ok) {
      throw new Error('Failed to update player');
    }

    return await response.json();
  }

  async editPlayer(player: Player): Promise<Player> {
    if (player.rating !== undefined) {
      this.validateRating(player.rating);
    }

    const response = await fetch(`${this.url}/${player.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(player),
    });

    if (!response.ok) {
      throw new Error('Failed to edit player');
    }

    return await response.json();
  }
  async deletePlayer(playerId: number): Promise<boolean> {
    const response = await fetch(`${this.url}/${playerId}`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
    });

    if (!response.ok) {
      throw new Error('Failed to delete player');
    }

    return true;
  }

  async enablePlayer(playerId: number): Promise<boolean> {
    const response = await fetch(`${this.url}/${playerId}/enable`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
    });

    if (!response.ok) {
      throw new Error('Failed to enable player');
    }

    return true;
  }

  async setPlayerAvailable(playerId: number): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/${playerId}/set-available`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
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
        headers: { 'Content-Type': 'application/json' },
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
        body: JSON.stringify(playerIds),
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
        body: JSON.stringify(playerIds),
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
        headers: { 'Content-Type': 'application/json' },
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
      const response = await fetch(
        `${this.url}/search?searchTerm=${encodeURIComponent(searchTerm)}`
      );

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

  async updatePlayerRating(
    playerId: number,
    ratingChange: number
  ): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/${playerId}/update-rating`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ratingChange }),
      });

      if (!response.ok) {
        throw new Error('Failed to update player rating');
      }

      return true;
    } catch (error) {
      console.error('Error updating player rating:', error);
      return false;
    }
  }

  async updateMultiplePlayerRatings(
    playerRatingUpdates: { playerId: number; ratingChange: number }[]
  ): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/update-multiple-ratings`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          playerRatingUpdates: playerRatingUpdates.map((update) => ({
            playerId: update.playerId,
            ratingChange: update.ratingChange,
          })),
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to update multiple player ratings');
      }

      return true;
    } catch (error) {
      console.error('Error updating multiple player ratings:', error);
      return false;
    }
  }

  async addPlayerOrganiserRelation(
    playerId: number
  ): Promise<void> {
    const body = { playerId };
    const headers = this.getAuthHeaders();


    const response = await fetch(`${this.url}/player-organiser`, {
      method: 'POST',
      headers,
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Response error:', response.status, errorText);
      throw new Error(
        `Failed to add player-organiser relation: ${response.status} ${errorText}`
      );
    }
  }
}
