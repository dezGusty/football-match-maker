import { Injectable } from '@angular/core';
import { PlayerMatchHistoryCreated } from '../models/playerMatchHistoryCreated.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PlayerMatchHistoryService {
  private readonly url: string = `${environment.apiUrl}/playermatchhistory`;

  async createPlayerMatchHistory(
    playerId: number,
    teamId: number,
    matchId: number
  ): Promise<PlayerMatchHistoryCreated> {
    const response = await fetch(this.url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        playerId,
        teamId,
        matchId,
        performanceRating: 0,
      }),
    });

    if (!response.ok) {
      throw new Error('Failed to create player match history');
    }

    return await response.json();
  }
}
