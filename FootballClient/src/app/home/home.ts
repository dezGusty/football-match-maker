import { Component } from '@angular/core';
import { Header } from '../header/header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [Header, FormsModule, CommonModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home {
  players: Player[] = [];
  editIndex: number | null = null;
  editedPlayer: Player | null = null;
  showAddModal = false;
  selectedFile: File | null = null;
  selectedFileName: string = '';

  constructor(private PlayerService: PlayerService) { }

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
    rating: 0,
    imageUrl: ''
  };

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.selectedFileName = file.name;
    }
  }

  async uploadImage(file: File): Promise<string> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch('http://localhost:5145/api/images/upload', {
      method: 'POST',
      body: formData
    });

    if (!response.ok) {
      throw new Error('Failed to upload image');
    }

    const result = await response.json();
    return result.imageUrl;
  }

  async addPlayer() {
    if (this.newPlayer.rating < 0 || this.newPlayer.rating > 10) {
      alert('Rating must be between 0 and 10.');
      return;
    }

    try {
      // Upload image if selected
      if (this.selectedFile) {
        try {
          const imageUrl = await this.uploadImage(this.selectedFile);
          this.newPlayer.imageUrl = imageUrl;
        } catch (error) {
          console.error('Error uploading image:', error);
          alert('Failed to upload image. Player will be added without an image.');
        }
      }

      const addedPlayer = await this.PlayerService.addPlayer(this.newPlayer);
      this.players.push(addedPlayer);
      this.newPlayer = { firstName: '', lastName: '', rating: 0, imageUrl: '' };
      this.selectedFile = null;
      this.selectedFileName = '';
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
      alert('Rating must be between 0 and 10.');
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
    const confirmDelete = confirm('Are you sure ?');
    if (!confirmDelete) return;

    try {
      const success = await this.PlayerService.deletePlayer(playerId);
      if (success) {
        await this.init();
        console.log('Player deleted successfully');
      }
    } catch (error) {
      console.error('Error deleting player:', error);
      alert('Failed to delete player. Please try again.');
    }
  }

  async enablePlayer(playerId: number) {
    const confirmEnable = confirm('Are you sure?');
    if (!confirmEnable) return;

    try {
      const success = await this.PlayerService.enablePlayer(playerId);
      if (success) {
        await this.init();
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