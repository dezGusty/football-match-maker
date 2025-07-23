import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';
import { Header } from '../header/header';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface Team {
    players: Player[];
    averageRating: number;
}

@Component({
    selector: 'app-select-players',
    standalone: true,
    imports: [Header, CommonModule, FormsModule],
    templateUrl: './select-players.component.html',
    styleUrls: ['./select-players.component.css']
})
export class SelectPlayersComponent implements OnInit {
    allPlayers: Player[] = [];
    maxAvailable = 12;
    loading = false;
    error: string | null = null;
    showTeamsModal = false;
    team1: Team = { players: [], averageRating: 0 };
    team2: Team = { players: [], averageRating: 0 };
    searchTerm: string = '';

    constructor(
        private playerService: PlayerService,
        private router: Router
    ) { }

    ngOnInit() {
        this.loadPlayers();
    }

    getRatingCategory(rating: number | undefined): string {
        if (!rating) return 'low';
        if (rating <= 5) return 'low';
        if (rating <= 8) return 'medium';
        return 'high';
    }

    async loadPlayers() {
        try {
            this.loading = true;
            this.error = null;
            this.allPlayers = await this.playerService.getPlayers();
            this.restoreSelectedPlayers();
        } catch (error) {
            console.error('Failed to load players:', error);
            this.error = 'Failed to load players. Please try again later.';
        } finally {
            this.loading = false;
        }
    }

    get availablePlayers(): Player[] {
        return this.allPlayers.filter(p => p.isEnabled && !p.isAvailable);
    }

    get filteredAvailablePlayers(): Player[] {
        return this.availablePlayers.filter(player => 
            this.searchTerm === '' || 
            `${player.firstName} ${player.lastName}`.toLowerCase().includes(this.searchTerm.toLowerCase())
        );
    }

    get selectedPlayers(): Player[] {
        return this.allPlayers.filter(p => p.isAvailable);
    }

    selectPlayer(player: Player) {
        if (this.selectedPlayers.length >= this.maxAvailable) {
            alert(`Maximum ${this.maxAvailable} players can be selected.`);
            return;
        }
        player.isAvailable = true;
        this.saveSelectedPlayers();
    }

    unselectPlayer(player: Player) {
        player.isAvailable = false;
        this.saveSelectedPlayers();
    }

    private saveSelectedPlayers() {
        const selectedIds = this.selectedPlayers.map(p => p.id);
        localStorage.setItem('selectedPlayerIds', JSON.stringify(selectedIds));
    }

    private restoreSelectedPlayers() {
        const saved = localStorage.getItem('selectedPlayerIds');
        if (!saved) return;

        const selectedIds: number[] = JSON.parse(saved);
        this.allPlayers.forEach(player => {
            if (selectedIds.includes(player.id!)) {
                player.isAvailable = true;
            } else {
                player.isAvailable = false;
            }
        });
    }

    private clearSelectedPlayers() {
        // Ștergem din localStorage
        localStorage.removeItem('selectedPlayerIds');
        
        // Resetăm starea jucătorilor
        this.allPlayers.forEach(player => {
            player.isAvailable = false;
        });
    }

    generateTeams() {
        const players = [...this.selectedPlayers];

        // Sortăm jucătorii după rating în ordine descrescătoare
        players.sort((a, b) => (b.rating || 0) - (a.rating || 0));

        const team1Players: Player[] = [];
        const team2Players: Player[] = [];
        let team1Rating = 0;
        let team2Rating = 0;

        // Distribuim toți jucătorii în afară de ultimul dacă avem număr impar
        const playersToDistribute = players.length % 2 === 0 ? players : players.slice(0, -1);

        // Distribuim jucătorii alternativ între echipe, începând cu cei mai buni
        playersToDistribute.forEach((player) => {
            if (team1Rating <= team2Rating) {
                team1Players.push(player);
                team1Rating += player.rating || 0;
            } else {
                team2Players.push(player);
                team2Rating += player.rating || 0;
            }
        });

        // Dacă avem un număr impar de jucători, adăugăm ultimul jucător (cel mai slab)
        // la echipa cu scorul total mai mare
        if (players.length % 2 !== 0) {
            const lastPlayer = players[players.length - 1];
            if (team1Rating >= team2Rating) {
                team1Players.push(lastPlayer);
                team1Rating += lastPlayer.rating || 0;
            } else {
                team2Players.push(lastPlayer);
                team2Rating += lastPlayer.rating || 0;
            }
        }

        this.team1 = {
            players: team1Players,
            averageRating: team1Rating / team1Players.length
        };

        this.team2 = {
            players: team2Players,
            averageRating: team2Rating / team2Players.length
        };

        this.showTeamsModal = true;
    }

    closeTeamsModal() {
        this.showTeamsModal = false;
    }

    beginMatch() {
        // Ștergem jucătorii selectați înainte de a începe meciul
        this.clearSelectedPlayers();
        
        // Navigăm către pagina de meci
        this.router.navigate(['/match-formation'], {
            state: {
                team1Players: this.team1.players,
                team2Players: this.team2.players
            }
        });
    }
}
