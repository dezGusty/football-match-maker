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
  editedPlayer: Player | null = null;
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

  async editPlayer() {
    if (!this.editedPlayer) return;
    try {
      const updatedPlayer = await this.PlayerService.editPlayer(this.editedPlayer);
      const index = this.players.findIndex(p => p.id === updatedPlayer.id);
      if (index !== -1) {
        this.players[index] = updatedPlayer;
      }
      if (typeof updatedPlayer.rating === 'number' && updatedPlayer.rating <= 0) {
        throw new Error('Rating must be a positive number.');
      }
      this.clearEditIndex();
      console.log('Player updated:', updatedPlayer);
    } catch (error) {
      console.error('Error updating player:', error);
    }
  }
  // Metodă nouă pentru soft delete
  // Metodă nouă pentru soft delete
  async deletePlayer(playerId: number, playerIndex: number) {
    // Confirmă ștergerea
    const confirmDelete = confirm('Are you sure you want to delete this player?');
    if (!confirmDelete) return;

    try {
      const success = await this.PlayerService.deletePlayer(playerId);
      if (success) {
        // Actualizează lista local - jucătorul va fi marcat ca dezactivat
        // și va afișa "Player{id}" în loc de numele real
        await this.init(); // Reîncarcă lista pentru a afișa numele actualizat
        console.log('Player soft deleted successfully');
      }
    } catch (error) {
      console.error('Error deleting player:', error);
      alert('Failed to delete player. Please try again.');
    }
  }


  setEditIndex(index: number) {
    this.editIndex = index;
    this.editedPlayer = { ...this.players[index] };
  }

  clearEditIndex() {
    this.editIndex = null;
    this.editedPlayer = null;
  }
  // Metodă helper pentru a verifica dacă un jucător este enabled
  isPlayerEnabled(player: Player): boolean {
    return player.isEnabled !== false; // true by default dacă nu e setat
  }
}
