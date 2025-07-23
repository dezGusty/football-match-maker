import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Header } from '../header/header';
import { UserService } from '../services/user.service';
import { AuthService } from '../auth/auth.service';
import { User } from '../models/user.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'app-account',
  imports: [Header, CommonModule, FormsModule],
  templateUrl: './account.html',
  styleUrl: './account.css'
})
export class Account {
  user: User | null = null;
  newPassword = '';
  confirmPassword = '';

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {
    this.loadUser();
  }

  async loadUser() {
    try {
      const userId = this.authService.getUserId();
      if (userId) {
        this.user = await this.userService.getUserById(userId);
      }
    } catch (error) {
      console.error('Failed to load user:', error);
    }
  }

  changePassword() {
    // Implement password change logic here
  }

  logout() {
    this.router.navigate(['/']);
    this.authService.logout();
  }
}
