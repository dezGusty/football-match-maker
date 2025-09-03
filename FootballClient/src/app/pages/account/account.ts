import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Header } from '../../components/header/header';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { User } from '../../models/user.interface';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatchService } from '../../services/match.service';
import { Match } from '../../models/match.interface';
import { UserRole } from '../../models/user-role.enum';
import { PlayerProfileImageService } from '../../services/player-profile-image.service';

@Component({
  selector: 'app-account',
  imports: [Header, CommonModule, FormsModule],
  templateUrl: './account.html',
  styleUrl: './account.css',
})
export class Account {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  user: User | null = null;
  userRole: UserRole | null = null;
  newPassword = '';
  confirmPassword = '';
  currentPassword = '';
  newUsername = '';
  usernamePassword = '';

  showPasswordForm = false;
  showUsernameForm = false;
  selectedFile: File | null = null;
  isUploadingImage = false;
  uploadError = '';

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private http: HttpClient,
    private playerProfileImageService: PlayerProfileImageService,
    private notificationService: NotificationService
  ) {
    this.userRole = this.authService.getUserRole();
    this.loadUser();
  }

  async loadUser() {
    try {
      const userId = this.authService.getUserId();
      if (userId) {
        this.user = await this.userService.getUserById(userId);
      }
    } catch (error) {
      console.error('Failed to load user:', error);
    }
  }

  togglePasswordForm() {
    this.showPasswordForm = !this.showPasswordForm;
    if (this.showPasswordForm) {
      this.showUsernameForm = false;
    }
    this.resetForms();
  }

  toggleUsernameForm() {
    this.showUsernameForm = !this.showUsernameForm;
    if (this.showUsernameForm) {
      this.showPasswordForm = false;
    }
    this.resetForms();
  }

  resetForms() {
    this.currentPassword = '';
    this.newPassword = '';
    this.confirmPassword = '';

    this.newUsername = '';
    this.usernamePassword = '';
  }

  async changePassword() {
    if (!this.user) return;
    try {
      const message = await this.userService.changePassword(
        this.user.id,
        this.currentPassword,
        this.newPassword,
        this.confirmPassword
      );
      this.notificationService.showSuccess(message);
      this.resetForms();
      this.showPasswordForm = false;
    } catch (error: any) {
      this.notificationService.showError(
        error.message || 'Password change failed'
      );
    }
  }

  async changeUsername() {
    if (!this.user) return;
    try {
      const message = await this.userService.changeUsername(
        this.user.id,
        this.newUsername,
        this.usernamePassword
      );
      this.notificationService.showSuccess(message);
      this.user.username = this.newUsername;
      this.resetForms();
      this.showUsernameForm = false;
    } catch (error: any) {
      this.notificationService.showError(
        error.message || 'Username change failed'
      );
    }
  }

  logout() {
    this.router.navigate(['/']);
    this.authService.logout();
  }

  getRoleString(role: UserRole): string {
    return UserRole[role];
  }

  UserRole = UserRole;

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.uploadError = '';

      const validation = this.playerProfileImageService.validateImageFile(
        this.selectedFile
      );
      if (!validation.isValid) {
        this.uploadError = validation.error || 'Invalid file';
        this.selectedFile = null;
        return;
      }

      this.uploadProfileImage();
    }
  }

  async uploadProfileImage() {
    if (!this.selectedFile || !this.user || !this.user.id) return;

    this.isUploadingImage = true;
    this.uploadError = '';

    try {
      const response = await this.playerProfileImageService
        .uploadProfileImage(this.user.id, this.selectedFile)
        .toPromise();

      if (response) {
        this.user.profileImageUrl = response.imageUrl;
        this.notificationService.showSuccess(
          'Profile image updated successfully!'
        );
      }
    } catch (error: any) {
      this.uploadError = error.error?.message || 'Failed to upload image';
      console.error('Upload error:', error);
    } finally {
      this.isUploadingImage = false;
      this.selectedFile = null;
      if (this.fileInput) {
        this.fileInput.nativeElement.value = '';
      }
    }
  }

  async deleteProfileImage() {
    if (!this.user || !this.user.id) return;

    if (!confirm('Are you sure you want to delete your profile image?')) {
      return;
    }

    try {
      await this.playerProfileImageService
        .deleteProfileImage(this.user.id)
        .toPromise();
      this.user.profileImageUrl =
        'http://localhost:5145/assets/default-avatar.png';
      this.notificationService.showSuccess(
        'Profile image deleted successfully!'
      );
    } catch (error: any) {
      this.notificationService.showError(
        error.error?.message || 'Failed to delete image'
      );
      console.error('Delete error:', error);
    }
  }

  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }
}
