import { Component } from '@angular/core';
import { Header } from '../../components/header/header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.interface';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/user-role.enum';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';
import { StatSelector } from '../../components/stat-selector/stat-selector';
import { MatchService } from '../../services/match.service';
import { NotificationService } from '../../services/notification.service';
import { MatchTemplateService } from '../../services/match-template.service';
import {
  CreateMatchRequest,
  MatchDisplay,
} from '../../models/create-match.interface';
import { MatchStatus } from '../../models/match-status.enum';
import { MatchTemplate, CreateMatchTemplateRequest, UpdateMatchTemplateRequest } from '../../models/match-template.interface';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: true,
  imports: [
    Header,
    FormsModule,
    CommonModule,
    FriendRequestsComponent,
    StatSelector,
  ],
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.css'],
})
export class OrganizerDashboardComponent {
  constructor(
    private UserService: UserService,
    private authService: AuthService,
    private matchService: MatchService,
    private matchTemplateService: MatchTemplateService,
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
  isViewOnlyMode = false;
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
  ratingPreviews: any[] = [];
  addingPlayers = false;
  savingPlayers = false;
  addPlayersErrorMessage = '';
  addPlayersSuccessMessage = '';
  playerLoading = false;
  playerErrorMessage = '';
  playerSuccessMessage = '';
  manualRatings: { [key: number]: number } = {};
  ratingMultiplier: number = 1.0;
  
  // Template variables
  templates: MatchTemplate[] = [];
  showTemplateManagementModal = false;
  showEditTemplateModal = false;
  selectedTemplate: MatchTemplate | null = null;
  newTemplate: CreateMatchTemplateRequest = {
    name: '',
    location: '',
    cost: null,
    teamAName: '',
    teamBName: ''
  };
  editingTemplate: UpdateMatchTemplateRequest = {
    name: '',
    location: '',
    cost: null,
    teamAName: '',
    teamBName: ''
  };

  private async loadAvailablePlayers() {
    const organiserId = this.authService.getUserId()!;

    try {
      const friendPlayers = await this.UserService.getPlayersForOrganiser();
      this.availablePlayers = [...friendPlayers];
    } catch (error) {
      console.error('Error fetching players for organiser:', error);
      this.availablePlayers = [];
    }

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
    console.log('Loading my matches...');
    try {
      const playerMatches = await this.matchService.getPlayerMatches();
      console.log('Fetched player msatches:', playerMatches);
      const myId = this.authService.getUserId();
      const processedMatches = await Promise.all(
        playerMatches.map(async (match: any) => {
          let myTeam: 'A' | 'B' | null = null;

          try {
            const matchDetails = await this.matchService.getMatchDetails(
              match.id
            );

            if (matchDetails.teams && Array.isArray(matchDetails.teams)) {
              if (
                matchDetails.teams[0]?.players?.some((p: any) => {
                  return p.id == myId;
                })
              ) {
                myTeam = 'A';
              } else if (
                matchDetails.teams[1]?.players?.some((p: any) => {
                  return p.id == myId;
                })
              ) {
                myTeam = 'B';
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

          return { ...match, myTeam };
        })
      );

      this.myMatches = processedMatches;
    } catch (error) {
      console.error('Error loading my matches:', error);
    }
  }
  async init() {
    const role = this.authService.getUserRole();
    console.log('User role:', role);

    if (role === UserRole.ADMIN) {
      this.players = await this.UserService.getPlayers();
      this.loadMatches();
      this.LoadMyMatches();
    } else if (role === UserRole.ORGANISER) {
      try {
        this.players = await this.UserService.getPlayersForOrganiser();
      } catch (error) {
        this.players = [];
      }

      this.loadMatches();
      this.LoadMyMatches();
    }
  }

  async loadMatches() {
    try {
      const allMatches = await this.matchService.getMatchesByOrganiser();
      console.log('Loaded matches:', allMatches); // Log matches to inspect team names
      this.matches = await Promise.all(
        allMatches.map(async (match) => {
          let teamAPlayerCount = 0;
          let teamBPlayerCount = 0;

          try {
            const matchDetails = await this.matchService.getMatchDetails(
              match.id
            );
            if (matchDetails.teams && Array.isArray(matchDetails.teams)) {
              teamAPlayerCount = matchDetails.teams[0]?.players?.length || 0;
              teamBPlayerCount = matchDetails.teams[1]?.players?.length || 0;
            }
          } catch (error) {
            console.error(
              'Error fetching match details for match',
              match.id,
              ':',
              error
            );
          }

          return {
            id: match.id,
            matchDate: match.matchDate,
            location: match.location,
            cost: match.cost,
            teamAName: match.teamAName,
            teamBName: match.teamBName,
            status: match.status,
            isPublic: match.isPublic,
            teamAPlayerCount,
            teamBPlayerCount,
          };
        })
      );
    } catch (error) {
      console.error('Error loading matches:', error);
    }
  }

  ngOnInit() {
    this.init();
    this.loadMatches();
    this.loadTemplates();
  }
  
  async loadTemplates() {
    try {
      this.templates = await this.matchTemplateService.getTemplates();
    } catch (error) {
      console.error('Error loading match templates:', error);
      this.notificationService.showError('Error loading match templates');
    }
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

    if (this.newPlayer.rating < 0 || this.newPlayer.rating > 10) {
      this.playerErrorMessage = 'Rating must be between 0 and 10.';
      return;
    }

    this.playerLoading = true;
    this.playerErrorMessage = '';
    this.playerSuccessMessage = '';

    try {
      const addedPlayer = await this.UserService.addPlayer(this.newPlayer);
      // Player relation is now managed through friend requests - no separate relation needed

      this.players.push(addedPlayer);

      this.resetPlayer();
      this.players = await this.UserService.getPlayersForOrganiser();

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

    const selectedDate = new Date(this.newMatch.matchDate);
    const now = new Date();
    if (selectedDate <= now) {
      this.matchErrorMessage =
        'Cannot create a match in the past. Please select a future date and time.';
      return;
    }

    this.matchLoading = true;
    this.matchErrorMessage = '';
    this.matchSuccessMessage = '';

    try {
      const createMatchRequest: CreateMatchRequest = {
        matchDate: new Date(this.newMatch.matchDate).toISOString(),
        status: MatchStatus.Open,
        location: this.newMatch.location,
        cost: this.newMatch.cost || undefined,
        teamAName: this.newMatch.teamAName || undefined,
        teamBName: this.newMatch.teamBName || undefined,
      };
      
      console.log('Creating match with request:', createMatchRequest);

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
      case MatchStatus.Open:
        return 'Open';
      case MatchStatus.Closed:
        return 'Closed';
      case MatchStatus.Finalized:
        return 'Finalized';
      case MatchStatus.Cancelled:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case MatchStatus.Open:
        return 'status-open';
      case MatchStatus.Closed:
        return 'status-closed';
      case MatchStatus.Finalized:
        return 'status-finalized';
      case MatchStatus.Cancelled:
        return 'status-cancelled';
      default:
        return 'status-unknown';
    }
  }
  async openFinalizeMatchModal(match: MatchDisplay) {
    this.selectedMatch = match;

    this.teamAScore = null;
    this.teamBScore = null;
    this.ratingPreviews = [];

    try {
      this.matchDetails = await this.matchService.getMatchDetails(match.id);

      this.originalTeamAPlayers = this.matchDetails.teams[0]?.players || [];
      this.originalTeamBPlayers = this.matchDetails.teams[1]?.players || [];

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
    if (this.teamAScore == null || this.teamBScore == null) {
      this.notificationService.showError('Please enter scores for both teams');
      return;
    }

    // First update preview one last time to ensure we have latest changes
    await this.updateRatingPreview();

    const filteredManualRatings = Object.fromEntries(
      Object.entries(this.manualRatings).filter(
        ([_, value]) => value !== undefined && value !== 0 && value !== null
      )
    );

    const multiplier = this.selectedRatingSystem.startsWith('Custom')
      ? this.ratingMultiplier
      : 1.0;

    try {
      await this.matchService.finalizeMatchServ(
        this.selectedMatch!.id,
        this.teamAScore,
        this.teamBScore,
        this.selectedRatingSystem,
        filteredManualRatings,
        multiplier
      );

      this.notificationService.showSuccess('Match finalized successfully');
      this.closeFinalizeMatchModal();
      await this.loadMatches();
    } catch (error) {
      this.notificationService.showError('Failed to finalize match');
      console.error('Error finalizing match:', error);
    }
  }

  async closeMatch(match: MatchDisplay) {
    try {
      await this.matchService.closeMatch(match.id);
      this.notificationService.showSuccess('Match closed successfully!');
      await this.loadMatches();
    } catch (error: any) {
      this.notificationService.showError(
        error.message || 'Error closing match'
      );
      console.error('Error closing match:', error);
    }
  }

  async cancelMatch(match: MatchDisplay) {
    try {
      await this.matchService.cancelMatch(match.id);
      this.notificationService.showSuccess('Match cancelled successfully!');
      await this.loadMatches();
    } catch (error: any) {
      this.notificationService.showError(
        error.message || 'Error cancelling match'
      );
      console.error('Error cancelling match:', error);
    }
  }

  canShowCloseButton(match: MatchDisplay): boolean {
    return (
      match.status === MatchStatus.Open &&
      (match.teamAPlayerCount || 0) + (match.teamBPlayerCount || 0) >= 10
    );
  }

  canShowFinalizeButton(match: MatchDisplay): boolean {
    const matchDate = new Date(match.matchDate);
    const now = new Date();
    return matchDate < now && match.status === MatchStatus.Closed;
  }

  getAddPlayersButtonText(match: MatchDisplay): string {
    const totalPlayers =
      (match.teamAPlayerCount || 0) + (match.teamBPlayerCount || 0);
    if (
      match.status === MatchStatus.Closed ||
      match.status === MatchStatus.Finalized
    ) {
      return 'View Players';
    }
    return 'Add Players';
  }
  async updateRatingPreview() {
    if (this.teamAScore === null || this.teamBScore === null) {
      return;
    }

    try {
      // Use the same rating system name handling as finalization
      const previewResponse = await this.matchService.calculateRatingPreview(
        this.selectedMatch!.id,
        this.teamAScore,
        this.teamBScore,
        this.selectedRatingSystem,
        this.selectedRatingSystem.startsWith('Custom')
          ? this.ratingMultiplier
          : 1.0
      );

      this.ratingPreviews = previewResponse.map((preview) => {
        const manualAdjustment = this.manualRatings[preview.playerId] || 0;
        let baseRating = parseFloat(preview.ratingChange);
        const totalChange = baseRating + manualAdjustment;

        return {
          ...preview,
          baseRatingChange: preview.ratingChange,
          ratingChange: totalChange.toFixed(2),
        };
      });
    } catch (error) {
      console.error('Error previewing ratings:', error);
      this.notificationService.showError('Failed to preview rating changes');
    }
  }

  getRatingPreviewForPlayer(playerId: number): string {
    const preview = this.ratingPreviews.find((p) => p.playerId === playerId);
    return preview ? preview.ratingChange : '0.0';
  }

  getRatingChangeClass(playerId: number): string {
    const preview = this.ratingPreviews.find((p) => p.playerId === playerId);
    if (!preview) return '';

    const change = parseFloat(preview.ratingChange);
    if (change > 0) return 'positive-rating';
    if (change < 0) return 'negative-rating';
    return 'neutral-rating';
  }

  async openAddPlayersModal(match: MatchDisplay) {
    this.selectedMatch = match;
    this.addPlayersErrorMessage = '';
    this.addPlayersSuccessMessage = '';

    // Determine if this is view-only mode
    this.isViewOnlyMode =
      this.getAddPlayersButtonText(match) === 'View Players';

    try {
      await this.loadAvailablePlayers();
      this.matchDetails = await this.matchService.getMatchDetails(match.id);

      this.originalTeamAPlayers = this.matchDetails.teams[0]?.players || [];
      this.originalTeamBPlayers = this.matchDetails.teams[1]?.players || [];

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

      await this.loadMatches();
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

      await this.loadMatches();
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

      await this.loadMatches();
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
  getTeamAverageRating(team: User[]): number {
    if (team.length === 0) return 0;
    return (
      team.reduce((sum, player) => sum + (player.rating || 0), 0) / team.length
    );
  }

  getTeamAverageRatingDisplay(team: User[]): string {
    return this.getTeamAverageRating(team).toFixed(1);
  }

  getTeamRatingClass(rating: number): string {
    if (rating >= 8) return 'rating-high';
    if (rating >= 6) return 'rating-medium';
    return 'rating-low';
  }
  getRatingClass(rating?: number): string {
    if (!rating) return 'rating-low';
    if (rating >= 8) return 'rating-high';
    if (rating >= 6) return 'rating-medium';
    return 'rating-low';
  }

  // getTeamAverageRating(team: User[]): string {
  //   if (team.length === 0) return '0.0';
  //   const avg =
  //     team.reduce((sum, player) => sum + (player.rating || 0), 0) / team.length;
  //   return avg.toFixed(1);
  // }

  getEmptySlots(currentPlayers: number): any[] {
    const emptyCount = Math.max(0, 6 - currentPlayers);
    return new Array(emptyCount).fill(null);
  }

  getDefaultDateTime(): string {
    const now = new Date();

    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  getMinDateTime(): string {
    const now = new Date();

    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
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
    const matchDate = new Date(match.matchDate);

    const year = matchDate.getFullYear();
    const month = String(matchDate.getMonth() + 1).padStart(2, '0');
    const day = String(matchDate.getDate()).padStart(2, '0');
    const hours = String(matchDate.getHours()).padStart(2, '0');
    const minutes = String(matchDate.getMinutes()).padStart(2, '0');
    const formattedDate = `${year}-${month}-${day}T${hours}:${minutes}`;

    this.editMatch = {
      matchDate: formattedDate,
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

    const selectedDate = new Date(this.editMatch.matchDate);
    const now = new Date();
    if (selectedDate <= now) {
      this.editMatchErrorMessage =
        'Cannot update match to a past date. Please select a future date and time.';
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
          teamAName: updatedMatch.teamAName,
          teamBName: updatedMatch.teamBName,
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

  updateManualRating(playerId: number) {
    const manualRating = this.manualRatings[playerId];
    if (manualRating !== undefined) {
      // Validate the input
      if (manualRating < -10 || manualRating > 10) {
        this.notificationService.showError(
          'Manual rating adjustment must be between -10 and +10'
        );
        return;
      }

      // Update the preview by adding manual adjustment to base rating
      this.ratingPreviews = this.ratingPreviews.map((preview) => {
        if (preview.playerId === playerId) {
          const baseChange = parseFloat(preview.baseRatingChange);
          const totalChange = baseChange + manualRating;
          return {
            ...preview,
            ratingChange: totalChange.toFixed(1),
          };
        }
        return preview;
      });
    }
  }

  async shuffleTeams() {
    if (!this.selectedMatch || this.selectedMatch.status !== 0) {
      this.notificationService.showError(
        'Teams can only be shuffled when the match is Open'
      );
      return;
    }

    const combinedPlayers = [...this.teamAPlayers, ...this.teamBPlayers];
    for (let i = combinedPlayers.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [combinedPlayers[i], combinedPlayers[j]] = [
        combinedPlayers[j],
        combinedPlayers[i],
      ];
    }

    // Split players into two teams
    const halfLength = Math.floor(combinedPlayers.length / 2);
    const newTeamAPlayers = combinedPlayers.slice(0, halfLength);
    const newTeamBPlayers = combinedPlayers.slice(halfLength);

    try {
      this.addingPlayers = true;

      // Remove all players from both teams first
      for (const player of this.teamAPlayers) {
        await this.removePlayerFromTeam(player, 'teamA');
      }
      for (const player of this.teamBPlayers) {
        await this.removePlayerFromTeam(player, 'teamB');
      }

      // Add players to their new teams
      for (const player of newTeamAPlayers) {
        await this.addPlayerToTeam(player, 'teamA');
      }
      for (const player of newTeamBPlayers) {
        await this.addPlayerToTeam(player, 'teamB');
      }

      this.teamAPlayers = newTeamAPlayers;
      this.teamBPlayers = newTeamBPlayers;

      this.notificationService.showSuccess(
        'Teams shuffled and saved successfully'
      );
    } catch (error) {
      this.notificationService.showError('Failed to save shuffled teams');
      console.error('Error saving shuffled teams:', error);

      // Revert to original teams if there's an error
      this.teamAPlayers = [...this.originalTeamAPlayers];
      this.teamBPlayers = [...this.originalTeamBPlayers];
    } finally {
      this.addingPlayers = false;
    }
  }
  balanceTeams(mode: string = 'rating') {
    if (!this.selectedMatch || this.selectedMatch.status !== 0) {
      this.notificationService.showError(
        'Teams can only be balanced when the match is Open'
      );
      return;
    }

    const combinedPlayers = [...this.teamAPlayers, ...this.teamBPlayers];

    if (mode === 'random') {
      // Random shuffle
      for (let i = combinedPlayers.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [combinedPlayers[i], combinedPlayers[j]] = [
          combinedPlayers[j],
          combinedPlayers[i],
        ];
      }
    } else {
      // Sort by rating (highest to lowest)
      combinedPlayers.sort((a, b) => (b.rating || 0) - (a.rating || 0));

      const tempTeamA: User[] = [];
      const tempTeamB: User[] = [];

      // Distribute players alternately to balance teams
      combinedPlayers.forEach((player, index) => {
        if (index % 2 === 0) {
          tempTeamA.push(player);
        } else {
          tempTeamB.push(player);
        }
      });

      // Calculate team ratings
      const teamARating =
        tempTeamA.reduce((sum, player) => sum + (player.rating || 0), 0) /
        tempTeamA.length;
      const teamBRating =
        tempTeamB.reduce((sum, player) => sum + (player.rating || 0), 0) /
        tempTeamB.length;

      // If ratings are too unbalanced, try swapping some players
      if (Math.abs(teamARating - teamBRating) > 1) {
        for (let i = 0; i < tempTeamA.length; i++) {
          for (let j = 0; j < tempTeamB.length; j++) {
            const newTeamARating =
              (teamARating * tempTeamA.length -
                (tempTeamA[i].rating || 0) +
                (tempTeamB[j].rating || 0)) /
              tempTeamA.length;
            const newTeamBRating =
              (teamBRating * tempTeamB.length -
                (tempTeamB[j].rating || 0) +
                (tempTeamA[i].rating || 0)) /
              tempTeamB.length;

            if (
              Math.abs(newTeamARating - newTeamBRating) <
              Math.abs(teamARating - teamBRating)
            ) {
              // Swap players
              const temp = tempTeamA[i];
              tempTeamA[i] = tempTeamB[j];
              tempTeamB[j] = temp;
              break;
            }
          }
        }
      }

      combinedPlayers.splice(0); // Clear array
      combinedPlayers.push(...tempTeamA, ...tempTeamB);
    }

    const halfLength = Math.floor(combinedPlayers.length / 2);
    this.teamAPlayers = combinedPlayers.slice(0, halfLength);
    this.teamBPlayers = combinedPlayers.slice(halfLength);
  }

  calculatePricePerPlayer(
    totalCost: number | undefined,
    playerCount: number
  ): number {
    if (!totalCost || totalCost <= 0) return 0;
    return Math.round((totalCost / playerCount) * 100) / 100;
  }

  getPriceInfo(match: MatchDisplay | any): {
    for10: number;
    for11: number;
    for12: number;
    exactPrice?: number;
    totalPlayers?: number;
  } {
    const totalCost = match.cost || 0;

    if (match.status === 1) {
      const totalPlayers = match.playerHistory?.length || 0;
      if (totalPlayers > 0) {
        return {
          for10: this.calculatePricePerPlayer(totalCost, 10),
          for11: this.calculatePricePerPlayer(totalCost, 11),
          for12: this.calculatePricePerPlayer(totalCost, 12),
          exactPrice: this.calculatePricePerPlayer(totalCost, totalPlayers),
          totalPlayers,
        };
      }
    }

    return {
      for10: this.calculatePricePerPlayer(totalCost, 10),
      for11: this.calculatePricePerPlayer(totalCost, 11),
      for12: this.calculatePricePerPlayer(totalCost, 12),
    };
  }

  // Template Methods
  async openTemplateManagementModal() {
    await this.loadTemplates();
    this.showTemplateManagementModal = true;
  }

  closeTemplateManagementModal() {
    this.showTemplateManagementModal = false;
  }

  async createTemplate() {
    if (!this.newTemplate.location) {
      this.notificationService.showError('Template location is required');
      return;
    }

    if (!this.newTemplate.name) {
      this.notificationService.showError('Template name is required');
      return;
    }

    try {
      await this.matchTemplateService.createTemplate(this.newTemplate);
      await this.loadTemplates();
      this.resetNewTemplate();
      this.notificationService.showSuccess('Template created successfully');
    } catch (error) {
      console.error('Error creating template:', error);
      this.notificationService.showError('Failed to create template');
    }
  }

  resetNewTemplate() {
    this.newTemplate = {
      name: '',
      location: '',
      cost: null,
      teamAName: '',
      teamBName: ''
    };
  }

  openEditTemplateModal(template: MatchTemplate) {
    this.selectedTemplate = template;
    this.editingTemplate = {
      name: template.name,
      location: template.location,
      cost: template.cost,
      teamAName: template.teamAName || '',
      teamBName: template.teamBName || ''
    };
    this.showEditTemplateModal = true;
  }

  closeEditTemplateModal() {
    this.showEditTemplateModal = false;
    this.selectedTemplate = null;
  }

  async updateTemplate() {
    if (!this.selectedTemplate || !this.selectedTemplate.id) {
      return;
    }

    if (!this.editingTemplate.location) {
      this.notificationService.showError('Template location is required');
      return;
    }

    if (!this.editingTemplate.name) {
      this.notificationService.showError('Template name is required');
      return;
    }

    try {
      await this.matchTemplateService.updateTemplate(
        this.selectedTemplate.id,
        this.editingTemplate
      );
      await this.loadTemplates();
      this.closeEditTemplateModal();
      this.notificationService.showSuccess('Template updated successfully');
    } catch (error) {
      console.error('Error updating template:', error);
      this.notificationService.showError('Failed to update template');
    }
  }

  async deleteTemplate(templateId: number) {
    const confirmed = confirm('Are you sure you want to delete this template?');
    if (!confirmed) {
      return;
    }

    try {
      await this.matchTemplateService.deleteTemplate(templateId);
      await this.loadTemplates();
      this.notificationService.showSuccess('Template deleted successfully');
    } catch (error) {
      console.error('Error deleting template:', error);
      this.notificationService.showError('Failed to delete template');
    }
  }

  applyTemplate(template: MatchTemplate) {
    console.log('Applying template:', template); // Log the template being applied
    
    this.newMatch.location = template.location;
    this.newMatch.cost = template.cost;
    
    // Apply team names if they exist in the template
    if (template.teamAName) {
      console.log('Setting teamAName:', template.teamAName);
      this.newMatch.teamAName = template.teamAName;
    }
    
    if (template.teamBName) {
      console.log('Setting teamBName:', template.teamBName);
      this.newMatch.teamBName = template.teamBName;
    }
    
    console.log('New match after applying template:', this.newMatch);
    this.notificationService.showSuccess('Template applied successfully');
  }
}
