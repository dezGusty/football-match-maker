import { Component } from '@angular/core';
import { Header } from '../../components/header/header';
import { MatchService } from '../../services/match.service';
import { Match } from '../../models/match.interface';
import { DatePipe } from '@angular/common';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatchStatus } from '../../models/match-status.enum';

@Component({
  selector: 'app-matches-history',
  imports: [Header, DatePipe, CommonModule],
  templateUrl: './matches-history.html',
  styleUrl: './matches-history.css',
})
export class MatchesHistory implements OnInit {
  matches: Match[] = [];
  MatchStatus = MatchStatus;

  constructor(private MatchService: MatchService) {}

  async ngOnInit() {
    await this.init();
  }

  async init() {
    try {
      this.matches = await this.MatchService.getPastMatches();
      for (const match of this.matches) {
        match.teamAName = await this.MatchService.getTeamById(match.teamAId);
        match.teamBName = await this.MatchService.getTeamById(match.teamBId);
      }
    } catch (error) {
      console.error('Error fetching matches:', error);
    }
  }

  selectedTeamAName: string = '';
  selectedTeamBName: string = '';
  selectedTeamAPlayers: string[] = [];
  selectedTeamBPlayers: string[] = [];
  showDetails: boolean = false;
  selectedMatch: Match | null = null;
  statusText: string = '';
  modalOpen = false;

  openPlayersModal(match: Match): void {
    this.selectedMatch = match;
    this.modalOpen = true;
  }

  closeModal() {
    this.modalOpen = false;
    this.selectedMatch = null;
  }

  getPlayers(match: Match | null, teamId?: number | undefined): string[] {
    if (!match || !teamId) return [];
    return match.playerHistory
      .filter((ph) => ph.teamId === teamId && ph.user)
      .map(
        (ph) =>
          `${ph.user.firstName} ${ph.user.lastName} ${(
            ph.user.rating || 0
          ).toFixed(2)}`
      );
  }

  getScoreDisplay(match: Match): string {
    switch (match.status) {
      case MatchStatus.Finalized:
        return `${match.scoreA || 0} - ${match.scoreB || 0}`;
      case MatchStatus.Closed:
        return "Waiting for organiser's score";
      default:
        return `Status: ${match.status}`;
    }
  }
}
