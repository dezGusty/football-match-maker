import { Injectable } from '@angular/core';
import { LoginRequest } from './auth.interface';
import { UserRole } from '../../models/user-role.enum';
import { HttpClient } from '@angular/common/http';
import { PlayerUser } from '../../models/player-user.interface'; // importă interfața

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5145/api';
  private isAuthenticated = false;
  private userId: number | null = null;
  errorMessage: string = '';
  email: string = '';

  constructor(private http: HttpClient) {
    this.isAuthenticated = localStorage.getItem('isAuthenticated') === 'true';
    const savedUserId = localStorage.getItem('userId');
    if (savedUserId) {
      this.userId = parseInt(savedUserId, 10);
    }
  }

  async register(
    email: string,
    username: string,
    password: string,
    role: UserRole,
    firstName?: string,
    lastName?: string,
    rating?: number,
    imageUrl?: string,
  ): Promise<void> {
    try {
      let response: Response;
      if (role === UserRole.PLAYER) {
        const playerUser: PlayerUser = {
          email,
          username,
          password,
          firstName: firstName || '',
          lastName: lastName || '',
          rating: rating ?? 0,
          imageUrl,
        };
        response = await fetch(`${this.apiUrl}/user/create-player-user`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(playerUser),
        });
      } else {
        response = await fetch(`${this.apiUrl}/user`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ email, username, password, role, imageUrl }),
        });
      }

      if (!response.ok) {
        const text = await response.text();
        let message = 'Registration failed';
        try {
          const error = JSON.parse(text);
          message = error.message || message;
        } catch (_) {}
        throw new Error(message);
      }
      const userData = await response.json();
      const now = new Date().getTime();
      const expiresAt = now + 60 * 60 * 1000;

      this.isAuthenticated = true;
      this.userId = userData.id;
      localStorage.setItem('isAuthenticated', 'true');
      localStorage.setItem('userId', userData.id.toString());
      localStorage.setItem('authExpiresAt', expiresAt.toString());
      localStorage.setItem('userRole', userData.role);
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
  }

  async login(credentials: LoginRequest): Promise<void> {
    try {
      const response = await fetch(`${this.apiUrl}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include', 
        body: JSON.stringify(credentials),
      });

      if (!response.ok) {
        throw new Error('Failed to login. Please check your credentials.');
      }

      const userData = await response.json();

      const now = new Date().getTime();
      const expiresAt = now + 60 * 60 * 1000;

      this.isAuthenticated = true;
      this.userId = userData.id;
      localStorage.setItem('isAuthenticated', 'true');
      localStorage.setItem('userId', userData.id.toString());
      localStorage.setItem('authExpiresAt', expiresAt.toString());
      localStorage.setItem('userRole', userData.role);
    } catch (error) {
      console.error('Failed to login:', error);
      throw error;
    }
  }

  logout(): void {
    this.isAuthenticated = false;
    this.userId = null;
    localStorage.removeItem('isAuthenticated');
    localStorage.removeItem('userId');
    localStorage.removeItem('authExpiresAt');
  }

  isLoggedIn(): boolean {
    const isAuth = localStorage.getItem('isAuthenticated') === 'true';
    const expiresAt = parseInt(
      localStorage.getItem('authExpiresAt') || '0',
      10,
    );
    const now = new Date().getTime();

    if (!isAuth || now > expiresAt) {
      this.logout();
      return false;
    }

    return true;
  }

  isRegistered(): boolean {
    return localStorage.getItem('isAuthenticated') === 'true';
  }

  getUserId(): number | null {
    return this.userId;
  }

  getUserRole(): UserRole | null {
    const role = localStorage.getItem('userRole');
    if (role !== null) {
      const roleNum = Number(role);
      if (!isNaN(roleNum) && UserRole[roleNum] !== undefined) {
        return roleNum as UserRole;
      }
    }
    return null;
  }

  forgotPassword(email: string) {
    if (!email) {
      throw new Error('Te rog introdu adresa de email.');
    }

    return this.http.post(`${this.apiUrl}/user/update-forgot-password`, {
      email,
    });
  }
}
