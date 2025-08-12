import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../components/auth/auth.service';
import { User } from '../../models/user.interface';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserRole } from '../../models/user-role.enum';
import { PlayerHeaderComponent } from '../../components/player-header/player-header.component';
@Component({
  selector: 'app-player-account',
  standalone: true,
  imports: [CommonModule, FormsModule, PlayerHeaderComponent],
  templateUrl: './player-account.component.html',
  styleUrl: './player-account.component.css',
})
export class PlayerAccountComponent {
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
    private http: HttpClient
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
      alert(message);
      this.resetForms();
      this.showPasswordForm = false;
    } catch (error: any) {
      alert(error.message || 'Password change failed');
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
      alert(message);
      this.user.username = this.newUsername;
      this.resetForms();
      this.showUsernameForm = false;
    } catch (error: any) {
      alert(error.message || 'Username change failed');
    }
  }

  getRoleString(role: UserRole): string {
    return UserRole[role];
  }

  goBack() {
    this.router.navigate(['/player-dashboard']);
  }
}
