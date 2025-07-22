import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Player } from '../player.interface';

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

    // Pozițiile pentru 6 jucători pe fiecare parte
    formation = {
        team1: [ // Echipa din stânga (roșie)
            { left: '25%', top: '15%' },
            { left: '25%', top: '35%' },
            { left: '25%', top: '55%' },
            { left: '25%', top: '75%' },
            { left: '40%', top: '30%' },
            { left: '40%', top: '60%' }
        ],
        team2: [ // Echipa din dreapta (albastră)
            { left: '75%', top: '15%' },
            { left: '75%', top: '35%' },
            { left: '75%', top: '55%' },
            { left: '75%', top: '75%' },
            { left: '60%', top: '30%' },
            { left: '60%', top: '60%' }
        ]
    };

    ngOnInit() {
        // Verificăm dacă avem datele jucătorilor din state
        const navigation = window.history.state;
        if (navigation) {
            this.team1Players = navigation.team1Players || [];
            this.team2Players = navigation.team2Players || [];
        }
    }

    getPlayerName(player: Player | undefined): string {
        if (!player) return 'N/A';
        return `${player.firstName} ${player.lastName}`;
    }
} 