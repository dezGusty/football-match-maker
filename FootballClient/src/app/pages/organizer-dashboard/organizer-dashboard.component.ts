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
  CreateMatchResponse,
  MatchDisplay,
} from '../../models/create-match.interface';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: true,
  imports: [
    Header,
    FormsModule,
    CommonModule,
    PlayerStatsComponent,
    FriendRequestsComponent,
  ],
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.css'],
})
export class OrganizerDashboardComponent {
  constructor(
    private UserService: UserService,
    private authService: AuthService,
    private matchService: MatchService
  ) {}

  players: User[] = [];
  filteredPlayers: User[] = [];
  matches: MatchDisplay[] = [];
  searchTerm: string = '';
  editIndex: number | null = null;
  editedPlayer: User | null = null;
  showAddModal = false;
  showCreateMatchModal = false;
  showAddPlayersModal = false;
  activeTab: 'players' | 'matches' = 'matches';
  selectedMatch: MatchDisplay | null = null;
  matchDetails: any = null; // Will contain team IDs
  teamAPlayers: User[] = [];
  teamBPlayers: User[] = [];
  originalTeamAPlayers: User[] = [];
  originalTeamBPlayers: User[] = [];
  addingPlayers = false;
  savingPlayers = false;
  addPlayersErrorMessage = '';
  addPlayersSuccessMessage = '';
  playerLoading = false;
  playerErrorMessage = '';
  playerSuccessMessage = '';

  async init() {
    const role = this.authService.getUserRole();

    if (role === UserRole.ADMIN) {
      this.players = await this.UserService.getPlayers();
    } else if (role === UserRole.ORGANISER) {
      this.players = await this.UserService.getPlayersForOrganiser(
        this.authService.getUserId()!
      );
      await this.loadMatches();
    }

    this.filterPlayers();
  }

  async loadMatches() {
    try {
      const allMatches = await this.matchService.getAllMatches();
      this.matches = allMatches.map((match) => ({
        id: match.id,
        matchDate: match.matchDate,
        location: match.location,
        cost: match.cost,
        teamAName: match.teamAName,
        teamBName: match.teamBName,
        status: match.status,
        isPublic: match.isPublic,
      }));
    } catch (error) {
      console.error('Error loading matches:', error);
    }
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

    if (this.newPlayer.rating < 0 || this.newPlayer.rating > 10000) {
      this.playerErrorMessage = 'Rating must be between 0 and 10000.';
      return;
    }

    this.playerLoading = true;
    this.playerErrorMessage = '';
    this.playerSuccessMessage = '';

    try {
      const addedPlayer = await this.UserService.addPlayer(this.newPlayer);
      await this.UserService.addPlayerOrganiserRelation(addedPlayer.id!);

      this.players.push(addedPlayer);
      this.filterPlayers();

      this.playerSuccessMessage = `Player ${this.newPlayer.firstName} ${this.newPlayer.lastName} added successfully!`;

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
      const updatedPlayer = await this.UserService.editUser(this.editedPlayer);
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
      const success = await this.UserService.deleteUser(playerId);
      if (success) {
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isDeleted = true;
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
      const success = await this.UserService.editUser(
        this.players.find((p) => p.id === playerId)!
      );
      if (success) {
        const playerIndex = this.players.findIndex((p) => p.id === playerId);
        if (playerIndex !== -1) {
          this.players[playerIndex].isDeleted = false;
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

  isPlayerEnabled(player: User): boolean {
    return !player.isDeleted;
  }

  newMatch = {
    matchDate: this.getDefaultDateTime(),
    location: '',
    cost: null as number | null,
    teamAName: '',
    teamBName: '',
  };

  matchLoading = false;
  matchErrorMessage = '';
  matchSuccessMessage = '';

  async createMatch() {
    if (!this.newMatch.matchDate || !this.newMatch.location) {
      this.matchErrorMessage = 'Match date and location are required';
      return;
    }

    this.matchLoading = true;
    this.matchErrorMessage = '';
    this.matchSuccessMessage = '';

    try {
      const createMatchRequest: CreateMatchRequest = {
        matchDate: new Date(this.newMatch.matchDate).toISOString(),
        status: 1,
        location: this.newMatch.location,
        cost: this.newMatch.cost || undefined,
        teamAName: this.newMatch.teamAName || undefined,
        teamBName: this.newMatch.teamBName || undefined,
      };

      const createdMatch = await this.matchService.createNewMatch(
        createMatchRequest
      );
      this.matchSuccessMessage = 'Match created successfully!';

      await this.loadMatches();

      this.newMatch = {
        matchDate: this.getDefaultDateTime(),
        location: '',
        cost: null,
        teamAName: '',
        teamBName: '',
      };

      setTimeout(() => {
        this.showCreateMatchModal = false;
        this.matchSuccessMessage = '';
      }, 1500);
    } catch (error: any) {
      this.matchErrorMessage = error.message || 'Error creating match';
      console.error('Error creating match:', error);
    } finally {
      this.matchLoading = false;
    }
  }

  getStatusText(status: number): string {
    switch (status) {
      case 1:
        return 'Open';
      case 2:
        return 'Closed';
      case 4:
        return 'Finalized';
      case 8:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case 1:
        return 'status-open';
      case 2:
        return 'status-closed';
      case 4:
        return 'status-finalized';
      case 8:
        return 'status-cancelled';
      default:
        return 'status-unknown';
    }
  }

  async openAddPlayersModal(match: MatchDisplay) {
    this.selectedMatch = match;
    this.addPlayersErrorMessage = '';
    this.addPlayersSuccessMessage = '';

    try {
      this.matchDetails = await this.matchService.getMatchDetails(match.id);
      console.log('Match details received:', this.matchDetails);

      this.originalTeamAPlayers = this.matchDetails.teams[0]?.players || [];
      this.originalTeamBPlayers = this.matchDetails.teams[1]?.players || [];

      console.log('Team A players:', this.originalTeamAPlayers);
      console.log('Team B players:', this.originalTeamBPlayers);

      this.teamAPlayers = [...this.originalTeamAPlayers];
      this.teamBPlayers = [...this.originalTeamBPlayers];

      this.showAddPlayersModal = true;
    } catch (error: any) {
      this.addPlayersErrorMessage =
        error.message || 'Error loading match details';
      console.error('Error loading match details:', error);
    }
  }

  async addPlayerToTeam(player: User, team: 'teamA' | 'teamB') {
    if (!this.selectedMatch || !this.matchDetails || !player.id) {
      return;
    }

    const isInTeamA = this.teamAPlayers.some((p) => p.id === player.id);
    const isInTeamB = this.teamBPlayers.some((p) => p.id === player.id);

    if (isInTeamA || isInTeamB) {
      this.addPlayersErrorMessage = 'Player is already added to a team';
      setTimeout(() => (this.addPlayersErrorMessage = ''), 3000);
      return;
    }

    const targetTeam = team === 'teamA' ? this.teamAPlayers : this.teamBPlayers;
    if (targetTeam.length >= 6) {
      this.addPlayersErrorMessage = `${
        team === 'teamA'
          ? this.selectedMatch.teamAName || 'TeamA'
          : this.selectedMatch.teamBName || 'TeamB'
      } is full (max 6 players)`;
      setTimeout(() => (this.addPlayersErrorMessage = ''), 3000);
      return;
    }

    this.addingPlayers = true;
    this.addPlayersErrorMessage = '';

    try {
      const teamIndex = team === 'teamA' ? 0 : 1;
      const teamId = this.matchDetails.teams[teamIndex].teamId;
      console.log(
        `Adding player ${player.id} to team ${teamId} in match ${this.selectedMatch.id}`
      );
      await this.matchService.addPlayerToMatch(
        this.selectedMatch.id,
        player.id,
        teamId
      );

      if (team === 'teamA') {
        this.teamAPlayers.push(player);
        this.originalTeamAPlayers.push(player);
      } else {
        this.teamBPlayers.push(player);
        this.originalTeamBPlayers.push(player);
      }

      this.addPlayersSuccessMessage = `${player.firstName} ${
        player.lastName
      } added to ${
        team === 'teamA'
          ? this.selectedMatch.teamAName || 'TeamA'
          : this.selectedMatch.teamBName || 'TeamB'
      }`;
      setTimeout(() => (this.addPlayersSuccessMessage = ''), 2000);
    } catch (error: any) {
      this.addPlayersErrorMessage =
        error.message || 'Error adding player to team';
      console.error('Error adding player:', error);
    } finally {
      this.addingPlayers = false;
    }
  }

  async removePlayerFromTeam(player: User, team: 'teamA' | 'teamB') {
    if (!this.selectedMatch || !player.id) {
      return;
    }

    this.addingPlayers = true;
    this.addPlayersErrorMessage = '';

    try {
      await this.matchService.removePlayerFromMatch(
        this.selectedMatch.id,
        player.id
      );

      if (team === 'teamA') {
        this.teamAPlayers = this.teamAPlayers.filter((p) => p.id !== player.id);
        this.originalTeamAPlayers = this.originalTeamAPlayers.filter(
          (p) => p.id !== player.id
        );
      } else {
        this.teamBPlayers = this.teamBPlayers.filter((p) => p.id !== player.id);
        this.originalTeamBPlayers = this.originalTeamBPlayers.filter(
          (p) => p.id !== player.id
        );
      }

      this.addPlayersSuccessMessage = `${player.firstName} ${
        player.lastName
      } removed from ${
        team === 'teamA'
          ? this.selectedMatch.teamAName || 'TeamA'
          : this.selectedMatch.teamBName || 'TeamB'
      }`;
      setTimeout(() => (this.addPlayersSuccessMessage = ''), 2000);
    } catch (error: any) {
      this.addPlayersErrorMessage =
        error.message || 'Error removing player from team';
      console.error('Error removing player:', error);
    } finally {
      this.addingPlayers = false;
    }
  }

  async movePlayerToOtherTeam(player: User, currentTeam: 'teamA' | 'teamB') {
    if (!this.selectedMatch || !this.matchDetails || !player.id) {
      return;
    }

    const otherTeam = currentTeam === 'teamA' ? 'teamB' : 'teamA';
    const otherTeamPlayers =
      otherTeam === 'teamA' ? this.teamAPlayers : this.teamBPlayers;

    if (otherTeamPlayers.length >= 6) {
      this.addPlayersErrorMessage = `${
        otherTeam === 'teamA'
          ? this.selectedMatch.teamAName || 'TeamA'
          : this.selectedMatch.teamBName || 'TeamB'
      } is full (max 6 players)`;
      setTimeout(() => (this.addPlayersErrorMessage = ''), 3000);
      return;
    }

    this.addingPlayers = true;
    this.addPlayersErrorMessage = '';

    try {
      await this.matchService.removePlayerFromMatch(
        this.selectedMatch.id,
        player.id
      );

      const otherTeamIndex = otherTeam === 'teamA' ? 0 : 1;
      const otherTeamId = this.matchDetails.teams[otherTeamIndex].teamId;
      await this.matchService.addPlayerToMatch(
        this.selectedMatch.id,
        player.id,
        otherTeamId
      );

      if (currentTeam === 'teamA') {
        this.teamAPlayers = this.teamAPlayers.filter((p) => p.id !== player.id);
        this.originalTeamAPlayers = this.originalTeamAPlayers.filter(
          (p) => p.id !== player.id
        );
        this.teamBPlayers.push(player);
        this.originalTeamBPlayers.push(player);
      } else {
        this.teamBPlayers = this.teamBPlayers.filter((p) => p.id !== player.id);
        this.originalTeamBPlayers = this.originalTeamBPlayers.filter(
          (p) => p.id !== player.id
        );
        this.teamAPlayers.push(player);
        this.originalTeamAPlayers.push(player);
      }

      this.addPlayersSuccessMessage = `${player.firstName} ${
        player.lastName
      } moved to ${
        otherTeam === 'teamA'
          ? this.selectedMatch.teamAName || 'TeamA'
          : this.selectedMatch.teamBName || 'TeamB'
      }`;
      setTimeout(() => (this.addPlayersSuccessMessage = ''), 2000);
    } catch (error: any) {
      this.addPlayersErrorMessage =
        error.message || 'Error moving player between teams';
      console.error('Error moving player:', error);
    } finally {
      this.addingPlayers = false;
    }
  }

  closeAddPlayersModal() {
    this.showAddPlayersModal = false;
    this.addPlayersErrorMessage = '';
    this.addPlayersSuccessMessage = '';
  }

  isPlayerInAnyTeam(player: User): boolean {
    return (
      this.teamAPlayers.some((p) => p.id === player.id) ||
      this.teamBPlayers.some((p) => p.id === player.id)
    );
  }

  getRatingClass(rating?: number): string {
    if (!rating) return 'rating-low';
    if (rating >= 8) return 'rating-high';
    if (rating >= 6) return 'rating-medium';
    return 'rating-low';
  }

  getTeamAverageRating(team: User[]): string {
    if (team.length === 0) return '0.0';
    const avg =
      team.reduce((sum, player) => sum + (player.rating || 0), 0) / team.length;
    return avg.toFixed(1);
  }

  getEmptySlots(currentPlayers: number): any[] {
    const emptyCount = Math.max(0, 6 - currentPlayers);
    return new Array(emptyCount).fill(null);
  }

  getDefaultDateTime(): string {
    const now = new Date();
    now.setHours(now.getHours() + 2);
    return now.toISOString().slice(0, 16);
  }

  getMinDateTime(): string {
    const now = new Date();
    return now.toISOString().slice(0, 16);
  }

  async makeMatchPublic(matchId: number) {
    try {
      await this.matchService.publishMatch(matchId);

      const matchIndex = this.matches.findIndex((m) => m.id === matchId);
      if (matchIndex > -1) {
        this.matches[matchIndex].isPublic = true;
      }

      alert('Match made public successfully!');
    } catch (error: any) {
      alert(error.message || 'Error making match public');
      console.error('Error making match public:', error);
    }
  }

  openEditMatchModal(match: MatchDisplay) {
    alert('Edit match functionality coming soon!');
  }
}
