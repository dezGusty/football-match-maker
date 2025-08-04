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
    styleUrls: ['./match-formation.component.css']
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
    ) { }

    async loadTeamNames() {
        if (this.teamAId && this.teamBId) {
            try {
                const [teamA, teamB] = await Promise.all([
                    this.matchService.getTeamById(this.teamAId),
                    this.matchService.getTeamById(this.teamBId)
                ]);
                this.team1Name = teamA;
                this.team2Name = teamB;
            } catch (error) {
                console.error('Error loading team names:', error);
                this.team1Name = 'Team A';
                this.team2Name = 'Team B';
            }
        }
    }

    getTeamPositions(teamSize: number): Position[] {
        const positions: Position[] = [];

        if (teamSize < 5 || teamSize > 6) {
            console.warn(`Număr invalid de jucători: ${teamSize}. Se așteaptă 5 sau 6 jucători.`);
            return positions;
        }

        if (teamSize === 5) {
            return [
                { left: '25%', top: '20%' },
                { left: '25%', top: '50%' },
                { left: '25%', top: '80%' },
                { left: '40%', top: '35%' },
                { left: '40%', top: '65%' }
            ];
        } else {
            return [
                { left: '25%', top: '15%' },
                { left: '25%', top: '35%' },
                { left: '25%', top: '65%' },
                { left: '25%', top: '85%' },
                { left: '40%', top: '30%' },
                { left: '40%', top: '70%' }
            ];
        }
    }

    getTeam1Positions(): Position[] {
        return this.getTeamPositions(this.team1Players.length);
    }

    getTeam2Positions(): Position[] {
        const positions = this.getTeamPositions(this.team2Players.length);
        return positions.map(pos => ({
            left: (100 - parseInt(pos.left.replace('%', ''))).toString() + '%',
            top: pos.top
        }));
    }

    async finalizeMatch() {
        this.showRatingModal = true;
    }

    async confirmFinalize() {
    if (this.matchId) {
        try {
            await this.matchService.updateMatch(this.matchId, {
                teamAGoals: this.scoreA,
                teamBGoals: this.scoreB
            });

            const ratingUpdates: { playerId: number; ratingChange: number }[] = [];

            this.team1Players.forEach(player => {
                if (player.id) {
                    const totalRatingChange = this.getTotalRatingChange(player, true);
                    if (totalRatingChange !== 0) {
                        ratingUpdates.push({
                            playerId: player.id,
                            ratingChange: totalRatingChange
                        });
                    }
                }
            });

            this.team2Players.forEach(player => {
                if (player.id) {
                    const totalRatingChange = this.getTotalRatingChange(player, false);
                    if (totalRatingChange !== 0) {
                        ratingUpdates.push({
                            playerId: player.id,
                            ratingChange: totalRatingChange
                        });
                    }
                }
            });

            if (ratingUpdates.length > 0) {
                const success = await this.playerService.updateMultiplePlayerRatings(ratingUpdates);
                if (!success) {
                    console.error('Failed to update player ratings');
                }
            }

            this.showRatingModal = false;
            this.router.navigate(['/matches-history']);
        } catch (error) {
            console.error('Error finalizing match:', error);
        }
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

        const isWinningTeam = (isTeam1 && this.isTeam1Winner()) || (!isTeam1 && this.isTeam2Winner());

        const baseRatingChange = isWinningTeam ? 0.05 : -0.05;

        const goalDifference = Math.abs(this.scoreA - this.scoreB);
        const goalDifferenceBonus = goalDifference * 0.02;

        return baseRatingChange + (isWinningTeam ? goalDifferenceBonus : -goalDifferenceBonus);
    }

    getManualAdjustment(player: Player): number {
        return this.manualAdjustments.get(player.id || 0) || 0;
    }

    setManualAdjustment(player: Player, adjustment: number): void {
        if (player.id) {
            this.manualAdjustments.set(player.id, adjustment);
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

    async ngOnInit() {
        const navigation = window.history.state;
        if (navigation) {
            this.team1Players = navigation.team1Players || [];
            this.team2Players = navigation.team2Players || [];
            this.matchId = navigation.matchId;
            this.teamAId = navigation.teamAId;
            this.teamBId = navigation.teamBId;

            const allPlayers = [...this.team1Players, ...this.team2Players];
            this.maxRating = Math.max(...allPlayers.map(p => p?.rating || 0));

            await this.loadTeamNames();
        }

        if (this.team1Players.length < 5 || this.team1Players.length > 6) {
            console.error(`Echipa 1 are un număr invalid de jucători: ${this.team1Players.length}`);
        }
        if (this.team2Players.length < 5 || this.team2Players.length > 6) {
            console.error(`Echipa 2 are un număr invalid de jucători: ${this.team2Players.length}`);
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
        if (!player || !player.imageUrl) {
            return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGNpcmNsZSBjeD0iMjAiIGN5PSIyMCIgcj0iMjAiIGZpbGw9IiNjY2NjY2MiLz4KPHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4PSI4IiB5PSI4Ij4KPHBhdGggZD0iTTEyIDEyQzE0LjIwOTEgMTIgMTYgMTAuMjA5MSAxNiA4QzE2IDUuNzkwODYgMTQuMjA5MSA0IDEyIDRDOS43OTA4NiA0IDggNS43OTA4NiA4IDhDOCAxMC4yMDkxIDkuNzkwODYgMTIgMTJaIiBmaWxsPSJ3aGl0ZSIvPgo8cGF0aCBkPSJNMTIgMTRDOC4xMzQwMSAxNCA1IDE3LjEzNDAgNSAyMVYyMkMxIDIyIDIzIDIyIDIzIDIyVjIxQzIzIDE3LjEzNDAgMTkuODY2IDE0IDEyIDE0WiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+Cjwvc3ZnPgo=';
        }
        return player.imageUrl;
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
        const hasHalfStar = (scaledRating % 2) >= 1;

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