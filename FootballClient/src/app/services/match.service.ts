import { Injectable } from '@angular/core';
import { Match } from '../models/match.interface';
import { MatchCreated } from '../models/matchCreated.interface';
import {
  CreateMatchRequest,
  CreateMatchResponse,
} from '../models/create-match.interface';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MatchService {
  private readonly baseUrl: string = `${environment.apiUrl}`;
  private teamNamesCache: Map<number, string> = new Map();

  constructor(private authService: AuthService) {}

  async getMatches(): Promise<Match[]> {
    const response = await fetch(`${this.baseUrl}/matches`);
    if (!response.ok) {
      throw new Error('Failed to fetch matches');
    }

    const rawMatches = await response.json();

    const matches: Match[] = await Promise.all(
      (rawMatches as Match[]).map(async (m) => ({
        id: m.id,
        matchDate: m.matchDate,
        teamAId: m.teamAId,
        teamBId: m.teamBId,
        teamAName: m.teamAName || (await this.getTeamById(m.teamAId)),
        teamBName: m.teamBName || (await this.getTeamById(m.teamBId)),
        scoreA: m.scoreA,
        scoreB: m.scoreB,
        playerHistory: m.playerHistory,
      }))
    );

    return matches;
  }

  async getFutureMatches(): Promise<Match[]> {
    try {
      const response = await fetch(`${this.baseUrl}/matches/future`, {
        method: 'GET',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch future matches: ${errorText}`);
      }

      const rawMatches = await response.json();

      const matches: Match[] = await Promise.all(
        (rawMatches as any[]).map(async (m) => {
          return {
            id: m.id,
            matchDate: m.matchDate,
            teamAId: m.teamAId, // These might be undefined
            teamBId: m.teamBId, // These might be undefined
            teamAName: m.teamAName || 'Team A',
            teamBName: m.teamBName || 'Team B',
            scoreA: m.teamAGoals || m.scoreA || 0,
            scoreB: m.teamBGoals || m.scoreB || 0,
            playerHistory: m.playerHistory || [],
            isPublic: m.isPublic,
            location: m.location,
            cost: m.cost,
            organiserId: m.organiserId,
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
    const response = await fetch(
      `${this.baseUrl}/playermatchhistory/match/${matchId}`
    );
    if (!response.ok) {
      throw new Error('Failed to fetch players for scheduled match');
    }

    const playerHistory = await response.json();
    return playerHistory.map((ph: any) => ph.playerId);
  }

  async getTeamById(teamId: number): Promise<string> {
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

      this.teamNamesCache.set(teamId, teamName);
      return teamName;
    } catch (error) {
      console.error(`Error fetching team ${teamId}:`, error);
      const fallbackName = `Team ${teamId}`;
      this.teamNamesCache.set(teamId, fallbackName);
      return fallbackName;
    }
  }

  async createMatch(
    teamAId: number,
    teamBId: number,
    matchDate: Date
  ): Promise<MatchCreated> {
    const response = await fetch(`${this.baseUrl}/matches`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ teamAId, teamBId, matchDate }),
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
  async finalizeMatchServ(matchId: number): Promise<void> {
    const currentMatch = await this.getMatchById(matchId);
    if (!currentMatch) {
      throw new Error('Match not found');
    }
    const updateMatchDto = {
      matchDate: currentMatch.matchDate,
      teamAId: currentMatch.teamAId,
      teamBId: currentMatch.teamBId,
    };
    const response = await fetch(`${this.baseUrl}/matches/finalize/${matchId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updateMatchDto),
    });
    if (!response.ok) {
      throw new Error('Failed to update match');
    }
    if(response.ok){
      console.log('Match finalized successfully');
    }

    return await response.json();
  }
  async updateMatch(
    matchId: number,
    updateData: { teamAGoals: number; teamBGoals: number }
  ): Promise<Match> {
    const currentMatch = await this.getMatchById(matchId);
    if (!currentMatch) {
      throw new Error('Match not found');
    }

    const updateMatchDto = {
      matchDate: currentMatch.matchDate,
      teamAId: currentMatch.teamAId,
      teamBId: currentMatch.teamBId,
      teamAGoals: updateData.teamAGoals,
      teamBGoals: updateData.teamBGoals,
    };

    const response = await fetch(`${this.baseUrl}/matches/${matchId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updateMatchDto),
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
      .filter((ph) => ph.teamId === teamId && ph.user)
      .map((ph) => `${ph.user.firstName} ${ph.user.lastName}`);
  }

  async getPastMatches(): Promise<Match[]> {
    try {
      const response = await fetch(`${this.baseUrl}/matches/past`, {
        method: 'GET',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch past matches: ${errorText}`);
      }

      const rawMatches = await response.json();

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
            playerHistory: m.playerHistory || [],
          };
        })
      );

      return matches;
    } catch (error) {
      console.error('Error fetching past matches:', error);
      throw error;
    }
  }

  clearTeamNamesCache(): void {
    this.teamNamesCache.clear();
  }

  async preloadTeamNames(teamIds: number[]): Promise<void> {
    const promises = teamIds
      .filter((id) => !this.teamNamesCache.has(id))
      .map((id) => this.getTeamById(id));

    await Promise.all(promises);
  }

  private getAuthHeaders(): HeadersInit {
    const token = this.authService.getToken();
    return {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
    };
  }

  async createNewMatch(
    createMatchRequest: CreateMatchRequest
  ): Promise<CreateMatchResponse> {
    const token = this.authService.getToken();

    if (!token) {
      throw new Error('Nu ești autentificat');
    }

    const response = await fetch(`${this.baseUrl}/matches`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(createMatchRequest),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Nu ești autentificat');
      }
      throw new Error(`Eroare la crearea meciului: ${errorText}`);
    }

    return await response.json();
  }

  async getAllMatches(): Promise<any[]> {
    const response = await fetch(`${this.baseUrl}/matches`, {
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch matches');
    }

    return await response.json();
  }

  async getMatchesByOrganiser(): Promise<any[]> {
    const response = await fetch(`${this.baseUrl}/matches/organiser/matches`, {
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch matches by organiser');
    }

    return await response.json();
  }

  async addPlayerToMatch(
    matchId: number,
    playerId: number,
    teamId: number
  ): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}/players`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify({
        userId: playerId,
        teamId: teamId,
      }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      throw new Error(`Error adding player: ${errorText}`);
    }

    return await response.json();
  }

  async publishMatch(matchId: number): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}/publish`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 400) {
        throw new Error('Could not make match public');
      }
      throw new Error(`Error publishing match: ${errorText}`);
    }

    return await response.json();
  }

  async makeMatchPrivate(matchId: number): Promise<any> {
    const response = await fetch(
      `${this.baseUrl}/matches/${matchId}/unpublish`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 400) {
        throw new Error('Could not make match private');
      }
      throw new Error(`Error making match private: ${errorText}`);
    }

    return await response.json();
  }

  async closeMatch(matchId: number): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}/close`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 400) {
        throw new Error('Could not close match. Match needs at least 10 players or is not in Open status.');
      }
      throw new Error(`Error closing match: ${errorText}`);
    }

    return await response.json();
  }

  async cancelMatch(matchId: number): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}/cancel`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 400) {
        throw new Error('Could not cancel match. Match not found or invalid status.');
      }
      throw new Error(`Error cancelling match: ${errorText}`);
    }

    return await response.json();
  }

  async getPlayerMatches(): Promise<any[]> {
    const response = await fetch(`${this.baseUrl}/matches/my-matches`, {
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch player matches');
    }

    return await response.json();
  }

  async getPublicMatches(): Promise<any[]> {
    const response = await fetch(`${this.baseUrl}/matches/public`, {
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch public matches');
    }

    return await response.json();
  }

  async getAvailableMatches(): Promise<any[]> {
    const response = await fetch(`${this.baseUrl}/matches/available`, {
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch available matches');
    }

    return await response.json();
  }

  async joinMatch(matchId: number): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}/join`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 400) {
        throw new Error(
          'Could not join match. Match might be full, private, or you might already be in it.'
        );
      }
      throw new Error(`Error joining match: ${errorText}`);
    }

    return await response.json();
  }

  async joinTeam(matchId: number, teamId: number): Promise<any> {
    const response = await fetch(
      `${this.baseUrl}/matches/${matchId}/teams/${teamId}/join`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 400) {
        throw new Error(
          'Could not join team. Team might be full or you might already be in the match.'
        );
      }
      throw new Error(`Error joining team: ${errorText}`);
    }

    return await response.json();
  }

  async getMatchDetails(matchId: number): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}/details`, {
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch match details');
    }

    return await response.json();
  }

  async removePlayerFromMatch(matchId: number, playerId: number): Promise<any> {
    const response = await fetch(
      `${this.baseUrl}/matches/${matchId}/players/${playerId}`,
      {
        method: 'DELETE',
        headers: this.getAuthHeaders(),
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      throw new Error(`Error removing player: ${errorText}`);
    }

    return await response.json();
  }

  async leaveMatch(matchId: number): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}/leave`, {
      method: 'DELETE',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 400) {
        throw new Error(
          'Could not leave match. You might not be part of this match.'
        );
      }
      throw new Error(`Error leaving match: ${errorText}`);
    }

    return await response.json();
  }

  async updateMatchPlayer(
    matchId: number,
    updateData: {
      matchDate: string;
      location: string;
      cost?: number;
      teamAName?: string;
      teamBName?: string;
    }
  ): Promise<any> {
    const response = await fetch(`${this.baseUrl}/matches/${matchId}`, {
      method: 'PUT',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(updateData),
    });

    if (!response.ok) {
      const errorText = await response.text();
      if (response.status === 401) {
        throw new Error('Not authenticated');
      }
      if (response.status === 404) {
        throw new Error('Match not found');
      }
      throw new Error(`Error updating match: ${errorText}`);
    }

    return await response.json();
  }
}
