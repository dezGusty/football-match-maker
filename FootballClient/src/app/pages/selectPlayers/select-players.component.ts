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
// Update the path below to the correct location of player-stats.component.ts
import { PlayerStatsComponent } from '../../components/player-stats.component/player-stats.component';

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

@Component({
    selector: 'app-select-players',
    standalone: true,
    imports: [Header, CommonModule, FormsModule, PlayerStatsComponent],
    templateUrl: './select-players.component.html',
    styleUrls: ['./select-players.component.css']
})
export class SelectPlayersComponent implements OnInit {
    // Carousel properties
    currentSlide = 1;
    selectedDate: string | null = null;

    // Existing properties
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
    selectedPlayerTeam1: Player | null = null;
    selectedPlayerTeam2: Player | null = null;

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

    // Carousel methods
    goToSlide(slideNumber: number) {
        if (slideNumber === 2 && !this.selectedDate) {
            alert('Please select a date first!');
            return;
        }
        this.currentSlide = slideNumber;
    }

    goToNextSlide() {
        if (this.currentSlide === 1 && this.selectedDate) {
            this.currentSlide = 2;
        }
    }

    goToPreviousSlide() {
        if (this.currentSlide === 2) {
            this.currentSlide = 1;
        }
    }

    validateDate() {
        if (this.matchDate) {
            this.selectedDate = this.matchDate;
        } else {
            this.selectedDate = null;
        }
    }

    getFormattedDate(): string {
        if (!this.selectedDate) return 'No date selected';

        const date = new Date(this.selectedDate);
        return date.toLocaleDateString('en-US', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    }

    // Existing methods
    getRatingCategory(rating: number | undefined): string {
        if (!rating) return 'low';
        if (rating <= 5) return 'low';
        if (rating <= 8) return 'medium';
        return 'high';
    }

    setMinDateToday() {
        const today = new Date();
        this.minDate = today.toISOString().split('T')[0];
        // Set matchDate to today by default if not set
        if (!this.matchDate) {
            this.matchDate = this.minDate;
            this.selectedDate = this.minDate;
        }
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
            this.searchTerm === '' || `${player.firstName} ${player.lastName}`.toLowerCase().includes(this.searchTerm.toLowerCase())
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
        localStorage.removeItem('selectedPlayerIds');
        this.allPlayers.forEach(player => {
            player.isAvailable = false;
        });
    }

    generateTeams() {
        if (!this.selectedDate) {
            alert('Please select a date first!');
            this.goToSlide(1);
            return;
        }

        const players = [...this.selectedPlayers];
        this.generateSingleTeamVariant(players);
        this.team1Name = 'Team 1';
        this.team2Name = 'Team 2';
        this.showTeamsModal = true;
    }

    private generateSingleTeamVariant(players: Player[]) {
        const shuffledPlayers = this.shufflePlayers(players);
        const team1Players: Player[] = [];
        const team2Players: Player[] = [];

        const totalPlayers = shuffledPlayers.length;
        const isEvenTotal = totalPlayers % 2 === 0;
        const targetSize1 = isEvenTotal ? totalPlayers / 2 : Math.ceil(totalPlayers / 2);
        const targetSize2 = isEvenTotal ? totalPlayers / 2 : Math.floor(totalPlayers / 2);

        // Use balanced distribution strategy
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

        // Ensure correct team sizes
        while (team1Players.length > targetSize1) {
            team2Players.push(team1Players.pop()!);
        }
        while (team2Players.length > targetSize2) {
            team1Players.push(team2Players.pop()!);
        }

        // Sort players by rating and optimize teams
        team1Players.sort((a, b) => (b.rating || 0) - (a.rating || 0));
        team2Players.sort((a, b) => (b.rating || 0) - (a.rating || 0));

        for (let i = 0; i < 3; i++) {
            if (!this.optimizeTeams(team1Players, team2Players)) break;
        }

        // Calculate team stats
        const team1Stats = this.calculateTeamStats(team1Players);
        const team2Stats = this.calculateTeamStats(team2Players);

        // Set the teams directly
        this.team1 = {
            ...team1Stats,
            players: team1Stats.players.map(p => ({ ...p, locked: false }))
        };
        this.team2 = {
            ...team2Stats,
            players: team2Stats.players.map(p => ({ ...p, locked: false }))
        };

        this.restoreLockedPlayers();
    }

    shuffleTeams() {
        // Save current locked player IDs to preserve them
        this.saveLockedPlayers();

        // Get locked players and their current teams
        const lockedTeam1Players = this.team1.players.filter(p => p.locked);
        const lockedTeam2Players = this.team2.players.filter(p => p.locked);

        // Get unlocked players from both teams
        const unlockedPlayers = [
            ...this.team1.players.filter(p => !p.locked),
            ...this.team2.players.filter(p => !p.locked)
        ];

        // Calculate how many more players each team needs
        const totalPlayers = this.team1.players.length + this.team2.players.length;
        const isEvenTotal = totalPlayers % 2 === 0;
        const targetSize1 = isEvenTotal ? totalPlayers / 2 : Math.ceil(totalPlayers / 2);
        const targetSize2 = isEvenTotal ? totalPlayers / 2 : Math.floor(totalPlayers / 2);

        const team1NeededPlayers = targetSize1 - lockedTeam1Players.length;
        const team2NeededPlayers = targetSize2 - lockedTeam2Players.length;

        // If no unlocked players to shuffle, return early
        if (unlockedPlayers.length === 0) {
            return;
        }

        // Shuffle the unlocked players multiple times for better randomization
        let shuffledUnlocked = this.shufflePlayers(unlockedPlayers);
        shuffledUnlocked = this.shufflePlayers(shuffledUnlocked);
        shuffledUnlocked = this.shufflePlayers(shuffledUnlocked);

        // Redistribute unlocked players using different strategies randomly
        const newTeam1Unlocked: Player[] = [];
        const newTeam2Unlocked: Player[] = [];

        // Choose random distribution strategy
        const strategy = Math.floor(Math.random() * 3);

        if (strategy === 0) {
            // Strategy 1: Alternating by rating
            const sortedUnlocked = [...shuffledUnlocked].sort((a, b) => (b.rating || 0) - (a.rating || 0));
            for (let i = 0; i < sortedUnlocked.length; i++) {
                if (i % 2 === 0 && newTeam1Unlocked.length < team1NeededPlayers) {
                    newTeam1Unlocked.push(sortedUnlocked[i]);
                } else if (newTeam2Unlocked.length < team2NeededPlayers) {
                    newTeam2Unlocked.push(sortedUnlocked[i]);
                } else {
                    newTeam1Unlocked.push(sortedUnlocked[i]);
                }
            }
        } else if (strategy === 1) {
            // Strategy 2: Random assignment with balance checking
            shuffledUnlocked.forEach(player => {
                const team1Avg = newTeam1Unlocked.length > 0 ?
                    newTeam1Unlocked.reduce((sum, p) => sum + (p.rating || 0), 0) / newTeam1Unlocked.length : 0;
                const team2Avg = newTeam2Unlocked.length > 0 ?
                    newTeam2Unlocked.reduce((sum, p) => sum + (p.rating || 0), 0) / newTeam2Unlocked.length : 0;

                if ((team1Avg <= team2Avg && newTeam1Unlocked.length < team1NeededPlayers) ||
                    newTeam2Unlocked.length >= team2NeededPlayers) {
                    newTeam1Unlocked.push(player);
                } else {
                    newTeam2Unlocked.push(player);
                }
            });
        } else {
            // Strategy 3: Pure random assignment
            shuffledUnlocked.forEach(player => {
                if (Math.random() < 0.5 && newTeam1Unlocked.length < team1NeededPlayers) {
                    newTeam1Unlocked.push(player);
                } else if (newTeam2Unlocked.length < team2NeededPlayers) {
                    newTeam2Unlocked.push(player);
                } else {
                    newTeam1Unlocked.push(player);
                }
            });
        }

        // Ensure correct team sizes for unlocked players
        while (newTeam1Unlocked.length > team1NeededPlayers && newTeam2Unlocked.length < team2NeededPlayers) {
            newTeam2Unlocked.push(newTeam1Unlocked.pop()!);
        }
        while (newTeam2Unlocked.length > team2NeededPlayers && newTeam1Unlocked.length < team1NeededPlayers) {
            newTeam1Unlocked.push(newTeam2Unlocked.pop()!);
        }

        // Combine locked and newly assigned unlocked players
        const finalTeam1Players = [...lockedTeam1Players, ...newTeam1Unlocked];
        const finalTeam2Players = [...lockedTeam2Players, ...newTeam2Unlocked];

        // Sort by rating and optimize
        finalTeam1Players.sort((a, b) => (b.rating || 0) - (a.rating || 0));
        finalTeam2Players.sort((a, b) => (b.rating || 0) - (a.rating || 0));

        // Try to optimize teams while respecting locked players
        for (let i = 0; i < 3; i++) {
            if (!this.optimizeTeamsWithLocks(finalTeam1Players, finalTeam2Players)) break;
        }

        // Update team stats and reset locked status for new players
        this.team1 = this.calculateTeamStats(finalTeam1Players);
        this.team2 = this.calculateTeamStats(finalTeam2Players);

        // Restore locked status only for previously locked players
        this.restoreLockedPlayers();
    }

    private optimizeTeamsWithLocks(team1Players: Player[], team2Players: Player[]): boolean {
        const team1Avg = this.calculateTeamAverage(team1Players);
        const team2Avg = this.calculateTeamAverage(team2Players);

        const weakerTeam = team1Avg < team2Avg ? team1Players : team2Players;
        const strongerTeam = team1Avg < team2Avg ? team2Players : team1Players;

        // Find the best swap between unlocked players only
        let bestSwap = null;
        let bestImprovementDiff = 0;
        const currentDiff = Math.abs(team1Avg - team2Avg);

        for (let i = 0; i < strongerTeam.length; i++) {
            for (let j = 0; j < weakerTeam.length; j++) {
                const strongerPlayer = strongerTeam[i];
                const weakerPlayer = weakerTeam[j];

                // Only swap if both players are unlocked
                if (strongerPlayer.locked || weakerPlayer.locked) continue;

                const strongerPlayerRating = strongerPlayer.rating || 0;
                const weakerPlayerRating = weakerPlayer.rating || 0;
                const ratingDifference = weakerPlayerRating - strongerPlayerRating;

                const newStrongerTeamRating = team1Avg < team2Avg ?
                    team2Avg + ratingDifference / strongerTeam.length :
                    team1Avg + ratingDifference / strongerTeam.length;
                const newWeakerTeamRating = team1Avg < team2Avg ?
                    team1Avg - ratingDifference / weakerTeam.length :
                    team2Avg - ratingDifference / weakerTeam.length;

                const newDiff = Math.abs(newStrongerTeamRating - newWeakerTeamRating);

                if (newDiff < currentDiff) {
                    const improvement = currentDiff - newDiff;
                    if (improvement > bestImprovementDiff) {
                        bestImprovementDiff = improvement;
                        bestSwap = { strongerIndex: i, weakerIndex: j };
                    }
                }
            }
        }

        if (bestSwap) {
            const tempPlayer = strongerTeam[bestSwap.strongerIndex];
            strongerTeam[bestSwap.strongerIndex] = weakerTeam[bestSwap.weakerIndex];
            weakerTeam[bestSwap.weakerIndex] = tempPlayer;
            return true;
        }

        return false;
    }

    closeTeamsModal() {
        this.showTeamsModal = false;
    }

    // Adaugă această metodă în clasa SelectPlayersComponent
    private isCurrentDate(selectedDate: string): boolean {
        const today = new Date();
        const selected = new Date(selectedDate);

        // Resetăm timpul la 00:00:00 pentru comparația zilei
        today.setHours(0, 0, 0, 0);
        selected.setHours(0, 0, 0, 0);

        return today.getTime() === selected.getTime();
    }

    // Modifică metoda beginMatch existentă
    async beginMatch() {
        try {
            if (!this.areTeamNamesValid()) {
                alert("Team names must be different and cannot be empty.");
                return;
            }

            if (!this.selectedDate) {
                alert("Please select a match date first.");
                return;
            }

            // VALIDARE NOUĂ: Verifică dacă data selectată este data curentă
            if (!this.isCurrentDate(this.selectedDate)) {
                alert("Meciul poate fi început doar în data curentă! Pentru alte date, folosește opțiunea 'Schedule Match'.");
                return;
            }

            const allSelectedPlayerIds = [...this.team1.players, ...this.team2.players].map(p => p.id!);
            await this.playerService.setMultiplePlayersUnavailable(allSelectedPlayerIds);
            const teamA = await this.teamService.createTeam(this.team1Name || 'Team A');
            const teamB = await this.teamService.createTeam(this.team2Name || 'Team B');
            const selectedDateObj = new Date(this.selectedDate);
            const match = await this.matchService.createMatch(teamA.id, teamB.id, selectedDateObj);
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

            if (!this.selectedDate) {
                alert("Please select a match date first.");
                return;
            }

            // Create teams and match in parallel
            const [teamA, teamB] = await Promise.all([
                this.teamService.createTeam(this.team1Name || 'Team A'),
                this.teamService.createTeam(this.team2Name || 'Team B')
            ]);

            const selectedDateObj = new Date(this.selectedDate);
            const match = await this.matchService.createMatch(teamA.id, teamB.id, selectedDateObj);

            // Create all player histories in parallel for better performance
            const allHistoryPromises = [
                ...this.team1.players.map(player =>
                    this.playerMatchHistoryService.createPlayerMatchHistory(player.id!, teamA.id, match.id)
                ),
                ...this.team2.players.map(player =>
                    this.playerMatchHistoryService.createPlayerMatchHistory(player.id!, teamB.id, match.id)
                )
            ];

            await Promise.all(allHistoryPromises);
            alert(`Match scheduled successfully for ${this.getFormattedDate()}!`);
            this.closeTeamsModal();
        } catch (error) {
            console.error('Failed to schedule match:', error);
            this.error = 'Failed to schedule match. Please try again.';
        }
    }

    areTeamNamesValid(): boolean {
        return this.team1Name.trim() !== ''
            && this.team2Name.trim() !== ''
            && this.team1Name.trim().toLowerCase() !== this.team2Name.trim().toLowerCase();
    }

    private shufflePlayers(players: Player[]): Player[] {
        const shuffled = [...players];
        // Fisher-Yates shuffle algorithm with multiple passes for better randomization
        for (let pass = 0; pass < 3; pass++) {
            for (let i = shuffled.length - 1; i > 0; i--) {
                const j = Math.floor(Math.random() * (i + 1));
                [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
            }
        }
        return shuffled;
    }

    toggleLock(player: Player) {
        player.locked = !player.locked;
        this.saveLockedPlayers();
    }

    movePlayerToOtherTeam(player: Player, fromTeam: 'team1' | 'team2') {
        if (fromTeam === 'team1') {
            // Move from team1 to team2
            const playerIndex = this.team1.players.findIndex(p => p.id === player.id);
            if (playerIndex !== -1) {
                const movedPlayer = this.team1.players.splice(playerIndex, 1)[0];
                // Remove locked status when moving
                movedPlayer.locked = false;
                this.team2.players.push(movedPlayer);
            }
        } else {
            // Move from team2 to team1
            const playerIndex = this.team2.players.findIndex(p => p.id === player.id);
            if (playerIndex !== -1) {
                const movedPlayer = this.team2.players.splice(playerIndex, 1)[0];
                // Remove locked status when moving
                movedPlayer.locked = false;
                this.team1.players.push(movedPlayer);
            }
        }

        // Sort teams by rating after move
        this.team1.players.sort((a, b) => (b.rating || 0) - (a.rating || 0));
        this.team2.players.sort((a, b) => (b.rating || 0) - (a.rating || 0));

        // Recalculate team stats
        this.team1 = this.calculateTeamStats(this.team1.players);
        this.team2 = this.calculateTeamStats(this.team2.players);

        // Update locked players storage
        this.saveLockedPlayers();
    }

    saveLockedPlayers() {
        const lockedIds = [
            ...this.team1.players.filter(p => p.locked).map(p => p.id),
            ...this.team2.players.filter(p => p.locked).map(p => p.id)
        ];
        localStorage.setItem('lockedPlayerIds', JSON.stringify(lockedIds));
    }

    restoreLockedPlayers() {
        const saved = localStorage.getItem('lockedPlayerIds');
        if (!saved) return;
        const lockedIds: number[] = JSON.parse(saved);
        [this.team1, this.team2].forEach(team => {
            team.players.forEach(player => {
                player.locked = lockedIds.includes(player.id!);
            });
        });
    }

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

    private findBestSwap(sourceTeam: Player[], targetTeam: Player[], weakPlayer: Player) {
        let bestSwap = null;
        let bestImprovementDiff = 0;

        // Cache team ratings to avoid recalculation
        const sourceTeamRating = this.calculateTeamAverage(sourceTeam);
        const targetTeamRating = this.calculateTeamAverage(targetTeam);
        const currentDiff = Math.abs(sourceTeamRating - targetTeamRating);

        for (let i = 0; i < sourceTeam.length; i++) {
            for (let j = 0; j < targetTeam.length; j++) {
                const sourcePlayer = sourceTeam[i];
                const targetPlayer = targetTeam[j];

                // Calculate new ratings more efficiently
                const sourcePlayerRating = sourcePlayer.rating || 0;
                const targetPlayerRating = targetPlayer.rating || 0;
                const ratingDifference = targetPlayerRating - sourcePlayerRating;

                const newSourceTeamRating = sourceTeamRating + ratingDifference / sourceTeam.length;
                const newTargetTeamRating = targetTeamRating - ratingDifference / targetTeam.length;
                const newDiff = Math.abs(newSourceTeamRating - newTargetTeamRating);

                if (newDiff < currentDiff) {
                    const improvement = currentDiff - newDiff;
                    const ratingDiffBonus = 1 - Math.abs(sourcePlayerRating - targetPlayerRating) / 10;
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