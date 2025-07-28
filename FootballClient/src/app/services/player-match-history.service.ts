import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PlayerMatchHistoryService {
  private readonly url: string = 'http://localhost:5145/api/playermatchhistory';

  async createPlayerMatchHistory(playerId: number, teamId: number, matchId: number): Promise<any> {
    const response = await fetch(this.url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        playerId,
        teamId,
        matchId,
        performanceRating: 0 // Initial rating, can be updated later
      })
    });

    if (!response.ok) {
      throw new Error('Failed to create player match history');
    }

    return await response.json();
  }
} 