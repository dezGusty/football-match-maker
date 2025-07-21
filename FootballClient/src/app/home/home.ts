import { Component } from '@angular/core';
import { inject } from '@angular/core';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';
import { Header } from '../header/header';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [Header, FormsModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home {
  players: Player[] = [];
  editIndex: number | null = null;
  showAddModal = false;
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
  newPlayer = {
    firstName: '',
    lastName: '',
    rating: 0
  };

  async addPlayer() {
    try {
      const addedPlayer = await this.PlayerService.addPlayer(this.newPlayer);
      this.players.push(addedPlayer);
      this.newPlayer = { firstName: '', lastName: '', rating: 0 };
      console.log('Player added:', addedPlayer);
    } catch (error) {
      console.error('Error adding player:', error);
    }
  }


  setEditIndex(index: number) {
    this.editIndex = index;
  }

  clearEditIndex() {
    this.editIndex = null;
  }
}
