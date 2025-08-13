import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Player } from '../../models/player.interface';
import { MatchService } from '../../services/match.service';
import { PlayerService } from '../../services/player.service';

interface Position {
  left: string;
  top: string;
}

@Component({
  selector: 'app-match-formation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './match-formation.component.html',
  styleUrls: ['./match-formation.component.css'],
})
export class MatchFormationComponent implements OnInit {
  @Input() team1Players: Player[] = [];
  @Input() team2Players: Player[] = [];
  scoreA: number = 0;
  scoreB: number = 0;
  matchId?: number;
  team1Name: string = 'Loading...';
  team2Name: string = 'Loading...';
  teamAId?: number;
  teamBId?: number;
  private maxRating: number = 0;
  showRatingModal: boolean = false;

  manualAdjustments: Map<number, number> = new Map();

  constructor(
    private matchService: MatchService,
    private playerService: PlayerService,
    private router: Router
  ) {}

  async loadTeamNames() {
    if (this.teamAId && this.teamBId) {
      try {
        const [teamA, teamB] = await Promise.all([
          this.matchService.getTeamById(this.teamAId),
          this.matchService.getTeamById(this.teamBId),
        ]);
        this.team1Name = teamA;
        this.team2Name = teamB;
      } catch (error) {
        this.team1Name = 'Team A';
        this.team2Name = 'Team B';
      }
    }
  }

  getTeamPositions(teamSize: number): Position[] {
    const positions: Position[] = [];

    if (teamSize < 5 || teamSize > 6) {
      console.warn(
        `Invalid number of players: ${teamSize}. Expected 5 or 6 players.`
      );
      return positions;
    }

    if (teamSize === 5) {
      return [
        { left: '20%', top: '18%' },
        { left: '20%', top: '50%' },
        { left: '20%', top: '82%' },
        { left: '40%', top: '32%' },
        { left: '40%', top: '68%' },
      ];
    } else {
      return [
        { left: '20%', top: '12%' },
        { left: '20%', top: '32%' },
        { left: '20%', top: '68%' },
        { left: '20%', top: '88%' },
        { left: '40%', top: '27%' },
        { left: '40%', top: '73%' },
      ];
    }
  }

  getTeam1Positions(): Position[] {
    return this.getTeamPositions(this.team1Players.length);
  }

  getTeam2Positions(): Position[] {
    const positions = this.getTeamPositions(this.team2Players.length);
    return positions.map((pos) => ({
      left: (100 - parseInt(pos.left.replace('%', ''))).toString() + '%',
      top: pos.top,
    }));
  }

  async finalizeMatch() {
    console.log('finalizeMatch() called - showing rating modal');
    console.log('Current score:', this.scoreA, '-', this.scoreB);
    this.showRatingModal = true;
  }

  async confirmFinalize() {
    console.log('=== confirmFinalize() called ===');
    console.log('Match ID:', this.matchId);
    console.log('Score A:', this.scoreA, 'Score B:', this.scoreB);
    console.log('Manual adjustments:', this.manualAdjustments);

    if (this.matchId) {
      try {
        console.log('Updating match score...');
        await this.matchService.updateMatch(this.matchId, {
          teamAGoals: this.scoreA,
          teamBGoals: this.scoreB,
        });
        console.log(
          'Match score updated successfully - ratings updated automatically by backend'
        );

        if (this.manualAdjustments.size > 0) {
          const manualRatingUpdates: {
            playerId: number;
            ratingChange: number;
          }[] = [];

          console.log('Processing manual adjustments...');
          this.manualAdjustments.forEach((adjustment, playerId) => {
            if (adjustment !== 0) {
              console.log(
                `Manual adjustment for player ${playerId}: ${adjustment}`
              );
              manualRatingUpdates.push({
                playerId: playerId,
                ratingChange: adjustment,
              });
            }
          });

          if (manualRatingUpdates.length > 0) {
            console.log(
              'Applying manual rating adjustments:',
              manualRatingUpdates
            );
            const success =
              await this.playerService.updateMultiplePlayerRatings(
                manualRatingUpdates
              );
            if (success) {
              console.log('Manual rating adjustments applied successfully');
            } else {
              console.error('Failed to apply manual rating adjustments');
            }
          } else {
            console.log('No manual adjustments to apply');
          }
        } else {
          console.log('No manual adjustments set');
        }

        console.log('Match finalized successfully, navigating to history...');
        this.showRatingModal = false;
        this.router.navigate(['/matches-history']);
      } catch (error) {
        console.error('Error finalizing match:', error);
      }
    } else {
      console.error('No match ID found');
    }
  }
  closeModal() {
    this.showRatingModal = false;
  }

  isDraw(): boolean {
    return this.scoreA === this.scoreB;
  }

  isTeam1Winner(): boolean {
    return this.scoreA > this.scoreB;
  }

  isTeam2Winner(): boolean {
    return this.scoreB > this.scoreA;
  }

  getRatingChange(isTeam1: boolean): number {
    if (this.isDraw()) {
      return 0;
    }

    const isWinningTeam =
      (isTeam1 && this.isTeam1Winner()) || (!isTeam1 && this.isTeam2Winner());

    const baseRatingChange = isWinningTeam ? 0.05 : -0.05;

    const goalDifference = Math.abs(this.scoreA - this.scoreB);
    const goalDifferenceBonus = goalDifference * 0.02;

    return (
      baseRatingChange +
      (isWinningTeam ? goalDifferenceBonus : -goalDifferenceBonus)
    );
  }

  getManualAdjustment(player: Player): number {
    return this.manualAdjustments.get(player.id || 0) || 0;
  }

  setManualAdjustment(player: Player, adjustment: number): void {
    console.log(
      'setManualAdjustment called with:',
      player.firstName,
      player.lastName,
      adjustment
    );
    if (player.id) {
      const numAdjustment = isNaN(adjustment) ? 0 : Number(adjustment);
      console.log(
        `Setting manual adjustment for ${player.firstName} ${player.lastName}: ${numAdjustment}`
      );
      this.manualAdjustments.set(player.id, numAdjustment);
      console.log('Current manual adjustments:', this.manualAdjustments);
    }
  }

  resetManualAdjustment(player: Player): void {
    if (player.id) {
      this.manualAdjustments.delete(player.id);
    }
  }

  getTotalRatingChange(player: Player, isTeam1: boolean): number {
    const automaticChange = this.getRatingChange(isTeam1);
    const manualChange = this.getManualAdjustment(player);
    return automaticChange + manualChange;
  }

  getFinalRating(player: Player, isTeam1: boolean): number {
    const currentRating = player.rating || 0;
    const totalChange = this.getTotalRatingChange(player, isTeam1);
    return Math.max(0, currentRating + totalChange);
  }

  trackByPlayerId(index: number, player: Player): number {
    return player.id || index;
  }

  async ngOnInit() {
    const navigation = window.history.state;
    if (navigation) {
      this.team1Players = navigation.team1Players || [];
      this.team2Players = navigation.team2Players || [];
      this.matchId = navigation.matchId;
      this.teamAId = navigation.teamAId;
      this.teamBId = navigation.teamBId;

      const allPlayers = [...this.team1Players, ...this.team2Players];
      this.maxRating = Math.max(...allPlayers.map((p) => p?.rating || 0));

      await this.loadTeamNames();
    }

    if (this.team1Players.length < 5 || this.team1Players.length > 6) {
      console.error(
        `Team 1 has an invalid number of players: ${this.team1Players.length}`
      );
    }
    if (this.team2Players.length < 5 || this.team2Players.length > 6) {
      console.error(
        `Team 2 has an invalid number of players: ${this.team2Players.length}`
      );
    }
  }

  getPlayerName(player: Player | undefined): string {
    if (!player) return 'N/A';
    return `${player.firstName} ${player.lastName}`;
  }

  getPlayerFirstName(player: Player | undefined): string {
    if (!player || !player.firstName) return 'N/A';
    return player.firstName;
  }

  getPlayerLastName(player: Player | undefined): string {
    if (!player || !player.lastName) return '';
    return player.lastName;
  }

  getPlayerImage(player: Player | undefined): string {
    if (player?.profileImageUrl) {
      return player.profileImageUrl;
    }
    return this.getDefaultImage();
  }

  onImageError(event: any) {
    console.log('Image failed to load:', event.target.src);
    event.target.src = this.getDefaultImage();
  }

  private getDefaultImage(): string {
    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGNpcmNsZSBjeD0iMjAiIGN5PSIyMCIgcj0iMjAiIGZpbGw9IiNjY2NjY2MiLz4KPHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4PSI4IiB5PSI4Ij4KPHBhdGggZD0iTTEyIDEyQzE0LjIwOTEgMTIgMTYgMTAuMjA5MSAxNiA4QzE2IDUuNzkwODYgMTQuMjA5MSA0IDEyIDRDOS43OTA4NiA0IDggNS43OTA4NiA4IDhDOCAxMC4yMDkxIDkuNzkwODYgMTIgMTJaIiBmaWxsPSJ3aGl0ZSIvPgo8cGF0aCBkPSJNMTIgMTRDOC4xMzQwMSAxNCA1IDE3LjEzNDAgNSAyMVYyMkMxIDIyIDIzIDIyIDIzIDIyVjIxQzIzIDE3LjEzNDAgMTkuODY2IDE0IDEyIDE0WiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+Cjwvc3ZnPgo=';
  }

  getStarArray(rating: number): string[] {
    const stars: string[] = [];

    const effectiveMaxRating = this.maxRating || 10;

    const scaledRating = (rating / effectiveMaxRating) * 10;
    const fullStars = Math.floor(scaledRating / 2);
    const hasHalfStar = scaledRating % 2 >= 1;

    for (let i = 0; i < fullStars && i < 5; i++) {
      stars.push('full');
    }

    if (hasHalfStar && stars.length < 5) {
      stars.push('half');
    }

    while (stars.length < 5) {
      stars.push('empty');
    }

    return stars;
  }
}
