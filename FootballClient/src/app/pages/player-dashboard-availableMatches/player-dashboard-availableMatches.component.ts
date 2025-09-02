import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatchService } from '../../services/match.service';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { PlayerHeaderComponent } from '../../components/player-header/player-header.component';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';
import { Match } from '../../models/match.interface';
import { User } from '../../models/user.interface';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-player-dashboard-availableMatches.component',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DatePipe,
    PlayerHeaderComponent,
    FriendRequestsComponent,
  ],
  templateUrl: './player-dashboard-availableMatches.component.html',
  styleUrls: ['./player-dashboard-availableMatches.component.css'],
})
export class PlayerDashboardAvailableMatchesComponent implements OnInit {
  activeTab: string = 'future';
  currentPlayer: User | null = null;

  upcomingMatches: Match[] = [];
  availableMatches: Match[] = [];

  modalOpen: boolean = false;
  modalType: 'view' | 'join' = 'view';
  selectedMatch: Match | null = null;
  selectedTeamAName: string = '';
  selectedTeamBName: string = '';
  selectedTeamAPlayers: string[] = [];
  selectedTeamBPlayers: string[] = [];

  constructor(
    private authService: AuthService,
    private matchService: MatchService,
    private userService: UserService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  async ngOnInit() {
    await this.loadPublicMatches();
  }

  async loadAvailableMatches() {
    try {
      const availableMatches = await this.matchService.getAvailableMatches();
      this.availableMatches = availableMatches;

      for (const match of this.availableMatches) {
        match.teamAName = await this.matchService.getTeamById(match.teamAId);
        match.teamBName = await this.matchService.getTeamById(match.teamBId);
      }
    } catch (error) {
      console.error('Error loading available matches:', error);
    }
  }

  async loadPublicMatches() {
    try {
      const publicMatches = await this.matchService.getPublicMatches();
      const now = new Date();
      this.availableMatches = publicMatches.filter(
        (match: any) =>
          new Date(match.matchDate) > new Date() &&
          !this.upcomingMatches.some((up) => up.id === match.id)
      );
    } catch (error) {
      console.error('Error loading public matches:', error);
    }
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  async openJoinMatchModal(match: Match) {
    try {
      this.modalType = 'join';
      await this.loadMatchDetails(match);
      this.modalOpen = true;
    } catch (error) {
      console.error('Error loading match details:', error);
    }
  }

  async loadMatchDetails(match: Match) {
    this.selectedMatch = match;
    this.selectedTeamAName = match.teamAName!;

    this.selectedTeamBName = match.teamBName!;

    try {
      const matchDetails = await this.matchService.getMatchDetails(match.id);

      const teamA = matchDetails.teams.find(
        (t: any) => t.teamId === match.teamAId
      );
      const teamB = matchDetails.teams.find(
        (t: any) => t.teamId === match.teamBId
      );

      this.selectedTeamAPlayers = teamA
        ? teamA.players.map((p: any) => `${p.username} - ${p.rating}`)
        : [];

      this.selectedTeamBPlayers = teamB
        ? teamB.players.map((p: any) => `${p.username} - ${p.rating}`)
        : [];
    } catch (error) {
      this.selectedTeamAPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamAId && p.user)
        .map((p) => `${p.user.username}- ${p.user.rating}`);

      this.selectedTeamBPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamBId && p.user)
        .map((p) => `${p.user.username} - ${p.user.rating}`);
    }
  }

  closeModal() {
    this.modalOpen = false;
    this.selectedMatch = null;
  }

  getPlayerTeamName(match: Match): string {
    const playerHistory = match.playerHistory?.find(
      (ph) => ph.user && ph.user.id === this.currentPlayer?.id
    );
    if (!playerHistory) return '';

    return playerHistory.teamId === match.teamAId
      ? match.teamAName || 'Team A'
      : match.teamBName || 'Team B';
  }

  canLeaveMatch(match: Match): boolean {
    const playerHistory = match.playerHistory?.find(
      (ph) => ph.user && ph.user.id === this.currentPlayer?.id
    );
    return playerHistory?.status === 2;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goToAccount() {
    this.router.navigate(['/player-account']);
  }

  async joinMatch(match: Match) {
    try {
      if (!match.id) {
        console.error('Match ID is missing');
        return;
      }
      await this.matchService.joinMatch(match.id);
      await this.loadAvailableMatches();
    } catch (error) {
      console.error('Error joining match:', error);
      this.notificationService.showError(
        'Failed to join match. Please try again.'
      );
    }
  }

  async joinTeam(teamId: number) {
    try {
      if (!this.selectedMatch?.id) {
        console.error('Match ID is missing');
        return;
      }

      if (teamId === undefined || teamId === null) {
        await this.matchService.joinMatch(this.selectedMatch.id);
      } else {
        await this.matchService.joinTeam(this.selectedMatch.id, teamId);
      }

      this.closeModal();
      await this.loadAvailableMatches();

      this.notificationService.showSuccess('Successfully joined the match!');
    } catch (error) {
      console.error('Error joining match:', error);
      this.notificationService.showError(
        'Failed to join match. Please try again.'
      );
    }
  }

  async leaveMatch(match: Match) {
    try {
      if (!match.id) {
        console.error('Match ID is missing');
        return;
      }

      if (!this.canLeaveMatch(match)) {
        this.notificationService.showWarning(
          'You cannot leave this match because you were added by the organizer.'
        );
        return;
      }

      if (confirm('Are you sure you want to leave this match?')) {
        await this.matchService.leaveMatch(match.id);
        await this.loadAvailableMatches();
        this.notificationService.showSuccess('Successfully left the match!');
      }
    } catch (error) {
      console.error('Error leaving match:', error);
      this.notificationService.showError(
        'Failed to leave match. Please try again.'
      );
    }
  }
}
