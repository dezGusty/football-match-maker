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
import { NotificationService } from '../../services/notification.service';
import {
  CreateMatchRequest,
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
    private matchService: MatchService,
    private notificationService: NotificationService
  ) {}

  players: User[] = [];
  availablePlayers: User[] = [];
  filteredPlayers: User[] = [];
  matches: MatchDisplay[] = [];
  myMatches: MatchDisplay[] = [];
  searchTerm: string = '';
  editIndex: number | null = null;
  editedPlayer: User | null = null;
  showAddModal = false;
  showCreateMatchModal = false;
  showEditMatchModal = false;
  showAddPlayersModal = false;
  showFinalizeMatchModal = false;
  activeTab: 'players' | 'matches' | 'myMatches' = 'matches';
  selectedMatch: MatchDisplay | null = null;
  selectedRatingSystem: string = 'Linear'; // Default rating system
  matchDetails: any = null; // Will contain team IDs
  teamAPlayers: User[] = [];
  teamBPlayers: User[] = [];
  teamAScore: number | null = null;
  teamBScore: number | null = null;
  originalTeamAPlayers: User[] = [];
  originalTeamBPlayers: User[] = [];
  addingPlayers = false;
  savingPlayers = false;
  addPlayersErrorMessage = '';
  addPlayersSuccessMessage = '';
  playerLoading = false;
  playerErrorMessage = '';
  playerSuccessMessage = '';

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

  private async LoadMyMatches() {
    try {
      const playerMatches = await this.matchService.getPlayerMatches();
      const myId = this.authService.getUserId();
      console.log('My ID:', myId);
      console.log('Player matches:', playerMatches);
      const processedMatches = await Promise.all(
        playerMatches.map(async (match: any) => {
          let myTeam: 'A' | 'B' | null = null;
          console.log('Processing match:', match.id);

          try {
            const matchDetails = await this.matchService.getMatchDetails(
              match.id
            );
            console.log('Match details for', match.id, ':', matchDetails);

            if (matchDetails.teams && Array.isArray(matchDetails.teams)) {
              if (
                matchDetails.teams[0]?.players?.some((p: any) => {
                  console.log(
                    'Checking Team A player:',
                    p.id,
                    'vs my ID:',
                    myId,
                    'Match:',
                    p.id == myId
                  );
                  return p.id == myId;
                })
              ) {
                myTeam = 'A';
                console.log('Found in Team A');
              } else if (
                matchDetails.teams[1]?.players?.some((p: any) => {
                  console.log(
                    'Checking Team B player:',
                    p.id,
                    'vs my ID:',
                    myId,
                    'Match:',
                    p.id == myId
                  );
                  return p.id == myId;
                })
              ) {
                myTeam = 'B';
                console.log('Found in Team B');
              }
            }
          } catch (detailsError) {
            console.error(
              'Error fetching match details for match',
              match.id,
              ':',
              detailsError
            );

            myTeam = null;
          }

          console.log('Final myTeam for match', match.id, ':', myTeam);
          return { ...match, myTeam };
        })
      );

      this.myMatches = processedMatches;
      console.log('Final myMatches:', this.myMatches);
    } catch (error) {
      console.error('Error loading my matches:', error);
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
      this.loadMatches();
      this.LoadMyMatches();
    }
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
    this.loadMatches();
  }

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

      this.resetPlayer();
      this.players = await this.UserService.getPlayersForOrganiser(
        this.authService.getUserId()!
      );

      this.showAddModal = false;
      this.notificationService.showSuccess(
        `Player ${this.newPlayer.firstName} ${this.newPlayer.lastName} added successfully!`
      );
    } catch (error: any) {
      this.playerErrorMessage =
        error.message || 'Failed to add player. Please try again.';
      this.notificationService.showError(
        error.message || 'Failed to add player. Please try again.'
      );
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
      this.notificationService.showError(
        'Failed to delete player. Please try again.'
      );
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
      this.notificationService.showError(
        'Failed to reactivate player. Please try again.'
      );
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

  editMatch = {
    matchDate: '',
    location: '',
    cost: null as number | null,
    teamAName: '',
    teamBName: '',
  };

  editMatchLoading = false;
  editMatchErrorMessage = '';
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

      await this.loadMatches();

      this.newMatch = {
        matchDate: this.getDefaultDateTime(),
        location: '',
        cost: null,
        teamAName: '',
        teamBName: '',
      };

      this.showCreateMatchModal = false;
      this.notificationService.showSuccess('Match created successfully!');
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
  async openFinalizeMatchModal(match: MatchDisplay) {
    this.selectedMatch = match;

    try {
      this.matchDetails = await this.matchService.getMatchDetails(match.id);
      console.log('Match details received:', this.matchDetails);

      this.originalTeamAPlayers = this.matchDetails.teams[0]?.players || [];
      this.originalTeamBPlayers = this.matchDetails.teams[1]?.players || [];

      console.log('Team A players:', this.originalTeamAPlayers);
      console.log('Team B players:', this.originalTeamBPlayers);

      this.teamAPlayers = [...this.originalTeamAPlayers];
      this.teamBPlayers = [...this.originalTeamBPlayers];


      this.showFinalizeMatchModal = true;
    } catch (error) {
      console.error('Error loading match details for finalize modal:', error);
    }
  }
  async closeFinalizeMatchModal() {
    this.showFinalizeMatchModal = false;
  }
  async finalizeMatch() {
    // await this.matchService.updateMatch(this.selectedMatch?.id);
    // console.log('Match finalized:', this.selectedMatch?.id);
    
  }
  ratingChange(player?: User): string {
    if(this.selectedRatingSystem == "Performance"){
      let playerErrorsChange = 0;
      switch (player?.errors) {
        case 1:
          playerErrorsChange = 0.025;
          break;
        case 2:
          playerErrorsChange = 0.05;
          break;
        case 3:
          playerErrorsChange = 0.075;
          break;
        case 4:
          playerErrorsChange = 0.1;
          break;
        default:
          playerErrorsChange = 0;
          break;
      }
      const totalRating =
        (player?.rating ?? 0) * 0.005 +
        (player?.speed ?? 0) * 0.025 +
        (player?.stamina ?? 0) * 0.025 +
        playerErrorsChange;
      
      if(this.teamAScore != null && this.teamBScore != null){
        if(this.teamAScore > this.teamBScore){
          if(this.teamAPlayers.some(p => p.id === player?.id)){
            return "+" + totalRating.toFixed(2);
          } else if(this.teamBPlayers.some(p => p.id === player?.id)){
            return "-" + totalRating.toFixed(2);
          }
        }
        else if(this.teamAScore < this.teamBScore){
          if(this.teamAPlayers.some(p => p.id === player?.id)){
            return "-" + totalRating.toFixed(2);
          } else if(this.teamBPlayers.some(p => p.id === player?.id)){
            return "+" + totalRating.toFixed(2);
          }
        }
        else {
          return "+" + (totalRating/2).toFixed(2);
        }
      }
      return "0"
    }
    else if(this.selectedRatingSystem == "Linear"){
      if(this.teamAScore != null && this.teamBScore != null){
        const scoreDiff = Math.abs(this.teamAScore - this.teamBScore);
        const ratingChange = Math.min(0.1 * scoreDiff, 1); // Max change capped at 1 point

        if(this.teamAScore > this.teamBScore){
          if(this.teamAPlayers.some(p => p.id === player?.id)){
            return "+" + ratingChange.toFixed(2);
          } else if(this.teamBPlayers.some(p => p.id === player?.id)){
            return "-" + ratingChange.toFixed(2);
          }
        }
        else if(this.teamAScore < this.teamBScore){
          if(this.teamAPlayers.some(p => p.id === player?.id)){
            return "-" + ratingChange.toFixed(2);
          } else if(this.teamBPlayers.some(p => p.id === player?.id)){
            return "+" + ratingChange.toFixed(2);
          }
        }
        else {
          return "0.00";
        }
      }
      return "0.00"
    }
    else{
      return "0.00"
    }
    
  }

  async openAddPlayersModal(match: MatchDisplay) {
    this.selectedMatch = match;
    this.addPlayersErrorMessage = '';
    this.addPlayersSuccessMessage = '';

    try {
      await this.loadAvailablePlayers();
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

      this.notificationService.showSuccess('Match made public successfully!');
    } catch (error: any) {
      this.notificationService.showError(
        error.message || 'Error making match public'
      );
      console.error('Error making match public:', error);
    }
  }

  async makeMatchPrivate(matchId: number) {
    try {
      await this.matchService.makeMatchPrivate(matchId);

      const matchIndex = this.matches.findIndex((m) => m.id === matchId);
      if (matchIndex > -1) {
        this.matches[matchIndex].isPublic = false;
      }

      this.notificationService.showSuccess('Match made private successfully!');
    } catch (error: any) {
      this.notificationService.showError(
        error.message || 'Error making match private'
      );
      console.error('Error making match private:', error);
    }
  }

  openEditMatchModal(match: MatchDisplay) {
    this.editMatch = {
      matchDate: new Date(match.matchDate).toISOString().slice(0, 16),
      location: match.location || '',
      cost: match.cost || null,
      teamAName: match.teamAName || '',
      teamBName: match.teamBName || '',
    };

    this.editMatchErrorMessage = '';
    this.selectedMatch = match;
    this.showEditMatchModal = true;
  }

  async updateMatch() {
    if (!this.editMatch.matchDate || !this.editMatch.location) {
      this.editMatchErrorMessage = 'Match date and location are required';
      return;
    }

    if (!this.selectedMatch) {
      this.editMatchErrorMessage = 'No match selected for editing';
      return;
    }

    this.editMatchLoading = true;
    this.editMatchErrorMessage = '';

    try {
      const updateMatchRequest = {
        matchDate: new Date(this.editMatch.matchDate).toISOString(),
        location: this.editMatch.location,
        cost: this.editMatch.cost || undefined,
        teamAName: this.editMatch.teamAName || undefined,
        teamBName: this.editMatch.teamBName || undefined,
      };

      const updatedMatch = await this.matchService.updateMatchPlayer(
        this.selectedMatch.id,
        updateMatchRequest
      );

      const matchIndex = this.matches.findIndex(
        (m) => m.id === this.selectedMatch!.id
      );
      if (matchIndex > -1) {
        this.matches[matchIndex] = {
          ...this.matches[matchIndex],
          matchDate: updatedMatch.matchDate,
          location: updatedMatch.location,
          cost: updatedMatch.cost,
        };
      }

      this.showEditMatchModal = false;
      this.notificationService.showSuccess('Match updated successfully!');
    } catch (error: any) {
      this.editMatchErrorMessage = error.message || 'Error updating match';
      this.notificationService.showError(
        error.message || 'Failed to update match. Please try again.'
      );
      console.error('Error updating match:', error);
    } finally {
      this.editMatchLoading = false;
    }
  }

}
