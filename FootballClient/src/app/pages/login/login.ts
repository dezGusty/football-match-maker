import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../components/auth/auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    if (this.authService.isLoggedIn()) {
    this.router.navigate(['/home']);
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
        password: this.password
      });
      this.router.navigate(['/home']);
    } catch (error) {
      this.errorMessage = 'Incorrect username or password';
      console.error('Eroare la autentificare:', error);
    }
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }

forgotPassword() {
  this.email = this.forgotEmail;
  this.authService.forgotPassword(this.email)
    .subscribe({
      next: () => alert('Email trimis cu succes! Verifică inbox-ul.'),
      error: (err) => {
        console.error(err);
        this.errorMessage = 'A apărut o eroare la trimiterea email-ului.';
      }
    });
  this.forgotEmail = '';
  this.email = '';
  this.closeForgotPasswordModal();
}

}
