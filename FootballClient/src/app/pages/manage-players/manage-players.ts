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
import {
  DelegationStatusDto,
  OrganizerDelegateDto,
} from '../../models/organizer-delegation.interface';
import { Router } from '@angular/router';
import { FriendRequestService } from '../../services/friend-request.service';

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
    private matchService: MatchService,
    private router: Router,
    private friendRequestService: FriendRequestService 
  ) {}

  players: User[] = [];
  availablePlayers: User[] = [];
  filteredPlayers: User[] = [];
  activeTab: 'players' | 'matches' | 'myMatches' = 'matches';
  selectedMatch: MatchDisplay | null = null;
  playerLoading = false;
  playerErrorMessage = '';
  playerSuccessMessage = '';

  editIndex: number | null = null;
  editedPlayer: User | null = null;

  searchTerm: string = '';

  showAddModal = false;
  showDelegateModal = false;

  delegationStatus: DelegationStatusDto | null = null;
  delegationLoading = false;
  delegationErrorMessage = '';
  delegationSuccessMessage = '';
  selectedPlayerForDelegation: User | null = null;
  delegationNotes = '';

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

    await this.loadDelegationStatus();
  }

  async loadDelegationStatus() {
    try {
      const userId = this.authService.getUserId()!;
      this.delegationStatus = await this.UserService.getDelegationStatus(
        userId
      );
    } catch (error) {
      console.error('Error loading delegation status:', error);
    }
  }

  ngOnInit() {
    this.init();
  }

  isPlayerEnabled(player: User): boolean {
    return !player.isDeleted;
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
    } catch (error) {
      console.error('Error updating player:', error);
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
      alert('Failed to reactivate player. Please try again.');
    }
  }

  clearEditIndex() {
    this.editIndex = null;
    this.editedPlayer = null;
  }

  openDelegateModal(player: User) {
    this.selectedPlayerForDelegation = player;
    this.showDelegateModal = true;
    this.delegationErrorMessage = '';
    this.delegationSuccessMessage = '';
    this.delegationNotes = '';
  }

  closeDelegateModal() {
    this.showDelegateModal = false;
    this.selectedPlayerForDelegation = null;
    this.delegationNotes = '';
    this.delegationErrorMessage = '';
    this.delegationSuccessMessage = '';
  }

  async delegateToPlayer() {
    if (!this.selectedPlayerForDelegation) return;

    this.delegationLoading = true;
    this.delegationErrorMessage = '';
    this.delegationSuccessMessage = '';

    try {
      const userId = this.authService.getUserId()!;
      await this.UserService.delegateOrganizerRole(
        userId,
        this.selectedPlayerForDelegation.id!,
        this.delegationNotes || undefined
      );

      this.delegationSuccessMessage = `Successfully delegated organizer role to ${this.selectedPlayerForDelegation.firstName} ${this.selectedPlayerForDelegation.lastName}. Redirecting...`;

      await this.loadDelegationStatus();

      setTimeout(() => {
        this.closeDelegateModal();
        this.router.navigate(['/delegated-organizer']);
      }, 1500);
    } catch (error: any) {
      this.delegationErrorMessage =
        error.message || 'Failed to delegate organizer role';
      console.error('Error delegating organizer role:', error);
    } finally {
      this.delegationLoading = false;
    }
  }

  async reclaimOrganizerRole() {
    if (!this.delegationStatus?.currentDelegation) return;

    const confirm = window.confirm(
      'Are you sure you want to reclaim your organizer role?'
    );
    if (!confirm) return;

    this.delegationLoading = true;
    this.delegationErrorMessage = '';

    try {
      const userId = this.authService.getUserId()!;
      await this.UserService.reclaimOrganizerRole(
        userId,
        this.delegationStatus.currentDelegation.id
      );

      await this.loadDelegationStatus();
    } catch (error: any) {
      this.delegationErrorMessage =
        error.message || 'Failed to reclaim organizer role';
      console.error('Error reclaiming organizer role:', error);
    } finally {
      this.delegationLoading = false;
    }
  }

  canDelegateToPlayer(player: User): boolean {
    return (
      this.isPlayerEnabled(player) &&
      !this.delegationStatus?.isDelegating &&
      !this.delegationStatus?.isDelegate &&
      player.role !== UserRole.ORGANISER
    );
  }

  async unfriend(player: User) {
  const confirmUnfriend = confirm(
    `Are you sure you want to unfriend ${player.firstName} ${player.lastName}?`
  );
  if (!confirmUnfriend) return;

  try {
    await this.friendRequestService.unfriend(player.id!);
    this.players = this.players.filter((p) => p.id !== player.id); // scoatem din listă
    alert(`${player.firstName} ${player.lastName} has been unfriended.`);
  } catch (error: any) {
    console.error('Error unfriending:', error);
    alert(error.message || 'Failed to unfriend user');
  }
}

}
