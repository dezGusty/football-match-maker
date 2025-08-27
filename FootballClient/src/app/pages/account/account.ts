import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Header } from '../../components/header/header';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { User } from '../../models/user.interface';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatchService } from '../../services/match.service';
import { Match } from '../../models/match.interface';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-account',
  imports: [Header, CommonModule, FormsModule],
  templateUrl: './account.html',
  styleUrl: './account.css',
})
export class Account {
  user: User | null = null;

  newPassword = '';
  confirmPassword = '';
  currentPassword = '';

  newUsername = '';
  usernamePassword = '';

  showPasswordForm = false;
  showUsernameForm = false;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private http: HttpClient,
    private notificationService: NotificationService
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

  togglePasswordForm() {
    this.showPasswordForm = !this.showPasswordForm;
    if (this.showPasswordForm) {
      this.showUsernameForm = false;
    }
    this.resetForms();
  }

  toggleUsernameForm() {
    this.showUsernameForm = !this.showUsernameForm;
    if (this.showUsernameForm) {
      this.showPasswordForm = false;
    }
    this.resetForms();
  }

  resetForms() {
    this.currentPassword = '';
    this.newPassword = '';
    this.confirmPassword = '';

    this.newUsername = '';
    this.usernamePassword = '';
  }

  async changePassword() {
    if (!this.user) return;
    try {
      const message = await this.userService.changePassword(
        this.user.id,
        this.currentPassword,
        this.newPassword,
        this.confirmPassword
      );
      this.notificationService.showSuccess(message);
      this.resetForms();
      this.showPasswordForm = false;
    } catch (error: any) {
      this.notificationService.showError(error.message || 'Password change failed');
    }
  }

  async changeUsername() {
    if (!this.user) return;
    try {
      const message = await this.userService.changeUsername(
        this.user.id,
        this.newUsername,
        this.usernamePassword
      );
      this.notificationService.showSuccess(message);
      this.user.username = this.newUsername;
      this.resetForms();
      this.showUsernameForm = false;
    } catch (error: any) {
      this.notificationService.showError(error.message || 'Username change failed');
    }
  }

  logout() {
    this.router.navigate(['/']);
    this.authService.logout();
  }

  getRoleString(role: UserRole): string {
    return UserRole[role];
  }

  UserRole = UserRole;
}
