import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.interface';

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
  isMenuOpen = false;
  currentPlayer: User | null = null;

  @Output() tabChange = new EventEmitter<string>();

  constructor(
    private router: Router,
    private authService: AuthService,
    private userService: UserService
  ) {}

  async ngOnInit() {
    await this.loadPlayerData();
  }

  async loadPlayerData() {
    const userId = this.authService.getUserId();
    if (userId) {
      try {
        // Get user data for username and image
        const user = await this.userService.getUserById(userId);
        this.username = user.username;

        this.currentPlayer = user;
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
    this.tabChange.emit(tab);
    this.isMenuOpen = false;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.isMenuOpen = false;
  }
}
