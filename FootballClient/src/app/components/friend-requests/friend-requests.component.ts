import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FriendRequestService } from '../../services/friend-request.service';
import { AuthService } from '../../services/auth.service';
import {
  FriendRequest,
  CreateFriendRequest,
  FriendRequestResponse,
} from '../../models/friend-request.interface';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-friend-requests',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './friend-requests.component.html',
  styleUrls: ['./friend-requests.component.css'],
})
export class FriendRequestsComponent implements OnInit {
  showModal = false;
  showSendRequestModal = false;
  activeTab = 'received';
  currentUserId: number | null = null;
  currentUserRole: UserRole | null = null;

  sentRequests: FriendRequest[] = [];
  receivedRequests: FriendRequest[] = [];
  friends: FriendRequest[] = [];

  selectedReceiverEmail: string = '';
  sendRequestError = '';
  sendRequestSuccess = '';

  errorMessage = '';
  successMessage = '';

  UserRole = UserRole;

  constructor(
    private friendRequestService: FriendRequestService,
    private authService: AuthService
  ) {}

  async ngOnInit() {
    this.currentUserId = this.authService.getUserId();
    this.currentUserRole = this.authService.getUserRole();

    await this.loadData();
  }

  async loadData() {
    try {
      await Promise.all([
        this.loadSentRequests(),
        this.loadReceivedRequests(),
        this.loadFriends(),
      ]);
    } catch (error: any) {
      console.error('Error loading data:', error);
      this.errorMessage = 'Failed to load data: ' + error.message;
    }
  }

  async loadSentRequests() {
    this.sentRequests = await this.friendRequestService.getSentRequests();
  }

  async loadReceivedRequests() {
    this.receivedRequests =
      await this.friendRequestService.getReceivedRequests();
  }

  async loadFriends() {
    this.friends = await this.friendRequestService.getFriends();
  }

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  openSendRequestModal() {
    this.showSendRequestModal = true;
    this.sendRequestError = '';
    this.sendRequestSuccess = '';
    this.selectedReceiverEmail = '';
  }

  closeSendRequestModal() {
    this.showSendRequestModal = false;
  }

  async sendFriendRequest() {
    if (!this.selectedReceiverEmail.trim()) {
      this.sendRequestError = 'Please enter a valid email address';
      return;
    }

    try {
      await this.friendRequestService.sendFriendRequest(
        this.selectedReceiverEmail
      );

      this.sendRequestSuccess = 'Friend request sent successfully!';
      this.sendRequestError = '';

      setTimeout(async () => {
        await this.loadData();
        this.closeSendRequestModal();
      }, 1500);
    } catch (error: any) {
      this.sendRequestError = error.message;
      this.sendRequestSuccess = '';
    }
  }

  async acceptRequest(request: FriendRequest) {
    try {
      const response: FriendRequestResponse = { accept: true };
      await this.friendRequestService.respondToFriendRequest(
        request.id,
        response
      );

      this.successMessage = `Accepted friend request from ${request.senderUsername}`;
      this.errorMessage = '';

      await this.loadData();
    } catch (error: any) {
      this.errorMessage = 'Failed to accept request: ' + error.message;
      this.successMessage = '';
    }
  }

  async rejectRequest(request: FriendRequest) {
    try {
      const response: FriendRequestResponse = { accept: false };
      await this.friendRequestService.respondToFriendRequest(
        request.id,
        response
      );

      this.successMessage = `Rejected friend request from ${request.senderUsername}`;
      this.errorMessage = '';

      await this.loadData();
    } catch (error: any) {
      this.errorMessage = 'Failed to reject request: ' + error.message;
      this.successMessage = '';
    }
  }

  async cancelRequest(request: FriendRequest) {
    try {
      await this.friendRequestService.deleteFriendRequest(request.id);

      this.successMessage = `Cancelled friend request to ${request.receiverUsername}`;
      this.errorMessage = '';

      await this.loadData();
    } catch (error: any) {
      this.errorMessage = 'Failed to cancel request: ' + error.message;
      this.successMessage = '';
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending':
        return 'orange';
      case 'Accepted':
        return 'green';
      case 'Rejected':
        return 'red';
      default:
        return 'gray';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  get pendingReceivedRequests(): FriendRequest[] {
    return this.receivedRequests.filter((r) => r.status !== 'Accepted');
  }

  get pendingSentRequests(): FriendRequest[] {
    return this.sentRequests.filter((r) => r.status !== 'Accepted');
  }

  get receivedCount(): number {
    return this.pendingReceivedRequests.length;
  }

  get sentCount(): number {
    return this.pendingSentRequests.length;
  }
}
