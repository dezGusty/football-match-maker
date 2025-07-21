import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { inject } from '@angular/core';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';
import { Header } from '../header/header';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [Header, FormsModule, CommonModule],
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

     if (this.newPlayer.rating <= 0) {
    alert('Rating must be a positive number.');
    return;
  }
    try {
      const addedPlayer = await this.PlayerService.addPlayer(this.newPlayer);
      this.players.push(addedPlayer);
      this.newPlayer = { firstName: '', lastName: '', rating: 0 };
      console.log('Player added:', addedPlayer);
    } catch (error) {
      console.error('Error adding player:', error);
    }
  }

  async editPlayer(player: Player) {
    try {
      const updatedPlayer = await this.PlayerService.editPlayer(player);
      const index = this.players.findIndex(p => p.id === updatedPlayer.id);
      if (index !== -1) {
        this.players[index] = updatedPlayer;
      }
      this.clearEditIndex();
      console.log('Player updated:', updatedPlayer);
    } catch (error) {
      console.error('Error updating player:', error);
    }
  }

  setEditIndex(index: number) {
    this.editIndex = index;
  }

  clearEditIndex() {
    this.editIndex = null;
  }
}
