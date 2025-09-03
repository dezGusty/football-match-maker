import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatchService } from '../../services/match.service';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { Header } from '../../components/header/header';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';
import { Match } from '../../models/match.interface';
import { User } from '../../models/user.interface';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-player-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DatePipe,
    Header,
    FriendRequestsComponent,
  ],
  templateUrl: './player-dashboard.component.html',
  styleUrls: ['./player-dashboard.component.css'],
})
export class PlayerDashboardComponent implements OnInit {
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
    await this.loadPlayerData();

    await this.loadMatches();
    await this.loadAvailableMatches();
    await this.loadPublicMatches();
    await this.loadPlayerSpecificMatches();
  }

  async loadPlayerData() {
    const userId = this.authService.getUserId();

    if (userId) {
      try {
        const userResponse = await fetch(
          `${environment.apiUrl}/user/${userId}`
        );
        if (userResponse.ok) {
          const user = await userResponse.json();
          const players = await this.userService.getPlayers();
          this.currentPlayer =
            players.find((p) => p.email === user.email) || null;
        } else {
          console.error('Failed to fetch user data:', userResponse.status);
        }
      } catch (error) {
        console.error('Error loading player data:', error);
      }
    }
  }

  async loadMatches() {
    if (!this.currentPlayer?.id) {
      return;
    }

    try {
      await this.loadPlayerSpecificMatches();

      if (this.upcomingMatches.length === 0) {
        await this.loadMatchesByFiltering();
      }
    } catch (error) {
      console.error('Error loading matches:', error);
      await this.loadMatchesByFiltering();
    }
  }

  async loadPlayerSpecificMatches() {
    try {
      const playerMatches = await this.matchService.getPlayerMatches();
      this.upcomingMatches = playerMatches.filter(
        (match: any) => new Date(match.matchDate) > new Date()
      );
    } catch (error) {}
  }

  async loadMatchesByFiltering() {
    try {
      const futureMatches = await this.matchService.getFutureMatches();

      this.upcomingMatches = futureMatches.filter((match) => {
        const isPlayerInMatch = match.playerHistory?.some(
          (ph) => ph.user && ph.user.id === this.currentPlayer?.id
        );
        return isPlayerInMatch;
      });

      for (const match of this.upcomingMatches) {
        if (!match.teamAName) {
          match.teamAName = match.teamAName || 'Team A';
        }
        if (!match.teamBName) {
          match.teamBName = match.teamBName || 'Team B';
        }
      }
    } catch (error) {
      console.error('Error in loadMatchesByFiltering:', error);
    }
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

  async openPlayersModal(match: Match) {
    try {
      this.modalType = 'view';
      await this.loadMatchDetails(match);
      this.modalOpen = true;
    } catch (error) {
      console.error('Error loading player data:', error);
    }
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
        ? teamA.players.map((p: any) => `${p.username} - ${(p.rating || 0).toFixed(1)}`)
        : [];

      this.selectedTeamBPlayers = teamB
        ? teamB.players.map((p: any) => `${p.username} - ${(p.rating || 0).toFixed(1)}`)
        : [];
    } catch (error) {
      this.selectedTeamAPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamAId && p.user)
        .map((p) => `${p.user.username}- ${(p.user.rating || 0).toFixed(1)}`);

      this.selectedTeamBPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamBId && p.user)
        .map((p) => `${p.user.username} - ${(p.user.rating || 0).toFixed(1)}`);
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
      await this.loadMatches();
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
      await this.loadMatches();
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
        await this.loadMatches();
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
