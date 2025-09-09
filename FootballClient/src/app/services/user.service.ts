import { Injectable } from '@angular/core';
import { User } from '../models/user.interface';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import {
  DelegateOrganizerRoleDto,
  ReclaimOrganizerRoleDto,
  OrganizerDelegateDto,
  DelegationStatusDto,
} from '../models/organizer-delegation.interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly url = `${environment.apiUrl}/user`;
  private readonly MAX_RATING = 10;
  private readonly MIN_RATING = 0;

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getAuthHeaders(): HeadersInit {
    const token = this.authService.getToken();
    return {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
    };
  }

  async getUserById(id: number): Promise<User> {
    const response = await fetch(`${this.url}/${id}`, {
      headers: this.getAuthHeaders(),
    });
    if (!response.ok) {
      throw new Error('Failed to fetch user data');
    }
    return (await response.json()) as User;
  }

  async changePassword(
    userId: number,
    currentPassword: string,
    newPassword: string,
    confirmPassword: string
  ): Promise<string> {
    const response = await fetch(`${this.url}/${userId}/change-password`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify({
        currentPassword,
        newPassword,
        confirmPassword,
      }),
    });
    const result = await response.json();
    if (!response.ok) {
      throw new Error(result.message || 'Failed to change password');
    }
    return result.message || 'Password changed successfully';
  }

  async changeUsername(
    userId: number,
    newUsername: string,
    password: string
  ): Promise<string> {
    const response = await fetch(`${this.url}/${userId}/change-username`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify({
        newUsername,
        password,
      }),
    });
    const result = await response.json();
    if (!response.ok) {
      throw new Error(result.message || 'Failed to change username');
    }
    return result.message || 'Username changed successfully';
  }

  private validateRating(rating: number): void {
    if (rating < this.MIN_RATING || rating > this.MAX_RATING) {
      throw new Error(
        `Rating must be between ${this.MIN_RATING} and ${this.MAX_RATING}`
      );
    }
  }

  async getPlayers(): Promise<User[]> {
    const response = await fetch(`${this.url}/players`, {
      headers: this.getAuthHeaders(),
    });
    if (!response.ok) {
      throw new Error('Failed to fetch players');
    }
    return await response.json();
  }

  async getAllUsers(): Promise<User[]> {
    const response = await fetch(`${this.url}`);
    if (!response.ok) {
      throw new Error('Failed to fetch users');
    }
    return await response.json();
  }

  async getPlayersForOrganiser(): Promise<User[]> {
    const response = await fetch(`${this.url}/organiser/players`, {
      headers: this.getAuthHeaders(),
    });
    if (!response.ok) {
      throw new Error('Failed to fetch players for organiser');
    }
    return await response.json();
  }

  async addPlayer(player: {
    firstName: string;
    lastName: string;
    email: string;
    rating: number;
    username: string;
    speed: number;
    stamina: number;
    errors: number;
  }): Promise<User> {
    this.validateRating(player.rating);

    const payload = {
      ...player,
      role: 2,
    };

    const response = await fetch(`${this.url}/create-player-account`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      let errorMsg = 'Failed to add player';
      try {
        const err = await response.json();
        if (err?.message) errorMsg = err.message;
      } catch {}
      throw new Error(errorMsg);
    }

    return await response.json();
  }

  async addUser(user: {
    firstName: string;
    lastName: string;
    email: string;
    rating: number;
    username: string;
    speed: number;
    stamina: number;
    errors: number;
    role: number;
  }): Promise<User> {
    this.validateRating(user.rating);

    const token = localStorage.getItem('authToken');
    const userId = localStorage.getItem('userId');

    const payload = {
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      rating: user.rating,
      username: user.username,
      speed: user.speed,
      stamina: user.stamina,
      errors: user.errors,
      role: user.role,
    };

    const response = await fetch(
      `${environment.apiUrl}/Auth/create-user-account`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(token && { Authorization: `Bearer ${token}` }),
          ...(userId && { UserId: userId }),
        },
        body: JSON.stringify(payload),
      }
    );

    if (!response.ok) {
      let errorMsg = 'Failed to add user';
      try {
        const err = await response.json();
        if (err && err.message) errorMsg = err.message;
      } catch {}
      throw new Error(errorMsg);
    }

    return await response.json();
  }

  async editUser(user: User): Promise<User> {
    if (user.rating !== undefined) {
      this.validateRating(user.rating);
    }

    const response = await fetch(`${this.url}/${user.id}`, {
      method: 'PUT',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(user),
    });

    if (!response.ok) {
      throw new Error('Failed to edit user');
    }

    return await response.json();
  }

  async deleteUser(userId: number): Promise<boolean> {
    const response = await fetch(`${this.url}/${userId}`, {
      method: 'DELETE',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to delete user');
    }

    return true;
  }

  async reactivateUser(userId: number): Promise<boolean> {
    const response = await fetch(`${this.url}/${userId}/reactivate`, {
      method: 'PATCH',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to reactivate user');
    }

    return true;
  }

  async updatePlayerRating(
    userId: number,
    ratingChange: number
  ): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/${userId}/update-rating`, {
        method: 'PATCH',
        headers: this.getAuthHeaders(),
        body: JSON.stringify({ ratingChange }),
      });

      if (!response.ok) {
        throw new Error('Failed to update player rating');
      }

      return true;
    } catch (error) {
      console.error('Error updating player rating:', error);
      return false;
    }
  }

  async addPlayerOrganiserRelation(userId: number): Promise<void> {
    const organiserId = this.authService.getUserId();
    if (!organiserId) {
      throw new Error('User not authenticated or organiser ID not available');
    }

    const body = { userId, organiserId };

    const response = await fetch(`${this.url}/player-organiser`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Response error:', response.status, errorText);
      throw new Error(
        `Failed to add player-organiser relation: ${response.status} ${errorText}`
      );
    }
  }

  async delegateOrganizerRole(
    userId: number,
    friendUserId: number,
    notes?: string
  ): Promise<OrganizerDelegateDto> {
    const body: DelegateOrganizerRoleDto = { friendUserId, notes };
    const response = await fetch(
      `${this.url}/${userId}/delegate-organizer-role`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(body),
      }
    );

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        errorResponse.message || 'Failed to delegate organizer role'
      );
    }

    return await response.json();
  }
  async reclaimOrganizerRole(
    userId: number,
    delegationId: number
  ): Promise<boolean> {
    const body: ReclaimOrganizerRoleDto = { delegationId };

    const response = await fetch(
      `${this.url}/${userId}/reclaim-organizer-role`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(body),
      }
    );

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        errorResponse.message || 'Failed to reclaim organizer role'
      );
    }

    return true;
  }

  async getDelegationStatus(userId: number): Promise<DelegationStatusDto> {
    const response = await fetch(`${this.url}/${userId}/delegation-status`, {
      method: 'GET',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        errorResponse.message || 'Failed to get delegation status'
      );
    }

    return await response.json();
  }
}
