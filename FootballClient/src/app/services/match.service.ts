import { Injectable } from '@angular/core';
import { Match } from '../models/match.interface';
import { MatchCreated } from '../models/matchCreated.interface';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  private readonly baseUrl: string = 'http://localhost:5145/api';
  // Cache pentru numele echipelor pentru a reduce request-urile
  private teamNamesCache: Map<number, string> = new Map();

  constructor() { }

  async getMatches(): Promise<Match[]> {
    const response = await fetch(`${this.baseUrl}/matches`);
    if (!response.ok) {
      throw new Error('Failed to fetch matches');
    }

    const rawMatches = await response.json();

    // Pentru fiecare match, obține numele echipelor dacă lipsesc
    const matches: Match[] = await Promise.all(
      (rawMatches as Match[]).map(async (m) => ({
        id: m.id,
        matchDate: m.matchDate,
        teamAId: m.teamAId,
        teamBId: m.teamBId,
        teamAName: m.teamAName || await this.getTeamById(m.teamAId),
        teamBName: m.teamBName || await this.getTeamById(m.teamBId),
        scoreA: m.scoreA,
        scoreB: m.scoreB,
        playerHistory: m.playerHistory
      }))
    );

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

      // Pentru fiecare match, asigură-te că ai numele echipelor
      const matches: Match[] = await Promise.all(
        (rawMatches as Match[]).map(async (m) => {
          let teamAName = m.teamAName;
          let teamBName = m.teamBName;

          // Dacă numele nu vin din API sau sunt default values, obține-le
          if (!teamAName || teamAName === 'Team A') {
            teamAName = await this.getTeamById(m.teamAId);
          }
          if (!teamBName || teamBName === 'Team B') {
            teamBName = await this.getTeamById(m.teamBId);
          }

          return {
            id: m.id,
            matchDate: m.matchDate,
            teamAId: m.teamAId,
            teamBId: m.teamBId,
            teamAName: teamAName,
            teamBName: teamBName,
            scoreA: m.teamAGoals || m.scoreA || 0,
            scoreB: m.teamBGoals || m.scoreB || 0,
            playerHistory: m.playerHistory || []
          };
        })
      );

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
    // Verifică cache-ul mai întâi
    if (this.teamNamesCache.has(teamId)) {
      return this.teamNamesCache.get(teamId)!;
    }

    try {
      const response = await fetch(`${this.baseUrl}/teams/${teamId}`);
      if (!response.ok) {
        console.error(`Failed to fetch team ${teamId}`);
        const fallbackName = `Team ${teamId}`;
        this.teamNamesCache.set(teamId, fallbackName);
        return fallbackName;
      }

      const team = await response.json();
      const teamName = team.name || `Team ${teamId}`;

      // Salvează în cache
      this.teamNamesCache.set(teamId, teamName);
      return teamName;
    } catch (error) {
      console.error(`Error fetching team ${teamId}:`, error);
      const fallbackName = `Team ${teamId}`;
      this.teamNamesCache.set(teamId, fallbackName);
      return fallbackName;
    }
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
    return team.currentPlayers.map((p: any) => `${p.firstName} ${p.lastName}`);
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

    const match = await response.json();

    // Asigură-te că și aici obții numele echipelor dacă lipsesc
    if (!match.teamAName) {
      match.teamAName = await this.getTeamById(match.teamAId);
    }
    if (!match.teamBName) {
      match.teamBName = await this.getTeamById(match.teamBId);
    }

    return match;
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

      // Pentru match-urile din trecut, obține numele echipelor dacă lipsesc
      const matches: Match[] = await Promise.all(
        (rawMatches as Match[]).map(async (m) => {
          let teamAName = m.teamAName;
          let teamBName = m.teamBName;

          if (!teamAName || teamAName === 'Team A') {
            teamAName = await this.getTeamById(m.teamAId);
          }
          if (!teamBName || teamBName === 'Team B') {
            teamBName = await this.getTeamById(m.teamBId);
          }

          return {
            id: m.id,
            matchDate: m.matchDate,
            teamAId: m.teamAId,
            teamBId: m.teamBId,
            teamAName: teamAName,
            teamBName: teamBName,
            scoreA: m.teamAGoals || m.scoreA || 0,
            scoreB: m.teamBGoals || m.scoreB || 0,
            playerHistory: m.playerHistory || []
          };
        })
      );

      return matches;
    } catch (error) {
      console.error('Error fetching past matches:', error);
      throw error;
    }
  }

  // Metodă pentru a curăța cache-ul dacă este necesar
  clearTeamNamesCache(): void {
    this.teamNamesCache.clear();
  }

  // Metodă pentru a pre-încărca numele echipelor (opțională)
  async preloadTeamNames(teamIds: number[]): Promise<void> {
    const promises = teamIds
      .filter(id => !this.teamNamesCache.has(id))
      .map(id => this.getTeamById(id));

    await Promise.all(promises);
  }
}