import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../components/auth/auth.service';
import { Router } from '@angular/router';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {
  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  role: UserRole = UserRole.ADMIN;
  errorMessage: string = '';
  successMessage: string = '';
  UserRole = UserRole;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    }
  }

  async onRegister() {
    if (!this.username || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Complete all fields!';
      this.successMessage = '';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match!';
      this.successMessage = '';
      return;
    }

    if (this.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters long!';
      this.successMessage = '';
      return;
    }

    if (this.username.length < 3) {
      this.errorMessage = 'Username must be at least 3 characters long!';
      this.successMessage = '';
      return;
    }

    try {
      await this.authService.register(this.email, this.username, this.password, this.role);
      this.successMessage = 'Registration successful!';
      this.errorMessage = '';
      this.router.navigate(['/home']);
      this.username = '';
      this.email = '';
      this.password = '';
      this.confirmPassword = '';
    } catch (error: any) {
      this.errorMessage = 'Registration failed: ' + error.message;
      this.successMessage = '';
    }
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}