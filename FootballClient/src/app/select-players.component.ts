import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerService } from './player.service';
import { Player } from './player.interface';

@Component({
    selector: 'app-select-players',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './select-players.component.html',
    styleUrls: ['./select-players.component.css']
})
export class SelectPlayersComponent implements OnInit {
    allPlayers: Player[] = [];
    maxAvailable = 12;
    loading = false;
    error: string | null = null;

    constructor(private playerService: PlayerService) { }

    ngOnInit() {
        this.loadPlayers();
    }

    async loadPlayers() {
        try {
            this.loading = true;
            this.error = null;
            this.allPlayers = await this.playerService.getPlayers();
        } catch (error) {
            console.error('Failed to load players:', error);
            this.error = 'Failed to load players. Please try again later.';
        } finally {
            this.loading = false;
        }
    }

    get unavailablePlayers(): Player[] {
        return this.allPlayers.filter(p => !p.isAvailable);
    }

    get availablePlayers(): Player[] {
        return this.allPlayers
            .filter(p => p.isAvailable)
            .slice(0, this.maxAvailable);
    }

    async selectPlayer(player: Player) {
        if (this.availablePlayers.length >= this.maxAvailable) {
            alert(`Maximum ${this.maxAvailable} players can be selected.`);
            return;
        }

        try {
            const updated = await this.playerService.editPlayer({
                ...player,
                isAvailable: true
            });

            this.updatePlayerInList(updated);
        } catch (error) {
            console.error('Failed to select player:', error);
            alert('Failed to select player. Please try again.');
        }
    }

    async unselectPlayer(player: Player) {
        try {
            const updated = await this.playerService.editPlayer({
                ...player,
                isAvailable: false
            });

            this.updatePlayerInList(updated);
        } catch (error) {
            console.error('Failed to unselect player:', error);
            alert('Failed to unselect player. Please try again.');
        }
    }

    private updatePlayerInList(updatedPlayer: Player) {
        this.allPlayers = this.allPlayers.map(p =>
            p.id === updatedPlayer.id ? updatedPlayer : p
        );
    }
}
