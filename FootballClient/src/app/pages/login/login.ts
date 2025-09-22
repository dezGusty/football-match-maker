import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { FirebaseService } from '../../services/firebase.service';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class Login {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    private router: Router,
    private authService: AuthService,
    private notificationService: NotificationService,
    private firebaseService: FirebaseService
  ) {
    if (this.authService.isLoggedIn()) {
      this.redirectBasedOnRole();
    }
  }
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

    this.loadFirebaseData();
  }

  async loadFirebaseData(): Promise<void> {
    try {
      console.log('Încărcare date din Firebase...');

      const matches = await this.firebaseService.getAllMatches();
      console.log('Toate match-urile din Firebase:', matches);

      const ratings = await this.firebaseService.getAllRatings();
      console.log('Toate rating-urile din Firebase:', ratings);

      console.log('Datele Firebase au fost încărcate cu succes!');
    } catch (error) {
      console.error('Eroare la încărcarea datelor din Firebase:', error);
    }
  }

  showForgotPasswordModal = false;
  forgotEmail = '';

  openForgotPasswordModal() {
    this.forgotEmail = '';
    this.showForgotPasswordModal = true;
  }

  closeForgotPasswordModal() {
    this.showForgotPasswordModal = false;
  }

  async onLogin() {
    try {
      await this.authService.login({
        email: this.email,
        password: this.password,
      });

      // Redirect based on user role after successful login
      this.redirectBasedOnRole();
    } catch (error) {
      this.errorMessage = 'Incorrect username or password';
      console.error('Eroare la autentificare:', error);
    }
  }

  private redirectBasedOnRole() {
    const userRole = this.authService.getUserRole();

    // Redirect based on role
    if (userRole === UserRole.PLAYER) {
      this.router.navigate(['/player-dashboard']);
    } else if (userRole === UserRole.ORGANISER) {
      this.router.navigate(['/home']);
    } else {
      // Default fallback for ADMIN or unknown roles
      this.router.navigate(['/home']);
    }
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }

  forgotPassword() {
    this.email = this.forgotEmail;
    this.authService.forgotPassword(this.email).subscribe({
      next: () =>
        this.notificationService.showSuccess(
          'Email trimis cu succes! Verifică inbox-ul.'
        ),
      error: (err) => {
        console.error(err);
        this.errorMessage = 'A apărut o eroare la trimiterea email-ului.';
      },
    });
    this.forgotEmail = '';
    this.email = '';
    this.closeForgotPasswordModal();
  }
}
