import { Injectable } from '@angular/core';
import { User } from '../models/user.interface';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly url = 'http://localhost:5145/api/user';

  async getUserById(id: number): Promise<User> {
    const response = await fetch(`${this.url}/${id}`, {
      headers: { 'Content-Type': 'application/json' }
    });
    if (!response.ok) {
      throw new Error('Failed to fetch user data');
    }
    return await response.json() as User;
  }

  async getUserWithImageById(id: number): Promise<User> {
    const response = await fetch(`${this.url}/${id}/with-image`, {
      headers: { 'Content-Type': 'application/json' }
    });
    if (!response.ok) {
      throw new Error('Failed to fetch user data');
    }
    return await response.json() as User;
  }

  async changePassword(userId: number, currentPassword: string, newPassword: string, confirmPassword: string): Promise<string> {
    const response = await fetch(`${this.url}/${userId}/change-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        currentPassword,
        newPassword,
        confirmPassword
      })
    });
    const result = await response.json();
    if (!response.ok) {
      throw new Error(result.message || 'Failed to change password');
    }
    return result.message || 'Password changed successfully';
  }
}
