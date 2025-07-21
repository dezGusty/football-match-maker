import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    // Redirectioneaza catre home daca utilizatorul este deja autentificat
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
    }
  }

  async onLogin() {
    try {
      await this.authService.login({
        username: this.username,
        password: this.password
      });
      this.router.navigate(['/home']);
    } catch (error) {
      this.errorMessage = 'Username sau parola incorecte';
      console.error('Eroare la autentificare:', error);
    }
  }
}
