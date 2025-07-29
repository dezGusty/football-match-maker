import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../../services/player.service';
import { Player } from '../../models/player.interface';
import { Header } from '../../components/header/header';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TeamService } from '../../services/team.service';
import { MatchService } from '../../services/match.service';
import { PlayerMatchHistoryService } from '../../services/player-match-history.service';

interface PlayerCategory {
    high: Player[];
    medium: Player[];
    low: Player[];
}

interface Team {
    players: Player[];
    averageRating: number;
    highCount: number;
    mediumCount: number;
    lowCount: number;
}

interface TeamVariant {
    team1: Team;
    team2: Team;
    ratingDifference: number;
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
    team1: Team = { players: [], averageRating: 0, highCount: 0, mediumCount: 0, lowCount: 0 };
    team2: Team = { players: [], averageRating: 0, highCount: 0, mediumCount: 0, lowCount: 0 };
    team1Name: string = '';
    team2Name: string = '';
    searchTerm: string = '';
    matchDate: string = '';
    matchDayError: boolean = false;
    minDate: string = '';
    teamVariants: TeamVariant[] = [];
    currentVariantIndex: number = 0;


    constructor(
        private playerService: PlayerService,
        private teamService: TeamService,
        private matchService: MatchService,
        private playerMatchHistoryService: PlayerMatchHistoryService,
        private router: Router
    ) { }

    ngOnInit() {
        this.loadPlayers();
        this.setMinDateToday();
    }

    private getNextTuesdayOrThursday(): Date {
        const today = new Date();
        let daysToAdd = 0;
        const currentDay = today.getDay();

        if (currentDay <= 2) {
            daysToAdd = 2 - currentDay;
        } else if (currentDay <= 4) {
            daysToAdd = 4 - currentDay;
        } else {
            daysToAdd = 9 - currentDay;
        }

        const nextDate = new Date(today);
        nextDate.setDate(today.getDate() + daysToAdd);
        return nextDate;
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
        this.teamVariants = [];
        
        // Generăm mai multe variante diferite de echipe (mărim la 5 variante)
        for (let variant = 0; variant < 5; variant++) {
            // Amestecăm jucătorii pentru fiecare variantă
            const shuffledPlayers = this.shufflePlayers(players);
            
            // Pentru fiecare variantă, vom încerca mai multe combinații de distribuție
            for (let attempt = 0; attempt < 3; attempt++) {
                const team1Players: Player[] = [];
                const team2Players: Player[] = [];

                // Calculăm dimensiunea țintă pentru fiecare echipă
                const totalPlayers = shuffledPlayers.length;
                const isEvenTotal = totalPlayers % 2 === 0;
                const targetSize1 = isEvenTotal ? totalPlayers / 2 : Math.ceil(totalPlayers / 2);
                const targetSize2 = isEvenTotal ? totalPlayers / 2 : Math.floor(totalPlayers / 2);

                // Implementăm diferite strategii de distribuție pentru fiecare încercare
                if (attempt === 0) {
                    // Prima strategie: Distribuție alternativă cu rating-uri amestecate
                    const sortedByRating = [...shuffledPlayers].sort((a, b) => (b.rating || 0) - (a.rating || 0));
                    for (let i = 0; i < sortedByRating.length; i++) {
                        if (i % 2 === 0 && team1Players.length < targetSize1) {
                            team1Players.push(sortedByRating[i]);
                        } else if (team2Players.length < targetSize2) {
                            team2Players.push(sortedByRating[i]);
                        } else {
                            team1Players.push(sortedByRating[i]);
                        }
                    }
                } else if (attempt === 1) {
                    // A doua strategie: Grupăm jucătorii în perechi de rating similar și le distribuim aleator
                    const playerPairs = this.createPlayerPairs(shuffledPlayers);
                    playerPairs.forEach(pair => {
                        if (Math.random() < 0.5 && team1Players.length < targetSize1) {
                            team1Players.push(pair[0]);
                            if (pair[1] && team2Players.length < targetSize2) {
                                team2Players.push(pair[1]);
                            }
                        } else if (team2Players.length < targetSize2) {
                            team2Players.push(pair[0]);
                            if (pair[1] && team1Players.length < targetSize1) {
                                team1Players.push(pair[1]);
                            }
                        }
                    });
                } else {
                    // A treia strategie: Distribuție complet aleatoare cu verificare de echilibru
                    const randomizedPlayers = this.shufflePlayers(shuffledPlayers);
                    randomizedPlayers.forEach(player => {
                        const team1Avg = team1Players.length > 0 ? 
                            this.calculateTeamAverage(team1Players) : 0;
                        const team2Avg = team2Players.length > 0 ? 
                            this.calculateTeamAverage(team2Players) : 0;

                        if ((team1Avg <= team2Avg && team1Players.length < targetSize1) || 
                            team2Players.length >= targetSize2) {
                            team1Players.push(player);
                        } else {
                            team2Players.push(player);
                        }
                    });
                }

                // Corectăm dacă s-a depășit limita de 6-5 sau 5-6
                while (team1Players.length > targetSize1) {
                    team2Players.push(team1Players.pop()!);
                }
                while (team2Players.length > targetSize2) {
                    team1Players.push(team2Players.pop()!);
                }

                // Optimizăm echipele folosind algoritmul existent
                for (let i = 0; i < 3; i++) {
                    if (!this.optimizeTeams(team1Players, team2Players)) break;
                }

                // Calculăm statisticile pentru această încercare
                const team1Stats = this.calculateTeamStats(team1Players);
                const team2Stats = this.calculateTeamStats(team2Players);
                const ratingDifference = Math.abs(team1Stats.averageRating - team2Stats.averageRating);

                // Adăugăm varianta la lista de variante
                this.teamVariants.push({
                    team1: team1Stats,
                    team2: team2Stats,
                    ratingDifference: ratingDifference
                });
            }
        }

        // Sortăm variantele după diferența de rating și eliminăm duplicatele
        this.teamVariants = this.filterUniqueCombinations(this.teamVariants);

        // Selectăm prima variantă (cea mai echilibrată)
        this.currentVariantIndex = 0;
        this.setCurrentVariant();

        this.team1Name = 'Team 1';
        this.team2Name = 'Team 2';
        
        const nextMatchDate = this.getNextTuesdayOrThursday();
        this.matchDate = nextMatchDate.toISOString().split('T')[0];

        this.showTeamsModal = true;
    }

    closeTeamsModal() {
        this.showTeamsModal = false;
    }

    async beginMatch() {
        try {

            if (!this.areTeamNamesValid()) {
                alert("Team names must be different and cannot be empty.");
                return;
            }
            const timestamp = new Date().toISOString().replace(/[^0-9]/g, '').slice(-6);

            const allSelectedPlayerIds = [...this.team1.players, ...this.team2.players].map(p => p.id!);
            await this.playerService.setMultiplePlayersUnavailable(allSelectedPlayerIds);

            const teamA = await this.teamService.createTeam(this.team1Name || 'Team A');
            const teamB = await this.teamService.createTeam(this.team2Name || 'Team B');

            const currentDate = new Date();
            const match = await this.matchService.createMatch(teamA.id, teamB.id, currentDate);

            const historyPromises: Promise<any>[] = [];

            for (const player of this.team1.players) {
                historyPromises.push(
                    this.playerMatchHistoryService.createPlayerMatchHistory(
                        player.id!,
                        teamA.id,
                        match.id
                    )
                );
            }

            for (const player of this.team2.players) {
                historyPromises.push(
                    this.playerMatchHistoryService.createPlayerMatchHistory(
                        player.id!,
                        teamB.id,
                        match.id
                    )
                );
            }

            await Promise.all(historyPromises);

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
            console.error('Failed to create teams, match, or player history:', error);
            this.error = 'Failed to create teams, match, or player history. Please try again.';
        }
    }
    async scheduleMatch() {
        try {
            if (!this.areTeamNamesValid()) {
                alert("Team names must be different and cannot be empty.");
                return;
            }

            if (!this.matchDate || this.matchDayError) {
                alert("Please select a valid match date (Tuesday or Thursday).");
                return;
            }

            // Creez echipele
            const teamA = await this.teamService.createTeam(this.team1Name || 'Team A');
            const teamB = await this.teamService.createTeam(this.team2Name || 'Team B');

            // Creez meciul cu data programată
            const selectedDate = new Date(this.matchDate);
            const match = await this.matchService.createMatch(teamA.id, teamB.id, selectedDate);

            // Salvez jucătorii în PlayerMatchHistory pentru meciul programat
            const historyPromises: Promise<any>[] = [];

            for (const player of this.team1.players) {
                historyPromises.push(
                    this.playerMatchHistoryService.createPlayerMatchHistory(
                        player.id!,
                        teamA.id,
                        match.id
                    )
                );
            }

            for (const player of this.team2.players) {
                historyPromises.push(
                    this.playerMatchHistoryService.createPlayerMatchHistory(
                        player.id!,
                        teamB.id,
                        match.id
                    )
                );
            }

            await Promise.all(historyPromises);

            // NU clear selected players - rămân disponibili pentru meciul programat
            alert(`Match scheduled successfully for ${this.matchDate}!`);
            this.closeTeamsModal();

        } catch (error) {
            console.error('Failed to schedule match:', error);
            this.error = 'Failed to schedule match. Please try again.';
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

    areTeamNamesValid(): boolean {
  return this.team1Name.trim() !== '' 
      && this.team2Name.trim() !== '' 
      && this.team1Name.trim().toLowerCase() !== this.team2Name.trim().toLowerCase();
}

    // Funcție pentru amestecarea aleatorie a jucătorilor
    private shufflePlayers(players: Player[]): Player[] {
        const shuffled = [...players];
        for (let i = shuffled.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
        }
        return shuffled;
    }

    // Funcție pentru crearea perechilor de jucători cu rating-uri similare
    private createPlayerPairs(players: Player[]): Player[][] {
        const sortedPlayers = [...players].sort((a, b) => (b.rating || 0) - (a.rating || 0));
        const pairs: Player[][] = [];
        
        for (let i = 0; i < sortedPlayers.length; i += 2) {
            if (i + 1 < sortedPlayers.length) {
                pairs.push([sortedPlayers[i], sortedPlayers[i + 1]]);
            } else {
                pairs.push([sortedPlayers[i]]);
            }
        }
        
        // Amestecăm perechile folosind o funcție specifică pentru array de perechi
        for (let i = pairs.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [pairs[i], pairs[j]] = [pairs[j], pairs[i]];
        }
        
        return pairs;
    }

    // Funcție pentru filtrarea combinațiilor unice și păstrarea celor mai bune
    private filterUniqueCombinations(variants: TeamVariant[]): TeamVariant[] {
        // Sortăm mai întâi după diferența de rating
        variants.sort((a, b) => a.ratingDifference - b.ratingDifference);

        // Folosim un Set pentru a urmări combinațiile unice
        const uniqueCombinations = new Set<string>();
        const filteredVariants: TeamVariant[] = [];

        variants.forEach(variant => {
            // Creăm o cheie unică bazată pe jucătorii din echipe
            const team1Players = variant.team1.players.map(p => p.id).sort().join(',');
            const team2Players = variant.team2.players.map(p => p.id).sort().join(',');
            const combinationKey = `${team1Players}|${team2Players}`;

            if (!uniqueCombinations.has(combinationKey)) {
                uniqueCombinations.add(combinationKey);
                filteredVariants.push(variant);
            }
        });

        // Păstrăm doar primele 5 variante unice cele mai echilibrate
        return filteredVariants.slice(0, 5);
    }

    // Funcție pentru setarea variantei curente
    setCurrentVariant() {
        if (this.teamVariants.length > 0) {
            const variant = this.teamVariants[this.currentVariantIndex];
            this.team1 = variant.team1;
            this.team2 = variant.team2;
        }
    }

    // Funcție pentru trecerea la următoarea variantă
    nextVariant() {
        if (this.currentVariantIndex < this.teamVariants.length - 1) {
            this.currentVariantIndex++;
            this.setCurrentVariant();
        }
    }

    // Funcție pentru trecerea la varianta anterioară
    previousVariant() {
        if (this.currentVariantIndex > 0) {
            this.currentVariantIndex--;
            this.setCurrentVariant();
        }
    }

    // Mutăm funcțiile existente în metode private ale clasei
    private calculateTeamAverage(team: Player[]): number {
        return team.reduce((sum, p) => sum + (p.rating || 0), 0) / team.length;
    }

    private findWeakestPlayer(team: Player[]): Player {
        return team.reduce((min, p) => (p.rating || 0) < (min.rating || 0) ? p : min, team[0]);
    }

    private optimizeTeams(team1Players: Player[], team2Players: Player[]): boolean {
        const team1Avg = this.calculateTeamAverage(team1Players);
        const team2Avg = this.calculateTeamAverage(team2Players);
        
        const weakerTeam = team1Avg < team2Avg ? team1Players : team2Players;
        const strongerTeam = team1Avg < team2Avg ? team2Players : team1Players;
        
        const weakestPlayer = this.findWeakestPlayer(weakerTeam);
        
        if ((weakestPlayer.rating || 0) < (this.calculateTeamAverage(weakerTeam) - 2)) {
            const swap = this.findBestSwap(strongerTeam, weakerTeam, weakestPlayer);
            if (swap) {
                const tempPlayer = strongerTeam[swap.sourceIndex];
                strongerTeam[swap.sourceIndex] = weakerTeam[swap.targetIndex];
                weakerTeam[swap.targetIndex] = tempPlayer;
                return true;
            }
        }
        return false;
    }

    private calculateTeamStats(teamPlayers: Player[]): Team {
        return {
            players: teamPlayers,
            averageRating: this.calculateTeamAverage(teamPlayers),
            highCount: teamPlayers.filter(p => (p.rating || 0) >= 7).length,
            mediumCount: teamPlayers.filter(p => (p.rating || 0) >= 4 && (p.rating || 0) < 7).length,
            lowCount: teamPlayers.filter(p => (p.rating || 0) < 4).length
        };
    }

    // Funcție pentru găsirea celui mai bun schimb posibil
    private findBestSwap(sourceTeam: Player[], targetTeam: Player[], weakPlayer: Player) {
        let bestSwap = null;
        let bestImprovementDiff = 0;

        for (let i = 0; i < sourceTeam.length; i++) {
            for (let j = 0; j < targetTeam.length; j++) {
                const sourcePlayer = sourceTeam[i];
                const targetPlayer = targetTeam[j];

                // Calculăm ratingurile medii înainte de schimb
                const sourceTeamRating = this.calculateTeamAverage(sourceTeam);
                const targetTeamRating = this.calculateTeamAverage(targetTeam);
                const currentDiff = Math.abs(sourceTeamRating - targetTeamRating);

                // Simulăm schimbul
                const newSourceTeam = [...sourceTeam];
                const newTargetTeam = [...targetTeam];
                newSourceTeam[i] = targetPlayer;
                newTargetTeam[j] = sourcePlayer;

                // Calculăm ratingurile medii după schimb
                const newSourceTeamRating = this.calculateTeamAverage(newSourceTeam);
                const newTargetTeamRating = this.calculateTeamAverage(newTargetTeam);
                const newDiff = Math.abs(newSourceTeamRating - newTargetTeamRating);

                // Verificăm dacă schimbul ar îmbunătăți echilibrul
                if (newDiff < currentDiff) {
                    const improvement = currentDiff - newDiff;
                    const ratingDiffBonus = 1 - Math.abs((sourcePlayer.rating || 0) - (targetPlayer.rating || 0)) / 10;
                    const totalImprovement = improvement + ratingDiffBonus;

                    if (totalImprovement > bestImprovementDiff) {
                        bestImprovementDiff = totalImprovement;
                        bestSwap = { sourceIndex: i, targetIndex: j };
                    }
                }
            }
        }

        return bestSwap;
    }
}