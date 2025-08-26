import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatchService } from '../../services/match.service';
import { UserService } from '../../services/user.service';
import { PlayerHeaderComponent } from '../../components/player-header/player-header.component';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';
import { Match } from '../../models/match.interface';
import { User } from '../../models/user.interface';

@Component({
  selector: 'app-player-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DatePipe,
    PlayerHeaderComponent,
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
    private router: Router
  ) {}

  async ngOnInit() {
    await this.loadPlayerData();

    await this.loadMatches();
    await this.loadAvailableMatches();
    await this.loadPublicMatches();
    await this.loadPlayerSpecificMatches();
    // await this.loadMatchesByFiltering();
  }

  async loadPlayerData() {
    const userId = this.authService.getUserId();

    if (userId) {
      try {
        const userResponse = await fetch(
          `http://localhost:5145/api/user/${userId}`
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
      const response = await fetch(
        `http://localhost:5145/api/user/${this.currentPlayer!.id}/matches`
      );
      if (response.ok) {
        const playerMatches = await response.json();
        this.upcomingMatches = playerMatches.filter(
          (match: any) => new Date(match.matchDate) > new Date()
        );

        return;
      }
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
        ? teamA.players.map(
            (p: any) => `${p.firstName} ${p.lastName} - ${p.rating}`
          )
        : [];

      this.selectedTeamBPlayers = teamB
        ? teamB.players.map(
            (p: any) => `${p.firstName} ${p.lastName} - ${p.rating}`
          )
        : [];
    } catch (error) {
      this.selectedTeamAPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamAId && p.user)
        .map(
          (p) => `${p.user.firstName} ${p.user.lastName} - ${p.user.rating}`
        );

      this.selectedTeamBPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamBId && p.user)
        .map(
          (p) => `${p.user.firstName} ${p.user.lastName} - ${p.user.rating}`
        );
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
      alert('Failed to join match. Please try again.');
    }
  }

  async joinTeam(teamId: number) {
    try {
      if (!this.selectedMatch?.id) {
        console.error('Match ID is missing');
        return;
      }

      if (teamId === undefined || teamId === null) {
        console.log('Team ID is undefined, using general joinMatch instead');
        await this.matchService.joinMatch(this.selectedMatch.id);
      } else {
        await this.matchService.joinTeam(this.selectedMatch.id, teamId);
      }

      this.closeModal();
      await this.loadMatches();
      await this.loadAvailableMatches();

      alert('Successfully joined the match!');
    } catch (error) {
      console.error('Error joining match:', error);
      alert('Failed to join match. Please try again.');
    }
  }

  async leaveMatch(match: Match) {
    try {
      if (!match.id) {
        console.error('Match ID is missing');
        return;
      }

      if (!this.canLeaveMatch(match)) {
        alert(
          'You cannot leave this match because you were added by the organizer.'
        );
        return;
      }

      if (confirm('Are you sure you want to leave this match?')) {
        await this.matchService.leaveMatch(match.id);
        await this.loadMatches();
        await this.loadAvailableMatches();
        alert('Successfully left the match!');
      }
    } catch (error) {
      console.error('Error leaving match:', error);
      alert('Failed to leave match. Please try again.');
    }
  }
}
