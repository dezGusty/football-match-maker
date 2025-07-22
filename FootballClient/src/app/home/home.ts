import { Component } from '@angular/core';
import { Header } from '../header/header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';

@Component({
  selector: 'app-home',
  imports: [Header, FormsModule, CommonModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home {
  players: Player[] = [];
  editIndex: number | null = null;
  editedPlayer: Player | null = null;
  showAddModal = false;

  constructor(private PlayerService: PlayerService) {}

  async init() {
    try {
      this.players = await this.PlayerService.getPlayers();
    } catch (error) {
      console.error('Error fetching players:', error);
    }
  }

  ngOnInit() {
    this.init();
  }

  newPlayer = {
    firstName: '',
    lastName: '',
    rating: 0
  };

  async addPlayer() {
    if (this.newPlayer.rating < 0 || this.newPlayer.rating > 10) {
      alert('Rating trebuie să fie între 0 și 10.');
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

  setEditIndex(index: number) {
    this.editIndex = index;
    this.editedPlayer = { ...this.players[index] };
  }

  async editPlayer() {
    if (!this.editedPlayer) return;
    
    if (typeof this.editedPlayer.rating === 'number' && (this.editedPlayer.rating < 0 || this.editedPlayer.rating > 10)) {
      alert('Rating trebuie să fie între 0 și 10.');
      return;
    }

    try {
      const updatedPlayer = await this.PlayerService.editPlayer(this.editedPlayer);
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

  async deletePlayer(playerId: number) {
    const confirmDelete = confirm('Sigur doriți să dezactivați acest jucător?');
    if (!confirmDelete) return;

    try {
      const success = await this.PlayerService.deletePlayer(playerId);
      if (success) {
        await this.init(); // Reîncarcă lista pentru a afișa statusul actualizat
        console.log('Player deleted successfully');
      }
    } catch (error) {
      console.error('Error deleting player:', error);
      alert('Failed to delete player. Please try again.');
    }
  }

  async enablePlayer(playerId: number) {
    const confirmEnable = confirm('Sigur doriți să reactivați acest jucător?');
    if (!confirmEnable) return;

    try {
      const success = await this.PlayerService.enablePlayer(playerId);
      if (success) {
        await this.init(); // Reîncarcă lista pentru a afișa statusul actualizat
        console.log('Player reactivated successfully');
      }
    } catch (error) {
      console.error('Error enabling player:', error);
      alert('Failed to reactivate player. Please try again.');
    }
  }

  clearEditIndex() {
    this.editIndex = null;
    this.editedPlayer = null;
  }

  isPlayerEnabled(player: Player): boolean {
    return player.isEnabled !== false;
  }
}