import { Component } from '@angular/core';
import { inject } from '@angular/core';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';
import { Header } from '../header/header';

@Component({
  selector: 'app-home',
  imports: [Header],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home {
  players: Player[] = [];
  editIndex: number | null = null;
  private PlayerService = inject(PlayerService);

  constructor() {
    this.init();
  }

  async init() {
    try {
      this.players = await this.PlayerService.getPlayers();
      console.log("Players fetched successfully:", this.players);
    } catch (error) {
      console.error('Error fetching players:', error);
    }
  }

  setEditIndex(index: number) {
    this.editIndex = index;
  }

  clearEditIndex() {
    this.editIndex = null;
  }
}
