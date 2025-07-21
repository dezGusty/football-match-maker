import { Injectable } from '@angular/core';
import { Player } from './player.interface';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  private readonly url: string = 'http://localhost:5145/api/players';
  constructor() { }
  async getPlayers(): Promise<Player[]> {
    const response = await fetch(`${this.url}`);
    const players = await response.json();
    return players;
  }
  async addPlayer(player: { firstName: string; lastName: string; rating: number }): Promise<Player> {
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
}
