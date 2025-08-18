import { Component } from '@angular/core';
import { Header } from '../../components/header/header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../../services/player.service';
import { Player } from '../../models/player.interface';
import { PlayerStatsComponent } from '../../components/player-stats.component/player-stats.component';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/user-role.enum';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    Header,
    FormsModule,
    CommonModule,
    PlayerStatsComponent,
    FriendRequestsComponent,
  ],
  templateUrl: './home.html',
  styleUrls: ['./home.css'],
})
export class Home {
  constructor(
    private PlayerService: PlayerService,
    private authService: AuthService
  ) {}

  players: Player[] = [];
  filteredPlayers: Player[] = [];
  searchTerm: string = '';
  editIndex: number | null = null;
  editedPlayer: Player | null = null;
  showAddModal = false;

  async init() {
    const role = this.authService.getUserRole();
    if (role === UserRole.ADMIN) {
      this.players = await this.PlayerService.getPlayers();
    } else if (role === UserRole.ORGANISER) {
      this.players = await this.PlayerService.getPlayersForOrganiser(
        this.authService.getUserId()!
      );
    }
    this.filterPlayers();
  }

  ngOnInit() {
    this.init();
  }

  filterPlayers() {
    this.filteredPlayers = this.players.filter((player) => {
      const isActive = this.isPlayerEnabled(player);
      const searchTerms = this.searchTerm.toLowerCase().trim().split(' ');
      const fullName = `${player.firstName || ''} ${
        player.lastName || ''
      }`.toLowerCase();

      const matchesSearch =
        this.searchTerm.trim() === '' ||
        searchTerms.every((term) => fullName.includes(term));

      return isActive && matchesSearch;
    });
  }

  onSearchChange() {
    this.filterPlayers();
  }

  newPlayer = {
    firstName: '',
    lastName: '',
    email: '',
    rating: 0,

    speed: 2,
    stamina: 2,
    errors: 2,
  };

  async addPlayerOrganiserRelation(
    playerId: number,
    organiserId: number | null = this.authService.getUserId()
  ): Promise<void> {
    const response = await fetch('http://localhost:5145/api/playerorganisers', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ playerId, organiserId }),
    });
    if (!response.ok) {
      alert('A player cannot add another player.');
    }
  }
  selectedFileName: string | null = null;

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input?.files && input.files.length > 0) {
      this.selectedFileName = input.files[0].name;
    } else {
      this.selectedFileName = null;
    }
  }

  async addPlayer() {
    if (this.newPlayer.rating < 0 || this.newPlayer.rating > 10000) {
      alert('Rating must be between 0 and 10000.');
      return;
    }

    try {
      const addedPlayer = await this.PlayerService.addPlayer(this.newPlayer);
      await this.PlayerService.addPlayerOrganiserRelation(
        addedPlayer.id!,
        this.authService.getUserId()!
      );

      this.players.push(addedPlayer);
      this.newPlayer = {
        firstName: '',
        lastName: '',
        email: '',
        rating: 0,
        speed: 2,
        stamina: 2,
        errors: 2,
      };
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

    if (
      typeof this.editedPlayer.rating === 'number' &&
      (this.editedPlayer.rating < 0 || this.editedPlayer.rating > 10000)
    ) {
      alert('Rating must be between 0 and 10000.');
      return;
    }

    try {
      const updatedPlayer = await this.PlayerService.editPlayer(
        this.editedPlayer
      );
      const index = this.players.findIndex((p) => p.id === updatedPlayer.id);
      if (index !== -1) {
        this.players[index] = updatedPlayer;
        this.filterPlayers();
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
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isEnabled = false;
        }

        this.filterPlayers();
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
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isEnabled = true;
        }

        this.filterPlayers();
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
