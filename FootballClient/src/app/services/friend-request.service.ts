import { Injectable } from '@angular/core';
import {
  FriendRequest,
  CreateFriendRequest,
  FriendRequestResponse,
} from '../models/friend-request.interface';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class FriendRequestService {
  private baseUrl = 'http://localhost:5145/api/friendrequest';

  constructor(private authService: AuthService) {}

  private getAuthHeaders(): HeadersInit {
    const token = this.authService.getToken();
    return {
      'Content-Type': 'application/json',
      Authorization: token ? `Bearer ${token}` : '',
    };
  }

  async sendFriendRequest(receiverEmail: string): Promise<FriendRequest> {
    const response = await fetch(this.baseUrl, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify({ receiverEmail }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || 'Failed to send friend request');
    }

    return response.json();
  }

  async respondToFriendRequest(
    requestId: number,
    response: FriendRequestResponse
  ): Promise<FriendRequest> {
    const responseHttp = await fetch(`${this.baseUrl}/${requestId}/respond`, {
      method: 'PUT',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(response),
    });

    if (!responseHttp.ok) {
      const errorText = await responseHttp.text();
      throw new Error(errorText || 'Failed to respond to friend request');
    }

    return responseHttp.json();
  }

  async getSentRequests(): Promise<FriendRequest[]> {
    const response = await fetch(`${this.baseUrl}/sent`, {
      method: 'GET',
      headers: {
        Authorization: this.authService.getToken()
          ? `Bearer ${this.authService.getToken()}`
          : '',
      },
    });

    if (!response.ok) {
      console.error(
        'Failed to get sent requests:',
        response.status,
        response.statusText
      );
      throw new Error('Failed to get sent requests');
    }

    const data = await response.json();
    return data;
  }

  async getReceivedRequests(): Promise<FriendRequest[]> {
    const response = await fetch(`${this.baseUrl}/received`, {
      method: 'GET',
      headers: {
        Authorization: this.authService.getToken()
          ? `Bearer ${this.authService.getToken()}`
          : '',
      },
    });

    if (!response.ok) {
      console.error(
        'Failed to get received requests:',
        response.status,
        response.statusText
      );
      throw new Error('Failed to get received requests');
    }

    const data = await response.json();
    return data;
  }

  async getFriends(): Promise<FriendRequest[]> {
    const response = await fetch(`${this.baseUrl}/friends`, {
      method: 'GET',
      headers: {
        Authorization: this.authService.getToken()
          ? `Bearer ${this.authService.getToken()}`
          : '',
      },
    });

    if (!response.ok) {
      throw new Error('Failed to get friends');
    }

    return response.json();
  }

  async deleteFriendRequest(requestId: number): Promise<void> {
    const response = await fetch(`${this.baseUrl}/${requestId}`, {
      method: 'DELETE',
      headers: {
        Authorization: this.authService.getToken()
          ? `Bearer ${this.authService.getToken()}`
          : '',
      },
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || 'Failed to delete friend request');
    }
  }
}
