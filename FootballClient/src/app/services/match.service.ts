import { Injectable } from '@angular/core';
import { Match } from '../models/match.interface';
import { MatchCreated } from '../models/matchCreated.interface';

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

    const matches: Match[] = (rawMatches as Match[]).map((m) => ({
      id: m.id,
      matchDate: m.matchDate,
      teamAId: m.teamAId,
      teamBId: m.teamBId,
      scoreA: m.scoreA,
      scoreB: m.scoreB,
      playerHistory: m.playerHistory
    }));

    return matches;
  }
  async getFutureMatches(): Promise<Match[]> {
    try {
      const response = await fetch(`${this.baseUrl}/matches/future`, {
        method: 'GET',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch future matches: ${errorText}`);
      }

      const rawMatches = await response.json();

      const matches: Match[] = (rawMatches as Match[]).map((m) => ({
        id: m.id,
        matchDate: m.matchDate,
        teamAId: m.teamAId,
        teamBId: m.teamBId,
        teamAName: m.teamAName || 'Team A',
        teamBName: m.teamBName ||'Team B',
        scoreA: m.teamAGoals,
        scoreB: m.teamBGoals,
        playerHistory: m.playerHistory || []
      }));

      return matches;
    } catch (error) {
      console.error('Error fetching future matches:', error);
      throw error;
    }
  }

  async getPlayersForScheduledMatch(matchId: number): Promise<number[]> {
    const response = await fetch(`${this.baseUrl}/playermatchhistory/match/${matchId}`);
    if (!response.ok) {
      throw new Error('Failed to fetch players for scheduled match');
    }

    const playerHistory = await response.json();
    return playerHistory.map((ph: any) => ph.playerId);
  }
  async getTeamById(teamId: number): Promise<string> {
    const response = await fetch(`${this.baseUrl}/teams/${teamId}`);
    if (!response.ok) {
      throw new Error('Failed to fetch team');
    }

    const team = await response.json();
    return team.name;
  }

  async createMatch(teamAId: number, teamBId: number, matchDate: Date): Promise<MatchCreated> {
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
    return team.currentPlayers.map((p: MemberByTeamId) => `${p.firstName} ${p.lastName}`);
  }

  async updateMatch(matchId: number, updateData: { teamAGoals: number, teamBGoals: number }): Promise<Match> {
    const currentMatch = await this.getMatchById(matchId);
    if (!currentMatch) {
      throw new Error('Match not found');
    }

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
  async getPastMatches(): Promise<Match[]> {
    try {
      const response = await fetch(`${this.baseUrl}/matches/past`, {
        method: 'GET',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch past matches: ${errorText}`);
      }

      const rawMatches = await response.json();

      const matches: Match[] = (rawMatches as Match[]).map((m) => ({
        id: m.id,
        matchDate: m.matchDate,
        teamAId: m.teamAId,
        teamBId: m.teamBId,
        teamAName: m.teamAName ||'Team A',
        teamBName: m.teamBName || 'Team B',
        scoreA: m.teamAGoals,
        scoreB: m.teamBGoals,
        playerHistory: m.playerHistory || []
      }));

      return matches;
    } catch (error) {
      console.error('Error fetching past matches:', error);
      throw error;
    }
  }
}
