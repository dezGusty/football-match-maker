import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  private readonly url: string = 'http://localhost:5145/api/matches';

  async createMatch(teamAId: number, teamBId: number, matchDate: Date): Promise<any> {
    const response = await fetch(this.url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        teamAId,
        teamBId,
        matchDate
      })
    });

    if (!response.ok) {
      throw new Error('Failed to create match');
    }

    return await response.json();
  }
} 