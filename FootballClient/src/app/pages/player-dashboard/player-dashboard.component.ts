import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatchService } from '../../services/match.service';
import { PlayerService } from '../../services/player.service';
import { PlayerHeaderComponent } from '../../components/player-header/player-header.component';
import { FriendRequestsComponent } from '../../components/friend-requests/friend-requests.component';
import { Match } from '../../models/match.interface';
import { Player } from '../../models/player.interface';
import { PlayerHistory } from '../../models/player-history.interface';

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
  activeTab: string = 'future'; // 'future', 'past'
  currentPlayer: Player | null = null;

  upcomingMatches: Match[] = [];
  matchHistory: Match[] = [];

  // Modal pentru afișarea jucătorilor
  modalOpen: boolean = false;
  selectedMatch: Match | null = null;
  selectedTeamAName: string = '';
  selectedTeamBName: string = '';
  selectedTeamAPlayers: string[] = [];
  selectedTeamBPlayers: string[] = [];

  constructor(
    private authService: AuthService,
    private matchService: MatchService,
    private playerService: PlayerService,
    private router: Router
  ) {}

  async ngOnInit() {
    await this.loadPlayerData();
    await this.loadMatches();
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
          const players = await this.playerService.getPlayers();
          this.currentPlayer =
            players.find((p) => p.email === user.email) || null;
        }
      } catch (error) {
        console.error('Error loading player data:', error);
      }
    }
  }

  async loadMatches() {
    if (!this.currentPlayer?.id) return;

    try {
      // Încarcă meciurile viitoare
      const futureMatches = await this.matchService.getFutureMatches();
      this.upcomingMatches = futureMatches.filter((match) =>
        match.playerHistory?.some(
          (ph) => ph.player.id === this.currentPlayer?.id
        )
      );

      // Adaugă numele echipelor pentru meciurile viitoare
      for (const match of this.upcomingMatches) {
        match.teamAName = await this.matchService.getTeamById(match.teamAId);
        match.teamBName = await this.matchService.getTeamById(match.teamBId);
      }

      // Încarcă istoricul meciurilor
      const pastMatches = await this.matchService.getPastMatches();
      this.matchHistory = pastMatches.filter((match) =>
        match.playerHistory?.some(
          (ph) => ph.player.id === this.currentPlayer?.id
        )
      );

      // Adaugă numele echipelor pentru istoricul meciurilor
      for (const match of this.matchHistory) {
        match.teamAName = await this.matchService.getTeamById(match.teamAId);
        match.teamBName = await this.matchService.getTeamById(match.teamBId);
      }
    } catch (error) {
      console.error('Error loading matches:', error);
    }
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  async openPlayersModal(match: Match) {
    try {
      this.selectedMatch = match;
      this.selectedTeamAName = match.teamAName!;
      this.selectedTeamBName = match.teamBName!;

      this.selectedTeamAPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamAId && p.player)
        .map(
          (p) =>
            `${p.player.firstName} ${p.player.lastName} - ${p.player.rating}`
        );

      this.selectedTeamBPlayers = match.playerHistory
        .filter((p) => p.teamId === match.teamBId && p.player)
        .map(
          (p) =>
            `${p.player.firstName} ${p.player.lastName} - ${p.player.rating}`
        );

      this.modalOpen = true;
    } catch (error) {
      console.error('Error loading player data:', error);
    }
  }

  closeModal() {
    this.modalOpen = false;
    this.selectedMatch = null;
  }

  getPlayerTeamName(match: Match): string {
    const playerHistory = match.playerHistory?.find(
      (ph) => ph.player.id === this.currentPlayer?.id
    );
    if (!playerHistory) return '';

    return playerHistory.teamId === match.teamAId
      ? match.teamAName || 'Team A'
      : match.teamBName || 'Team B';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goToAccount() {
    this.router.navigate(['/player-account']);
  }
}
