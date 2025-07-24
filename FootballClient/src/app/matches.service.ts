import { Injectable } from '@angular/core';
import { Match } from './match.interface';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  private readonly url: string = 'http://localhost:5145/api';
  constructor() { }

  async getMatches(): Promise<Match[]> {
  const response = await fetch(`${this.url}/matches`);
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
    scoreB: m.teamBGoals
  }));

  return matches;
}


async getTeamById(teamId: number): Promise<string> {
    const response = await fetch(`${this.url}/teams/${teamId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch team');
    }
    const team = await response.json();
    return team.name;
}
}
