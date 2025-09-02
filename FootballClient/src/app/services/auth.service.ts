import { Injectable } from '@angular/core';
import { LoginRequest } from '../models/auth.interface';
import { UserRole } from '../models/user-role.enum';
import { HttpClient } from '@angular/common/http';
import { PlayerUser } from '../models/player-user.interface'; // importă interfața
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}`;
  private isAuthenticated = false;
  private userId: number | null = null;
  private userRole: UserRole | null = null;
  errorMessage: string = '';
  email: string = '';

  constructor(private http: HttpClient, private router: Router) {
    this.LoadAuthState();
  }

  private LoadAuthState() {
    const token = this.getToken();
    if (token && !this.isTokenExpired(token)) {
      this.isAuthenticated = true;
      const savedUserId = localStorage.getItem('userId');
      const savedUserRole = localStorage.getItem('userRole');

      if (savedUserId) {
        this.userId = parseInt(savedUserId, 10);
      }
      if (savedUserRole) {
        this.userRole = parseInt(savedUserRole, 10) as UserRole;
      }
    } else {
      this.logout();
    }
  }

  async register(
    email: string,
    username: string,
    password: string,
    role: UserRole,
    firstName?: string,
    lastName?: string,
    rating?: number
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
          role: UserRole.PLAYER,
        };
        response = await fetch(`${this.apiUrl}/user/create-player-user`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            ...(this.getToken() && {
              Authorization: `Bearer ${this.getToken()}`,
            }),
            ...(this.getUserId() && { UserId: this.getUserId()!.toString() }),
          },
          body: JSON.stringify(playerUser),
        });
      } else {
        const organiserUser: PlayerUser = {
          email,
          username,
          password,
          firstName: firstName || '',
          lastName: lastName || '',
          rating: rating ?? 1000,
          role: UserRole.ORGANISER,
        };
        response = await fetch(`${this.apiUrl}/user/create-player-user`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            ...(this.getToken() && {
              Authorization: `Bearer ${this.getToken()}`,
            }),
            ...(this.getUserId() && { UserId: this.getUserId()!.toString() }),
          },
          body: JSON.stringify(organiserUser),
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
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
  }

  async login(credentials: LoginRequest): Promise<void> {
    try {
      const response = await fetch(`${this.apiUrl}/Auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials),
      });

      if (!response.ok) {
        let message = 'Failed to login. Please check your credentials.';
        try {
          const errorData = await response.json();
          message = errorData?.message || message;
        } catch {
          const text = await response.text();
          if (text) message = text;
        }
        throw new Error(message);
      }

      const loginResponse = await response.json();

      localStorage.setItem('authToken', loginResponse.token);

      const payload = JSON.parse(atob(loginResponse.token.split('.')[1]));
      if (payload?.exp) {
        localStorage.setItem('authExpiresAt', String(payload.exp * 1000));
      }

      localStorage.setItem('isAuthenticated', 'true');

      localStorage.setItem('userId', loginResponse.user.id.toString());
      localStorage.setItem('userRole', loginResponse.user.role.toString());

      this.isAuthenticated = true;
      this.userId = loginResponse.user.id;
      this.userRole = loginResponse.user.role;

      if (loginResponse.user.role === UserRole.PLAYER) {
        this.router.navigate(['/player-dashboard']);
      } else if (
        loginResponse.user.role === UserRole.ORGANISER ||
        loginResponse.user.role === UserRole.ADMIN
      ) {
        this.router.navigate(['/organizer-dashboard']);
      } else {
        this.router.navigate(['/']);
      }
    } catch (error) {
      console.error('Failed to login:', error);
      throw error;
    }
  }

  isLoggedIn(allowRedirect: boolean = true): boolean {
    const isAuth = localStorage.getItem('isAuthenticated') === 'true';
    const expiresAt = parseInt(
      localStorage.getItem('authExpiresAt') || '0',
      10
    );
    const now = new Date().getTime();

    if (!isAuth || now > expiresAt) {
      if (allowRedirect) {
        this.logout();
      } else {
        this.clearAuthState();
      }
      return false;
    }

    return true;
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

  async isDelegatedOrganizer(): Promise<boolean> {
    const userId = this.getUserId();
    const userRole = this.getUserRole();

    if (!userId || userRole !== UserRole.ORGANISER) {
      return false;
    }

    try {
      const response = await fetch(
        `${this.apiUrl}/user/${userId}/delegation-status`,
        {
          headers: {
            Authorization: `Bearer ${this.getToken()}`,
            'Content-Type': 'application/json',
          },
        }
      );

      if (!response.ok) {
        throw new Error('Failed to fetch delegation status');
      }

      const delegationStatus = await response.json();
      return delegationStatus.isDelegating === true;
    } catch (error) {
      console.error('Error checking delegation status:', error);
      return false;
    }
  }

  forgotPassword(email: string) {
    if (!email) {
      throw new Error('Te rog introdu adresa de email.');
    }

    return this.http.post(`${this.apiUrl}/Auth/forgot-password`, {
      email,
    });
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000;
      return Date.now() >= exp;
    } catch (error) {
      console.error('Error checking token expiration:', error);
      return true;
    }
  }

  private clearAuthState(): void {
    this.isAuthenticated = false;
    this.userId = null;
    this.userRole = null;
    localStorage.removeItem('authToken');
    localStorage.removeItem('isAuthenticated');
    localStorage.removeItem('userId');
    localStorage.removeItem('userRole');
    localStorage.removeItem('authExpiresAt');
  }
  async logout(redirect: boolean = true): Promise<void> {
    try {
      const token = this.getToken();
      if (token) {
        await fetch(`${this.apiUrl}/auth/logout`, {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      this.clearAuthState();
      if (redirect) {
        this.router.navigate(['/login']);
      }
    }
  }
}
