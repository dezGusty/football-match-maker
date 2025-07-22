import { Injectable } from '@angular/core';
import { User } from '../models/user.interface';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly url = 'http://localhost:5145/api/user';

  async getUserById(id: number): Promise<User> {
    const response = await fetch(`${this.url}/${id}`, {
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include'
    });
    if (!response.ok) {
      throw new Error('Failed to fetch user data');
    }
    return await response.json() as User;
  }
}
