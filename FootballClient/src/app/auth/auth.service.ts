import { Injectable } from '@angular/core';
import { LoginRequest } from './auth.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5145/api';
  private isAuthenticated = false;

  constructor() {
    // Verifica daca exista un utilizator salvat
    this.isAuthenticated = localStorage.getItem('isAuthenticated') === 'true';

    // Adauga event listener pentru inchiderea ferestrei
    window.addEventListener('beforeunload', () => {
      this.logout();
    });
  }

  async login(credentials: LoginRequest): Promise<void> {
    try {
      const response = await fetch(`${this.apiUrl}/user/authenticate`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials)
      });

      if (!response.ok) {
        throw new Error('Autentificare esuata');
      }

      this.isAuthenticated = true;
      localStorage.setItem('isAuthenticated', 'true');
    } catch (error) {
      console.error('Eroare la autentificare:', error);
      throw error;
    }
  }

  logout(): void {
    this.isAuthenticated = false;
    localStorage.removeItem('isAuthenticated');
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated;
  }
} 