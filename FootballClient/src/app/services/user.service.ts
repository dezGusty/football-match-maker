import { Injectable } from '@angular/core';
import { User } from '../models/user.interface';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { 
  DelegateOrganizerRoleDto, 
  ReclaimOrganizerRoleDto, 
  OrganizerDelegateDto, 
  DelegationStatusDto 
} from '../models/organizer-delegation.interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly url = 'http://localhost:5145/api/user';
  private readonly MAX_RATING = 10000;
  private readonly MIN_RATING = 0;

  constructor(private http: HttpClient, private authService: AuthService) {}

  async getUserById(id: number): Promise<User> {
    const response = await fetch(`${this.url}/${id}`, {
      headers: { 'Content-Type': 'application/json' },
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
      headers: { 'Content-Type': 'application/json' },
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
      headers: { 'Content-Type': 'application/json' },
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

  async createPlayerUser(playerUser: any): Promise<any> {
    const token = localStorage.getItem('authToken');
    const userId = localStorage.getItem('userId');

    const response = await fetch(`${this.url}/create-player-user`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...(userId && { UserId: userId }),
      },
      body: JSON.stringify(playerUser),
    });
    const result = await response.json();
    if (!response.ok) {
      throw new Error(result.message || 'Failed to create player user');
    }
    return result;
  }

  private getAuthHeaders(): HeadersInit {
    const token = this.authService.getToken();
    return {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
    };
  }

  private validateRating(rating: number): void {
    if (rating < this.MIN_RATING || rating > this.MAX_RATING) {
      throw new Error(
        `Rating must be between ${this.MIN_RATING} and ${this.MAX_RATING}`
      );
    }
  }

  async getPlayers(): Promise<User[]> {
    const response = await fetch(`${this.url}/players`);
    if (!response.ok) {
      throw new Error('Failed to fetch players');
    }
    const players = await response.json();
    return players;
  }

  async getPlayersForOrganiser(organiserId: number): Promise<User[]> {
    const response = await fetch(`${this.url}/${organiserId}/players`);
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

    const token = localStorage.getItem('authToken');
    const userId = localStorage.getItem('userId');

    const payload = {
      firstName: player.firstName,
      lastName: player.lastName,
      email: player.email,
      rating: player.rating,
      username: player.username,
      speed: player.speed,
      stamina: player.stamina,
      errors: player.errors,
      role: 2,
    };

    const response = await fetch(
      'http://localhost:5145/api/Auth/create-player-account',
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
      let errorMsg = 'Failed to add player';
      try {
        const err = await response.json();
        if (err && err.message) errorMsg = err.message;
      } catch {}
      throw new Error(errorMsg);
    }

    return await response.json();
  }

  async updateUser(userId: number, updateData: Partial<User>): Promise<User> {
    if (updateData.rating !== undefined) {
      this.validateRating(updateData.rating);
    }

    const response = await fetch(`${this.url}/${userId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updateData),
    });

    if (!response.ok) {
      throw new Error('Failed to update user');
    }

    return await response.json();
  }

  async editUser(user: User): Promise<User> {
    if (user.rating !== undefined) {
      this.validateRating(user.rating);
    }

    const response = await fetch(`${this.url}/${user.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
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
      headers: { 'Content-Type': 'application/json' },
    });

    if (!response.ok) {
      throw new Error('Failed to delete user');
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
        headers: { 'Content-Type': 'application/json' },
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

  async updateMultiplePlayerRatings(
    userRatingUpdates: { userId: number; ratingChange: number }[]
  ): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/update-multiple-ratings`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          playerRatingUpdates: userRatingUpdates.map((update) => ({
            userId: update.userId,
            ratingChange: update.ratingChange,
          })),
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to update multiple player ratings');
      }

      return true;
    } catch (error) {
      console.error('Error updating multiple player ratings:', error);
      return false;
    }
  }

  async addPlayerOrganiserRelation(userId: number): Promise<void> {
    const organiserId = this.authService.getUserId();
    if (!organiserId) {
      throw new Error('User not authenticated or organiser ID not available');
    }

    const body = { userId, organiserId };
    const headers = this.getAuthHeaders();

    const response = await fetch(`${this.url}/player-organiser`, {
      method: 'POST',
      headers,
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

  async setMultiplePlayersAvailable(playerIds: number[]): Promise<boolean> {
    try {
      const response = await fetch(`${this.url}/set-multiple-available`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(playerIds),
      });

      if (!response.ok) {
        throw new Error('Failed to set multiple players available');
      }

      return true;
    } catch (error) {
      console.error('Error setting multiple players available:', error);
      return false;
    }
  }

  async delegateOrganizerRole(userId: number, friendUserId: number, notes?: string): Promise<OrganizerDelegateDto> {
    const headers = this.getAuthHeaders();
    const body: DelegateOrganizerRoleDto = { friendUserId, notes };
    
    const response = await fetch(`${this.url}/${userId}/delegate-organizer-role`, {
      method: 'POST',
      headers,
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(errorResponse.message || 'Failed to delegate organizer role');
    }

    return await response.json();
  }

  async reclaimOrganizerRole(userId: number, delegationId: number): Promise<boolean> {
    const headers = this.getAuthHeaders();
    const body: ReclaimOrganizerRoleDto = { delegationId };

    const response = await fetch(`${this.url}/${userId}/reclaim-organizer-role`, {
      method: 'POST',
      headers,
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(errorResponse.message || 'Failed to reclaim organizer role');
    }

    return true;
  }

  async getDelegationStatus(userId: number): Promise<DelegationStatusDto> {
    const headers = this.getAuthHeaders();
    
    const response = await fetch(`${this.url}/${userId}/delegation-status`, {
      method: 'GET',
      headers,
    });

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(errorResponse.message || 'Failed to get delegation status');
    }

    return await response.json();
  }
}
