import { Component } from '@angular/core';
import { Header } from '../../components/header/header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.interface';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-manage-all-players',
  standalone: true,
  imports: [Header, FormsModule, CommonModule],
  templateUrl: './manage-all-players.html',
  styleUrls: ['./manage-all-players.css'],
})
export class ManageAllPlayersComponent {
  constructor(
    private UserService: UserService,
    private authService: AuthService
  ) {}

  players: User[] = [];
  filteredPlayers: User[] = [];
  playerLoading = false;
  playerErrorMessage = '';
  playerSuccessMessage = '';

  editIndex: number | null = null;
  editedPlayer: User | null = null;

  searchTerm: string = '';

  async init() {
    const role = this.authService.getUserRole();

    if (role === UserRole.ADMIN) {
      this.players = await this.UserService.getPlayers();
      this.filteredPlayers = [...this.players];
    }
  }

  ngOnInit() {
    this.init();
  }

  isPlayerEnabled(player: User): boolean {
    return !player.isDeleted;
  }

  onSearchChange() {
    if (!this.searchTerm) {
      this.filteredPlayers = [...this.players];
    } else {
      this.filteredPlayers = this.players.filter(
        (player) =>
          player.firstName
            ?.toLowerCase()
            .includes(this.searchTerm.toLowerCase()) ||
          player.lastName
            ?.toLowerCase()
            .includes(this.searchTerm.toLowerCase()) ||
          player.username
            ?.toLowerCase()
            .includes(this.searchTerm.toLowerCase()) ||
          player.email?.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }
  }

  setEditIndex(index: number) {
    this.editIndex = index;
    this.editedPlayer = { ...this.filteredPlayers[index] };
  }

  async editPlayer() {
    if (!this.editedPlayer) return;

    if (
      typeof this.editedPlayer.rating === 'number' &&
      (this.editedPlayer.rating < 0 || this.editedPlayer.rating > 10)
    ) {
      alert('Rating must be between 0 and 10.');
      return;
    }

    try {
      const updatedPlayer = await this.UserService.editUser(this.editedPlayer);
      const playerIndex = this.players.findIndex(
        (p) => p.id === updatedPlayer.id
      );
      if (playerIndex !== -1) {
        this.players[playerIndex] = updatedPlayer;
      }
      const filteredIndex = this.filteredPlayers.findIndex(
        (p) => p.id === updatedPlayer.id
      );
      if (filteredIndex !== -1) {
        this.filteredPlayers[filteredIndex] = updatedPlayer;
      }
      this.clearEditIndex();
    } catch (error) {
      console.error('Error updating player:', error);
    }
  }

  async deletePlayer(playerId: number) {
    const confirmDelete = confirm(
      'Are you sure you want to delete this player?'
    );
    if (!confirmDelete) return;

    try {
      const success = await this.UserService.deleteUser(playerId);
      if (success) {
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isDeleted = true;
        }
        const filteredIndex = this.filteredPlayers.findIndex(
          (p) => p.id === playerId
        );
        if (filteredIndex !== -1) {
          this.filteredPlayers[filteredIndex].isDeleted = true;
        }
      }
    } catch (error) {
      console.error('Error deleting player:', error);
      alert('Failed to delete player. Please try again.');
    }
  }

  async enablePlayer(playerId: number) {
    const confirmEnable = confirm(
      'Are you sure you want to reactivate this player?'
    );
    if (!confirmEnable) return;

    try {
      const success = await this.UserService.reactivateUser(playerId);
      if (success) {
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isDeleted = false;
        }
        const filteredIndex = this.filteredPlayers.findIndex(
          (p) => p.id === playerId
        );
        if (filteredIndex !== -1) {
          this.filteredPlayers[filteredIndex].isDeleted = false;
        }
      }
    } catch (error) {
      console.error('Error reactivating player:', error);
      alert('Failed to reactivate player. Please try again.');
    }
  }

  clearEditIndex() {
    this.editIndex = null;
    this.editedPlayer = null;
  }
}
