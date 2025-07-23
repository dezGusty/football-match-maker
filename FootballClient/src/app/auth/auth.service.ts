import { Injectable } from '@angular/core';
import { LoginRequest } from './auth.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5145/api';
  private isAuthenticated = false;
  private userId: number | null = null;

  constructor() {
    this.isAuthenticated = localStorage.getItem('isAuthenticated') === 'true';
    const savedUserId = localStorage.getItem('userId');
    if (savedUserId) {
      this.userId = parseInt(savedUserId, 10);
    }

    // Event listener pentru închiderea ferestrei
    window.addEventListener('beforeunload', () => {
      this.logout();
    });
  }

  async register(username: string, password: string, role: string = 'Admin'): Promise<void> {
    try {
      const response = await fetch(`${this.apiUrl}/user`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username, password, role })
      });

      if (!response.ok) {
      const text = await response.text();
      let message = 'Registration failed';
      try {
        const error = JSON.parse(text);
        message = error.message || message;
      } catch (_) {
        // leave default message
      }
      throw new Error(message);
    }
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
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
        throw new Error('Autentificare eșuată');
      }

      const userData = await response.json();
      this.isAuthenticated = true;
      this.userId = userData.id;
      localStorage.setItem('isAuthenticated', 'true');
      localStorage.setItem('userId', userData.id.toString());
    } catch (error) {
      console.error('Eroare la autentificare:', error);
      throw error;
    }
  }

  logout(): void {
    this.isAuthenticated = false;
    this.userId = null;
    localStorage.removeItem('isAuthenticated');
    localStorage.removeItem('userId');
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated;
  }

  isRegistered(): boolean {
    return localStorage.getItem('isAuthenticated') === 'true';
  }

  getUserId(): number | null {
    return this.userId;
  }
} 