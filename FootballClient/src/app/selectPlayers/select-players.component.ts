import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerService } from '../player.service';
import { Player } from '../player.interface';
import { Header } from '../header/header';

@Component({
    selector: 'app-select-players',
    standalone: true,
    imports: [Header, CommonModule],
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
            for (const player of this.allPlayers) {
                player.isSelected = false;
            }
        } catch (error) {
            console.error('Failed to load players:', error);
            this.error = 'Failed to load players. Please try again later.';
        } finally {
            this.loading = false;
        }
    }

    get availablePlayers(): Player[] {
        return this.allPlayers.filter(p => !p.isSelected && p.isEnabled);
    }

    get selectedPlayers(): Player[] {
        return this.allPlayers.filter(p => p.isSelected);
    }

    selectPlayer(player: Player) {
        if (this.selectedPlayers.length >= this.maxAvailable) {
            alert(`Maximum ${this.maxAvailable} players can be selected.`);
            return;
        }
        player.isSelected = true;
    }

    unselectPlayer(player: Player) {
        player.isSelected = false;
    }

    private updatePlayerInList(updatedPlayer: Player) {
        this.allPlayers = this.allPlayers.map(p =>
            p.id === updatedPlayer.id ? updatedPlayer : p
        );
    }
}
