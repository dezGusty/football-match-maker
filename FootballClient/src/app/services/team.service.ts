import { Injectable } from '@angular/core';
import { TeamCreated } from '../models/teamCreated.interface';

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  private readonly url: string = 'http://localhost:5145/api/teams';

  async createTeam(name: string): Promise<TeamCreated> {
    const response = await fetch(this.url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name }),
    });

    if (!response.ok) {
      throw new Error('Failed to create team');
    }

    return await response.json();
  }
}
