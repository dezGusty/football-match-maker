import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ImpersonationService } from '../../services/impersonation.service';
import { UserRole } from '../../models/user-role.enum';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Header } from '../header/header';

@Component({
  selector: 'app-user-impersonation',
  templateUrl: './user-impersonation.component.html',
  styleUrls: ['./user-impersonation.component.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, Header],
})
export class UserImpersonationComponent implements OnInit {
  users: any[] = [];
  filteredUsers: any[] = [];
  isImpersonating = false;
  impersonatedUser: any = null;
  searchTerm: string = '';
  roleFilter: string = 'all';
  isAdmin = false;

  constructor(
    private http: HttpClient,
    private impersonationService: ImpersonationService
  ) {}

  ngOnInit(): void {
    // Check if user is already impersonating
    this.impersonationService.isImpersonating$.subscribe((isImpersonating) => {
      this.isImpersonating = isImpersonating;
    });

    this.impersonationService.impersonatedUser$.subscribe((user) => {
      this.impersonatedUser = user;
    });

    // Get user role
    const userRole = Number(localStorage.getItem('userRole'));
    this.isAdmin = userRole === UserRole.ADMIN;

    if (this.isAdmin) {
      this.loadUsers();
    }
  }

  loadUsers(): void {
    this.http.get<any[]>(`${environment.apiUrl}/User/with-accounts`).subscribe(
      (data) => {
        // Filter out admin users - only show players and organizers
        this.users = data.filter((user) => user.role !== UserRole.ADMIN);
        this.applyFilters();
      },
      (error) => {
        console.error('Error loading users:', error);
      }
    );
  }

  applyFilters(): void {
    let filtered = [...this.users];

    // Apply role filter
    if (this.roleFilter !== 'all') {
      filtered = filtered.filter(
        (user) => user.role.toString() === this.roleFilter
      );
    }

    // Apply search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(
        (user) =>
          user.username.toLowerCase().includes(term) ||
          user.email.toLowerCase().includes(term) ||
          (user.firstName && user.firstName.toLowerCase().includes(term)) ||
          (user.lastName && user.lastName.toLowerCase().includes(term))
      );
    }

    this.filteredUsers = filtered;
  }

  startImpersonation(userId: number): void {
    this.impersonationService.startImpersonation(userId).subscribe(
      () => {
        // Handled by the service
      },
      (error) => {
        console.error('Error starting impersonation:', error);
      }
    );
  }

  stopImpersonation(): void {
    this.impersonationService.stopImpersonation().subscribe(
      () => {
        // Handled by the service
      },
      (error) => {
        console.error('Error stopping impersonation:', error);
      }
    );
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onRoleFilterChange(): void {
    this.applyFilters();
  }

  getRoleName(roleValue: number): string {
    switch (roleValue) {
      case UserRole.ADMIN:
        return 'Admin';
      case UserRole.ORGANISER:
        return 'Organizer';
      case UserRole.PLAYER:
        return 'Player';
      default:
        return 'Unknown';
    }
  }
}
