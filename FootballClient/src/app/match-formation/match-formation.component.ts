import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Player } from '../player.interface';

interface Position {
    left: string;
    top: string;
}

@Component({
    selector: 'app-match-formation',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './match-formation.component.html',
    styleUrls: ['./match-formation.component.css']
})
export class MatchFormationComponent implements OnInit {
    @Input() team1Players: Player[] = [];
    @Input() team2Players: Player[] = [];

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

    ngOnInit() {
        const navigation = window.history.state;
        if (navigation) {
            this.team1Players = navigation.team1Players || [];
            this.team2Players = navigation.team2Players || [];
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
            return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGNpcmNsZSBjeD0iMjAiIGN5PSIyMCIgcj0iMjAiIGZpbGw9IiNjY2NjY2MiLz4KPHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4PSI4IiB5PSI4Ij4KPHBhdGggZD0iTTEyIDEyQzE0LjIwOTEgMTIgMTYgMTAuMjA5MSAxNiA4QzE2IDUuNzkwODYgMTQuMjA5MSA0IDEyIDRDOS43OTA4NiA0IDggNS43OTA4NiA4IDhDOCAxMC4yMDkxIDkuNzkwODYgMTIgMTIgMTJaIiBmaWxsPSJ3aGl0ZSIvPgo8cGF0aCBkPSJNMTIgMTRDOC4xMzQwMSAxNCA1IDE3LjEzNDAgNSAyMVYyMkMxIDIyIDIzIDIyIDIzIDIyVjIxQzIzIDE3LjEzNDAgMTkuODY2IDE0IDEyIDE0WiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+Cjwvc3ZnPgo=';
        }
        return player.imageUrl;
    }

    onImageError(event: any) {
        console.log('Image failed to load:', event.target.src);
        event.target.src = this.getDefaultImage();
    }

    private getDefaultImage(): string {
        return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGNpcmNsZSBjeD0iMjAiIGN5PSIyMCIgcj0iMjAiIGZpbGw9IiNjY2NjY2MiLz4KPHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4PSI4IiB5PSI4Ij4KPHBhdGggZD0iTTEyIDEyQzE0LjIwOTEgMTIgMTYgMTAuMjA5MSAxNiA4QzE2IDUuNzkwODYgMTQuMjA5MSA0IDEyIDRDOS43OTA4NiA0IDggNS43OTA4NiA4IDhDOCAxMC4yMDkxIDkuNzkwODYgMTIgMTIgMTJaIiBmaWxsPSJ3aGl0ZSIvPgo8cGF0aCBkPSJNMTIgMTRDOC4xMzQwMSAxNCA1IDE3LjEzNDAgNSAyMVYyMkMxIDIyIDIzIDIyIDIzIDIyVjIxQzIzIDE3LjEzNDAgMTkuODY2IDE0IDEyIDE0WiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+Cjwvc3ZnPgo=';
    }

    getStarArray(rating: number): string[] {
        const stars: string[] = [];
        const fullStars = Math.floor(rating / 2);
        const hasHalfStar = (rating % 2) >= 1;

        for (let i = 0; i < fullStars; i++) {
            stars.push('full');
        }

        if (hasHalfStar && fullStars < 5) {
            stars.push('half');
        }

        while (stars.length < 5) {
            stars.push('empty');
        }

        return stars;
    }
}