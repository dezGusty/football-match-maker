 import { Component } from '@angular/core';
 import { inject } from '@angular/core';
 import { PlayerService } from '../player.service';
 import { Player } from '../player.interface';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  players: Player[] = [];
  private PlayerService = inject(PlayerService);
  constructor() {
    this.init();
  }
  async init() {
    try {
      this.players = await this.PlayerService.getPlayers();
    } catch (error) {
      console.error('Error fetching players:', error);
    }
  }
}
