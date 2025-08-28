import { Component } from '@angular/core';
import { Header } from '../../components/header/header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.interface';
import { PlayerStatsComponent } from '../../components/player-stats.component/player-stats.component';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/user-role.enum';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';
import { MatchService } from '../../services/match.service';
import {
  CreateMatchRequest,
  MatchDisplay,
} from '../../models/create-match.interface';

@Component({
  selector: 'app-manage-players',
  standalone: true,
  imports: [
    Header,
    FormsModule,
    CommonModule,
    PlayerStatsComponent,
    FriendRequestsComponent,
  ],
  templateUrl: './manage-players.html',
  styleUrls: ['./manage-players.css'],
})
export class ManagePlayersComponent {
  constructor(
    private UserService: UserService,
    private authService: AuthService,
    private matchService: MatchService
  ) {}

  players: User[] = [];
  availablePlayers: User[] = [];
  filteredPlayers: User[] = [];
  activeTab: 'players' | 'matches' | 'myMatches' = 'matches';
  selectedMatch: MatchDisplay | null = null;
  playerLoading = false;
  playerErrorMessage = '';
  playerSuccessMessage = '';

  // Edit mode variables
  editIndex: number | null = null;
  editedPlayer: User | null = null;

  // Search functionality
  searchTerm: string = '';

  // Modal states
  showAddModal = false;

  private async loadAvailablePlayers() {
    const organiserId = this.authService.getUserId()!;

    this.availablePlayers = [...this.players];

    try {
      const organiser = await this.UserService.getUserById(organiserId);
      if (organiser) {
        const organiserAsPlayer: User = {
          ...organiser,
          lastName: `${organiser.lastName} (myself)`,
        };
        this.availablePlayers = [organiserAsPlayer, ...this.availablePlayers];
      }
    } catch (error) {
      console.error('Error fetching organizer details:', error);
    }
  }

  async init() {
    const role = this.authService.getUserRole();

    if (role === UserRole.ADMIN) {
      this.players = await this.UserService.getPlayers();
      this.availablePlayers = [...this.players];
    } else if (role === UserRole.ORGANISER) {
      this.players = await this.UserService.getPlayersForOrganiser(
        this.authService.getUserId()!
      );
    }
  }

  ngOnInit() {
    this.init();
  }

  // Player management
  isPlayerEnabled(player: User): boolean {
    return !player.isDeleted;
  }

  // Search functionality
  onSearchChange() {}

  newPlayer = {
    firstName: '',
    lastName: '',
    email: '',
    rating: 0,
    username: '',
    speed: 2,
    stamina: 2,
    errors: 2,
  };

  async addPlayer() {
    if (
      !this.newPlayer.firstName ||
      !this.newPlayer.lastName ||
      !this.newPlayer.email ||
      !this.newPlayer.username
    ) {
      this.playerErrorMessage =
        'First Name, Last Name, Email, and Username are required';
      return;
    }

    console.log('new player:  ', this.newPlayer);
    if (this.newPlayer.rating < 0 || this.newPlayer.rating > 10) {
      this.playerErrorMessage = 'Rating must be between 0 and 10.';
      return;
    }

    this.playerLoading = true;
    this.playerErrorMessage = '';
    this.playerSuccessMessage = '';

    try {
      const addedPlayer = await this.UserService.addPlayer(this.newPlayer);
      await this.UserService.addPlayerOrganiserRelation(addedPlayer.id!);

      this.players.push(addedPlayer);

      this.playerSuccessMessage = `Player ${this.newPlayer.firstName} ${this.newPlayer.lastName} added successfully!`;

      this.resetPlayer();
      this.players = await this.UserService.getPlayersForOrganiser(
        this.authService.getUserId()!
      );

      setTimeout(() => {
        this.showAddModal = false;
        this.playerSuccessMessage = '';
      }, 2000);
    } catch (error: any) {
      this.playerErrorMessage =
        error.message || 'Failed to add player. Please try again.';
      console.error('Error adding player:', error);
    } finally {
      this.playerLoading = false;
    }
  }

  resetPlayer() {
    this.newPlayer = {
      firstName: '',
      lastName: '',
      email: '',
      username: '',
      rating: 0,
      speed: 2,
      stamina: 2,
      errors: 2,
    };
  }

  setEditIndex(index: number) {
    this.editIndex = index;
    this.editedPlayer = { ...this.players[index] };
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
      const index = this.players.findIndex((p) => p.id === updatedPlayer.id);
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
      const success = await this.UserService.deleteUser(playerId);
      if (success) {
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isDeleted = true;
        }

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
      const success = await this.UserService.editUser(
        this.players.find((p) => p.id === playerId)!
      );
      if (success) {
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isDeleted = false;
        }

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
}
