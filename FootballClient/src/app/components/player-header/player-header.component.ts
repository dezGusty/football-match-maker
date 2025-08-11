import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { UserService } from '../../services/user.service';
import { PlayerService } from '../../services/player.service';
import { User } from '../../models/user.interface';
import { Player } from '../../models/player.interface';

// Service pentru comunicarea între componente
@Component({
  selector: 'app-player-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './player-header.component.html',
  styleUrls: ['./player-header.component.css'],
})
export class PlayerHeaderComponent implements OnInit {
  username: string = '';
  imageUrl?: string;
  isMenuOpen = false;
  currentPlayer: Player | null = null;

  @Output() tabChange = new EventEmitter<string>();

  constructor(
    private router: Router,
    private authService: AuthService,
    private userService: UserService,
    private playerService: PlayerService,
  ) {}

  async ngOnInit() {
    await this.loadPlayerData();
  }

  async loadPlayerData() {
    const userId = this.authService.getUserId();
    if (userId) {
      try {
        // Get user data for username and image
        const user = await this.userService.getUserWithImageById(userId);
        this.username = user.username;
        this.imageUrl = user.imageUrl;

        // Get player data for first/last name
        const players = await this.playerService.getPlayers();
        this.currentPlayer =
          players.find((p) => p.email === user.email) || null;
      } catch (error) {
        console.error('Error loading player data:', error);
      }
    }
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
    this.isMenuOpen = false;
  }

  switchTab(tab: string) {
    console.log('Switching to tab:', tab); // Debug log
    this.tabChange.emit(tab);
    this.isMenuOpen = false;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.isMenuOpen = false;
  }
}
