import { Injectable } from '@angular/core';
import {Player} from './player.interface';

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
  
}
