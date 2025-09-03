import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatchService } from '../../services/match.service';
import { NotificationService } from '../../services/notification.service';
import { Header } from '../../components/header/header';
import { Match } from '../../models/match.interface';

@Component({
  selector: 'app-player-dashboard-availableMatches.component',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    Header,
  ],
  templateUrl: './player-dashboard-availableMatches.component.html',
  styleUrls: ['./player-dashboard-availableMatches.component.css'],
})
export class PlayerDashboardAvailableMatchesComponent implements OnInit {
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
    private router: Router,
    private notificationService: NotificationService
  ) {}

  async ngOnInit() {
    await this.loadPublicMatches();
  }


  async loadPublicMatches() {
    try {
      const publicMatches = await this.matchService.getMyPublicMatches();
      this.availableMatches = publicMatches.filter(
        (match: any) => new Date(match.matchDate) > new Date()
      );
    } catch (error) {
      console.error('Error loading public matches:', error);
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
      await this.loadPublicMatches();
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
      await this.loadPublicMatches();

      this.notificationService.showSuccess('Successfully joined the match!');
    } catch (error) {
      console.error('Error joining match:', error);
      this.notificationService.showError(
        'Failed to join match. Please try again.'
      );
    }
  }

}
