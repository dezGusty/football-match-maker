import { Injectable } from '@angular/core';
import { Match } from './match.interface';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  private readonly baseUrl: string = 'http://localhost:5145/api';

  constructor() { }

  async getMatches(): Promise<Match[]> {
    const response = await fetch(`${this.baseUrl}/matches`);
    if (!response.ok) {
      throw new Error('Failed to fetch matches');
    }

    const rawMatches = await response.json();

    const matches: Match[] = rawMatches.map((m: any) => ({
      id: m.id,
      matchDate: m.matchDate,
      teamAId: m.teamAId,
      teamBId: m.teamBId,
      scoreA: m.teamAGoals,
      scoreB: m.teamBGoals,
      playerHistory: m.playerHistory
    }));

    return matches;
  }

  async getTeamById(teamId: number): Promise<string> {
    const response = await fetch(`${this.baseUrl}/teams/${teamId}`);
    if (!response.ok) {
      throw new Error('Failed to fetch team');
    }

    const team = await response.json();
    return team.name;
  }

  async createMatch(teamAId: number, teamBId: number, matchDate: Date): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ teamAId, teamBId, matchDate })
    });

    if (!response.ok) {
      throw new Error('Failed to create match');
    }

    return await response.json();
  }

  async getPlayersByTeamId(teamId: number): Promise<string[]> {
    const response = await fetch(`${this.baseUrl}/teams/${teamId}`);
    if (!response.ok) {
      throw new Error('Failed to fetch team players');
    }

    const team = await response.json();
    return team.currentPlayers.map((p: any) => `${p.firstName} ${p.lastName}`);
  }

  async updateMatch(matchId: number, updateData: { teamAGoals: number, teamBGoals: number }): Promise<Match> {
    // First get the current match data
    const currentMatch = await this.getMatchById(matchId);
    if (!currentMatch) {
      throw new Error('Match not found');
    }

    // Prepare the update data with all required fields
    const updateMatchDto = {
      matchDate: currentMatch.matchDate,
      teamAId: currentMatch.teamAId,
      teamBId: currentMatch.teamBId,
      teamAGoals: updateData.teamAGoals,
      teamBGoals: updateData.teamBGoals
    };

    const response = await fetch(`${this.baseUrl}/matches/${matchId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updateMatchDto)
    });

    if (!response.ok) {
      throw new Error('Failed to update match');
    }

    return await response.json();
  }

  async getMatchById(matchId: number): Promise<Match> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}`);
    if (!response.ok) {
      throw new Error('Failed to fetch match');
    }
    return await response.json();
  }

  getPlayersFromMatch(match: Match, teamId: number): string[] {
    return match.playerHistory
      .filter(ph => ph.teamId === teamId && ph.player)
      .map(ph => `${ph.player.firstName} ${ph.player.lastName}`);
  }
}
