import { Injectable } from '@angular/core';
import { User } from '../models/user.interface';
import { HttpClient } from '@angular/common/http';
import { PlayerUser } from '../models/player-user.interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly url = 'http://localhost:5145/api/user';

  constructor(private http: HttpClient) {}

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

  async createPlayerUser(playerUser: PlayerUser): Promise<any> {
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
}
