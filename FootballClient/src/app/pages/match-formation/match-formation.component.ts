import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Player } from '../../models/player.interface';
import { MatchService } from '../../services/match.service';

interface Position {
    left: string;
    top: string;
}
//test branch
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

    constructor(
        private matchService: MatchService,
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
            // Formație 3-2 pentru 5 jucători
            return [
                { left: '25%', top: '20%' },  // Primul rând - 3 jucători
                { left: '25%', top: '50%' },
                { left: '25%', top: '80%' },
                { left: '40%', top: '35%' },  // Al doilea rând - 2 jucători
                { left: '40%', top: '65%' }
            ];
        } else {
            // Formație 4-2 pentru 6 jucători
            return [
                { left: '25%', top: '15%' },  // Primul rând - 4 jucători
                { left: '25%', top: '35%' },
                { left: '25%', top: '65%' },
                { left: '25%', top: '85%' },
                { left: '40%', top: '30%' },  // Al doilea rând - 2 jucători
                { left: '40%', top: '70%' }
            ];
        }
    }

    getTeam1Positions(): Position[] {
        return this.getTeamPositions(this.team1Players.length);
    }

    getTeam2Positions(): Position[] {
        const positions = this.getTeamPositions(this.team2Players.length);
        // Ajustăm pozițiile pentru echipa din dreapta
        return positions.map(pos => ({
            left: (100 - parseInt(pos.left.replace('%', ''))).toString() + '%',
            top: pos.top
        }));
    }

    async finalizeMatch() {
        if (this.matchId) {
            try {
                await this.matchService.updateMatch(this.matchId, {
                    teamAGoals: this.scoreA,
                    teamBGoals: this.scoreB
                });
                this.router.navigate(['/matches-history']);
            } catch (error) {
                console.error('Error updating match:', error);
                // You could add user feedback here
            }
        }
    }

    async ngOnInit() {
        const navigation = window.history.state;
        if (navigation) {
            this.team1Players = navigation.team1Players || [];
            this.team2Players = navigation.team2Players || [];
            this.matchId = navigation.matchId;
            this.teamAId = navigation.teamAId;
            this.teamBId = navigation.teamBId;

            // Calculate max rating from all players
            const allPlayers = [...this.team1Players, ...this.team2Players];
            this.maxRating = Math.max(...allPlayers.map(p => p?.rating || 0));

            // Load team names after we have the IDs
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

        // If maxRating is 0, use a default scale of 10
        const effectiveMaxRating = this.maxRating || 10;

        // Scale the rating relative to maxRating (5 stars maximum)
        const scaledRating = (rating / effectiveMaxRating) * 10;
        const fullStars = Math.floor(scaledRating / 2);
        const hasHalfStar = (scaledRating % 2) >= 1;

        // Add full stars
        for (let i = 0; i < fullStars && i < 5; i++) {
            stars.push('full');
        }

        // Add half star if applicable
        if (hasHalfStar && stars.length < 5) {
            stars.push('half');
        }

        // Fill remaining with empty stars
        while (stars.length < 5) {
            stars.push('empty');
        }

        return stars;
    }
}