import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css'],
})
export class Register {
  username: string = '';
  firstName: string = '';
  lastName: string = '';
  rating: number = 1000;
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  role: UserRole = UserRole.ORGANISER;
  errorMessage: string = '';
  successMessage: string = '';
  UserRole = UserRole;

  ngOnInit(): void {
    let index = 0;
    const slides =
      document.querySelectorAll<HTMLImageElement>('.slideshow .slide');

    setInterval(() => {
      slides.forEach((slide, i) => {
        slide.classList.toggle('active', i === index);
      });
      index = (index + 1) % slides.length;
    }, 4000);
  }

  constructor(private router: Router, private authService: AuthService) {
    if (this.authService.isLoggedIn()) {
      this.redirectBasedOnRole();
    }
  }

  async onRegister() {
    if (
      !this.username ||
      !this.email ||
      !this.password ||
      !this.confirmPassword ||
      !this.firstName ||
      !this.lastName
    ) {
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
      await this.authService.register(
        this.email,
        this.username,
        this.password,
        this.role,
        this.firstName,
        this.lastName,
        this.rating || 1000
      );

      this.successMessage = 'Registration successful! Redirecting to login...';
      this.errorMessage = '';

      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 1500);

      this.clearForm();
    } catch (error: any) {
      this.errorMessage = 'Registration failed: ' + error.message;
      this.successMessage = '';
    }
  }

  private redirectBasedOnRole() {
    const userRole = this.authService.getUserRole();

    if (userRole === UserRole.PLAYER) {
      this.router.navigate(['/player-dashboard']);
    } else if (userRole === UserRole.ORGANISER) {
      this.router.navigate(['/home']);
    } else {
      this.router.navigate(['/home']);
    }
  }

  private clearForm() {
    this.username = '';
    this.firstName = '';
    this.lastName = '';
    this.rating = 1000;
    this.email = '';
    this.password = '';
    this.confirmPassword = '';
    this.role = UserRole.ORGANISER;
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
