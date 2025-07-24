import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';
import { Header } from '../header/header';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TeamService } from '../team.service';
import { MatchService } from '../match.service';

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
    matchDate: string = '';
    matchDayError: boolean = false;
    minDate: string = '';
 

    constructor(
        private playerService: PlayerService,
        private teamService: TeamService,
        private matchService: MatchService,
        private router: Router
    ) { }

    ngOnInit() {
        this.loadPlayers();
        this.setMinDateToday();
    }

    getRatingCategory(rating: number | undefined): string {
        if (!rating) return 'low';
        if (rating <= 5) return 'low';
        if (rating <= 8) return 'medium';
        return 'high';
    }

    setMinDateToday() {
  const today = new Date();
  this.minDate = today.toISOString().split('T')[0];
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
        player.isEnabled = false;
        this.saveSelectedPlayers();
    }

    unselectPlayer(player: Player) {
        player.isEnabled = true;
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
                player.isEnabled = false;
            } else {
                player.isAvailable = false;
                player.isEnabled = true;
            }
        });
    }

    private clearSelectedPlayers() {
        localStorage.removeItem('selectedPlayerIds');
        
        this.allPlayers.forEach(player => {
            player.isAvailable = false;
            player.isEnabled = true;
        });
    }

    generateTeams() {
        const players = [...this.selectedPlayers];

        players.sort((a, b) => (b.rating || 0) - (a.rating || 0));

        const team1Players: Player[] = [];
        const team2Players: Player[] = [];
        let team1Rating = 0;
        let team2Rating = 0;

        const totalPlayers = players.length;
        const playersToDistribute = players.length % 2 === 0 ? players : players.slice(0, -1);
        const team1Size = Math.ceil(playersToDistribute.length / 2);
        const team2Size = playersToDistribute.length - team1Size;

        playersToDistribute.forEach((player, index) => {
            if (team1Players.length < team1Size && (team2Players.length === team2Size || team1Rating <= team2Rating)) {
                team1Players.push(player);
                team1Rating += player.rating || 0;
            } else {
                team2Players.push(player);
                team2Rating += player.rating || 0;
            }
        });

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

    async beginMatch() {
        try {
            const timestamp = new Date().toISOString().replace(/[^0-9]/g, '').slice(-6);
            // Create Team A and Team B in the database
            const teamA = await this.teamService.createTeam(`Team A ${timestamp}`);
            const teamB = await this.teamService.createTeam(`Team B ${timestamp}`);

            // Create a match with the current date
            const currentDate = new Date();
            const match = await this.matchService.createMatch(teamA.id, teamB.id, currentDate);

            this.clearSelectedPlayers();

            this.router.navigate(['/match-formation'], {
                state: {
                    team1Players: this.team1.players,
                    team2Players: this.team2.players,
                    teamAId: teamA.id,
                    teamBId: teamB.id,
                    matchId: match.id
                }
            });
        } catch (error) {
            console.error('Failed to create teams or match:', error);
            this.error = 'Failed to create teams or match. Please try again.';
        }
    }

    validateMatchDay() {
  if (!this.matchDate) return;

  const selected = new Date(this.matchDate);
  const day = selected.getDay();

  const isTuesday = day === 2;
  const isThursday = day === 4;

  if (isTuesday || isThursday) {
    this.matchDayError = false;
  } else {
    this.matchDayError = true;
    this.matchDate = '';
  }
}

}