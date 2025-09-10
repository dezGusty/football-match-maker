import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { UserRole } from '../../models/user-role.enum';
import { DelegationStatusDto } from '../../models/organizer-delegation.interface';
import { ImpersonationStatusComponent } from '../impersonation-status/impersonation-status.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, ImpersonationStatusComponent],
  templateUrl: './header.html',
  styleUrls: ['./header.css'],
})
export class Header implements OnInit {
  username: string = '';
  isMenuOpen = false;
  userRole: UserRole | null = null;
  delegationStatus: DelegationStatusDto | null = null;

  @Output() tabChange = new EventEmitter<string>();

  constructor(
    private router: Router,
    private authService: AuthService,
    private userService: UserService
  ) {}

  async ngOnInit() {
    const userId = this.authService.getUserId();
    if (userId) {
      const user = await this.userService.getUserById(userId);
      this.username = user.username;
      this.userRole = this.authService.getUserRole();

      if (this.isOrganizer()) {
        try {
          this.delegationStatus = await this.userService.getDelegationStatus(
            userId
          );
        } catch (error) {
          console.error('Error loading delegation status in header:', error);
        }
      }
    }
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
    this.isMenuOpen = false;
  }

  switchTab(tab: string) {
    this.tabChange.emit(tab);
    this.isMenuOpen = false;
  }

  // Navigate to account (unified for all roles)
  navigateToAccount() {
    this.router.navigate(['/account']);
    this.isMenuOpen = false;
  }

  // Navigate to dashboard based on user role
  navigateToDashboard() {
    if (this.userRole === UserRole.PLAYER) {
      this.router.navigate(['/player-dashboard']);
    } else if (
      this.userRole === UserRole.ORGANISER ||
      this.userRole === UserRole.ADMIN
    ) {
      this.router.navigate(['/organizer-dashboard']);
    }
    this.isMenuOpen = false;
  }

  // Check if user is organizer or admin
  isOrganizer(): boolean {
    return (
      this.userRole === UserRole.ORGANISER || this.userRole === UserRole.ADMIN
    );
  }

  // Check if user is player
  isPlayer(): boolean {
    return this.userRole === UserRole.PLAYER;
  }

  // Check if user is admin
  isAdmin(): boolean {
    return this.userRole === UserRole.ADMIN;
  }

  // Check delegation status
  isDelegating(): boolean {
    return !!this.delegationStatus?.isDelegating;
  }

  isDelegate(): boolean {
    return !!this.delegationStatus?.isDelegate;
  }

  getDelegationText(): string {
    if (this.isDelegating()) {
      return `Role delegated to ${this.delegationStatus?.currentDelegation?.delegateUserName}`;
    } else if (this.isDelegate()) {
      return `Acting for ${this.delegationStatus?.receivedDelegation?.originalOrganizerName}`;
    }
    return '';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
    this.isMenuOpen = false;
  }
}
