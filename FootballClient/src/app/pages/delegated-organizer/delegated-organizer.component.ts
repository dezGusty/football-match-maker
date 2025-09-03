import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Header } from '../../components/header/header';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { DelegationStatusDto } from '../../models/organizer-delegation.interface';
import { Router } from '@angular/router';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-delegated-organizer',
  standalone: true,
  imports: [CommonModule, Header],
  templateUrl: './delegated-organizer.component.html',
  styleUrls: ['./delegated-organizer.component.css'],
})
export class DelegatedOrganizerComponent implements OnInit {
  delegationStatus: DelegationStatusDto | null = null;
  loading = false;
  errorMessage = '';
  successMessage = '';
  showConfirmDialog = false;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  async ngOnInit() {
    await this.loadDelegationStatus();

    // If user is not delegated, redirect them appropriately
    if (!this.isDelegatedOrganizer()) {
      this.router.navigate(['/organizer-dashboard']);
    }
  }

  async loadDelegationStatus() {
    try {
      this.loading = true;
      const userId = this.authService.getUserId()!;
      this.delegationStatus = await this.userService.getDelegationStatus(
        userId
      );
    } catch (error) {
      console.error('Error loading delegation status:', error);
      this.errorMessage = 'Failed to load delegation status';
    } finally {
      this.loading = false;
    }
  }

  showReclaimConfirmation() {
    this.showConfirmDialog = true;
  }

  cancelReclaim() {
    this.showConfirmDialog = false;
  }

  async reclaimOrganizerRole() {
    if (!this.delegationStatus?.currentDelegation) return;

    this.showConfirmDialog = false; // Închide dialogul
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    try {
      const userId = this.authService.getUserId()!;
      await this.userService.reclaimOrganizerRole(
        userId,
        this.delegationStatus.currentDelegation.id
      );

      this.notificationService.showSuccess(
        'Organizer role reclaimed successfully! Redirecting...'
      );

      // Reload delegation status
      await this.loadDelegationStatus();

      // Redirect to organizer dashboard after a short delay
      setTimeout(() => {
        this.router.navigate(['/organizer-dashboard']);
      }, 2000);
    } catch (error: any) {
      this.notificationService.showError(
        error.message || 'Failed to reclaim organizer role'
      );
      console.error('Error reclaiming organizer role:', error);
    } finally {
      this.loading = false;
    }
  }

  isDelegatedOrganizer(): boolean {
    return this.delegationStatus?.isDelegating === true;
  }

  getDelegateName(): string {
    return (
      this.delegationStatus?.currentDelegation?.delegateUserName || 'Unknown'
    );
  }

  getDelegationDate(): string {
    if (!this.delegationStatus?.currentDelegation?.createdAt) return 'Unknown';
    return new Date(
      this.delegationStatus.currentDelegation.createdAt
    ).toLocaleDateString();
  }

  getDelegationNotes(): string {
    return (
      this.delegationStatus?.currentDelegation?.notes || 'No notes provided'
    );
  }
}
