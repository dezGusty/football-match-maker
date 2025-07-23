import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { AuthService } from '../auth/auth.service';
import { User } from '../models/user.interface';

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  styleUrls: ['./header.css']
})
export class Header {
  username: string = '';
  isMenuOpen = false;

  constructor(
    private router: Router,
    private userService: UserService,
    private authService: AuthService
  ) {
    this.loadUsername();
  }

  async loadUsername() {
    try {
      const userId = this.authService.getUserId();
      if (userId) {
        const user = await this.userService.getUserById(userId);
        this.username = user.username;
      }
    } catch (error) {
      console.error('Error loading username:', error);
      this.username = '';
    }
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
    this.isMenuOpen = false;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
    this.isMenuOpen = false;
  }
}
