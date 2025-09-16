import { Component } from '@angular/core';
import { Header } from '../../components/header/header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.interface';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/user-role.enum';
import { NotificationService } from '../../services/notification.service';
import { StatSelector } from '../../components/stat-selector/stat-selector';
import { PlayerStatsComponent } from '../../components/player-stats.component/player-stats.component';

@Component({
  selector: 'app-manage-accounts',
  standalone: true,
  imports: [
    Header,
    FormsModule,
    CommonModule,
    StatSelector,
    PlayerStatsComponent,
  ],
  templateUrl: './manage-accounts.component.html',
  styleUrls: ['./manage-accounts.component.css'],
})
export class ManageAccountsComponent {
  constructor(
    private UserService: UserService,
    private authService: AuthService,
    private notificationService: NotificationService
  ) {}

  users: User[] = [];
  filteredUsers: User[] = [];
  userLoading = false;
  userErrorMessage = '';
  userSuccessMessage = '';
  showAddModal = false;

  newUser = {
    firstName: '',
    lastName: '',
    username: '',
    email: '',
    role: UserRole.PLAYER,
    rating: 5,
    speed: 2,
    stamina: 2,
    errors: 2,
    isDeleted: false,
  };

  editIndex: number | null = null;
  editedUser: User | null = null;

  showDeleteConfirmDialog = false;
  userToDelete: number | null = null;
  showReactivateConfirmDialog = false;
  userToReactivate: number | null = null;

  searchTerm: string = '';
  selectedRole: number | 'all' = 'all';
  UserRole = UserRole;

  async init() {
    const role = this.authService.getUserRole();

    if (role === UserRole.ADMIN) {
      this.users = await this.UserService.getAllUsers();
      const id = this.authService.getUserId();
      this.users = this.users.filter((user) => user.id !== id);
      this.applyFilters();
    }
  }

  ngOnInit() {
    this.init();
  }

  isUserEnabled(user: User): boolean {
    return !user.isDeleted;
  }

  getRoleString(role: UserRole): string {
    switch (role) {
      case UserRole.ADMIN:
        return 'Admin';
      case UserRole.ORGANISER:
        return 'Organizer';
      case UserRole.PLAYER:
        return 'Player';
    }
  }

  onSearchChange() {
    this.applyFilters();
  }

  onRoleFilterChange() {
    this.applyFilters();
  }

  private applyFilters() {
    let filtered = [...this.users];

    if (this.searchTerm) {
      filtered = filtered.filter(
        (user) =>
          user.firstName
            ?.toLowerCase()
            .includes(this.searchTerm.toLowerCase()) ||
          user.lastName
            ?.toLowerCase()
            .includes(this.searchTerm.toLowerCase()) ||
          user.username
            ?.toLowerCase()
            .includes(this.searchTerm.toLowerCase()) ||
          user.email?.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }

    if (this.selectedRole !== 'all') {
      const roleFilter = Number(this.selectedRole);
      filtered = filtered.filter((user) => user.role === roleFilter);
    }

    filtered.sort((a, b) => {
      const aActive = this.isUserEnabled(a);
      const bActive = this.isUserEnabled(b);

      if (aActive && !bActive) return -1;
      if (!aActive && bActive) return 1;
      return 0;
    });

    this.filteredUsers = filtered;
  }

  setEditIndex(index: number) {
    this.editIndex = index;
    this.editedUser = { ...this.filteredUsers[index] };
  }

  async editUser() {
    if (!this.editedUser) return;

    if (
      typeof this.editedUser.rating === 'number' &&
      (this.editedUser.rating < 0 || this.editedUser.rating > 10)
    ) {
      alert('Rating must be between 0 and 10.');
      return;
    }

    try {
      if (this.editedUser.role !== undefined) {
        this.editedUser.role = Number(this.editedUser.role);
      }

      const updatedUser = await this.UserService.editUser(this.editedUser);
      const userIndex = this.users.findIndex((u) => u.id === updatedUser.id);
      if (userIndex !== -1) {
        this.users[userIndex] = updatedUser;
      }
      const filteredIndex = this.filteredUsers.findIndex(
        (u) => u.id === updatedUser.id
      );
      if (filteredIndex !== -1) {
        this.filteredUsers[filteredIndex] = updatedUser;
      }
      this.clearEditIndex();
      this.notificationService.showSuccess('User updated successfully!');
    } catch (error) {
      console.error('Error updating user:', error);
    }
  }

  showDeleteConfirmation(userId: number) {
    this.userToDelete = userId;
    this.showDeleteConfirmDialog = true;
  }

  showReactivateConfirmation(userId: number) {
    this.userToReactivate = userId;
    this.showReactivateConfirmDialog = true;
  }

  cancelDelete() {
    this.showDeleteConfirmDialog = false;
    this.userToDelete = null;
  }

  cancelReactivate() {
    this.showReactivateConfirmDialog = false;
    this.userToReactivate = null;
  }

  async deleteUser(userId: number) {
    this.showDeleteConfirmDialog = false;

    try {
      const success = await this.UserService.deleteUser(userId);
      if (success) {
        const userIndex = this.users.findIndex((u) => u.id === userId);
        if (userIndex !== -1) {
          this.users[userIndex].isDeleted = true;
        }
        const filteredIndex = this.filteredUsers.findIndex(
          (u) => u.id === userId
        );
        if (filteredIndex !== -1) {
          this.filteredUsers[filteredIndex].isDeleted = true;
        }
        this.applyFilters();
        this.notificationService.showSuccess('User deleted successfully!');
      }
    } catch (error) {
      console.error('Error deleting user:', error);
      this.notificationService.showError(
        'Failed to delete user. Please try again.'
      );
    } finally {
      this.userToDelete = null;
    }
  }

  async enableUser(userId: number) {
    this.showReactivateConfirmDialog = false;

    try {
      const success = await this.UserService.reactivateUser(userId);
      if (success) {
        const userIndex = this.users.findIndex((u) => u.id === userId);
        if (userIndex !== -1) {
          this.users[userIndex].isDeleted = false;
        }
        const filteredIndex = this.filteredUsers.findIndex(
          (u) => u.id === userId
        );
        if (filteredIndex !== -1) {
          this.filteredUsers[filteredIndex].isDeleted = false;
        }
        this.applyFilters();
        this.notificationService.showSuccess('User reactivated successfully!');
      }
    } catch (error) {
      console.error('Error reactivating user:', error);
      this.notificationService.showError(
        'Failed to reactivate user. Please try again.'
      );
    }
  }

  clearEditIndex() {
    this.editIndex = null;
    this.editedUser = null;
  }

  openAddUserModal() {
    this.showAddModal = true;
    this.resetNewUser();
  }

  closeAddUserModal() {
    this.showAddModal = false;
    this.resetNewUser();
  }

  resetNewUser() {
    this.newUser = {
      firstName: '',
      lastName: '',
      username: '',
      email: '',
      role: UserRole.PLAYER,
      rating: 5,
      speed: 2,
      stamina: 2,
      errors: 2,
      isDeleted: false,
    };
  }

  async addUser() {
    try {
      if (
        !this.newUser.firstName ||
        !this.newUser.lastName ||
        !this.newUser.username ||
        !this.newUser.email
      ) {
        this.notificationService.showError(
          'Please fill in all required fields.'
        );
        return;
      }

      const existingUser = this.users.find(
        (u) =>
          u.username === this.newUser.username || u.email === this.newUser.email
      );

      if (existingUser) {
        this.notificationService.showError('Username or email already exists.');
        return;
      }

      this.userLoading = true;

      const userToCreate = {
        firstName: this.newUser.firstName!,
        lastName: this.newUser.lastName!,
        username: this.newUser.username!,
        email: this.newUser.email!,
        rating: this.newUser.rating || 5,
        speed: this.newUser.speed || 2,
        stamina: this.newUser.stamina || 2,
        errors: this.newUser.errors || 2,
        role: Number(this.newUser.role) || 2,
      };

      const createdUser = await this.UserService.addUser(userToCreate);

      if (createdUser) {
        this.users.push(createdUser);
        this.applyFilters();
        this.notificationService.showSuccess('User created successfully!');
        this.closeAddUserModal();
      }
    } catch (error: any) {
      console.error('Error creating user:', error);
      this.notificationService.showError(
        error.message || 'Failed to create user. Please try again.'
      );
    } finally {
      this.userLoading = false;
    }
  }
}
