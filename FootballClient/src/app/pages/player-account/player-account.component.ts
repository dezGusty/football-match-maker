import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../components/auth/auth.service';
import { User } from '../../models/user.interface';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserRole } from '../../models/user-role.enum';
import { PlayerHeaderComponent } from '../../components/player-header/player-header.component';
@Component({
  selector: 'app-player-account',
  standalone: true,
  imports: [CommonModule, FormsModule, PlayerHeaderComponent],
  templateUrl: './player-account.component.html',
  styleUrl: './player-account.component.css',
})
export class PlayerAccountComponent {
  user: User | null = null;

  newPassword = '';
  confirmPassword = '';
  currentPassword = '';

  newUsername = '';
  usernamePassword = '';

  showPasswordForm = false;
  showUsernameForm = false;

  images: string[] = [];
  selectedImage: string = '';
  showImageSelector = false;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private http: HttpClient,
  ) {
    this.loadUser();
    this.loadImages();
  }

  async loadUser() {
    try {
      const userId = this.authService.getUserId();
      if (userId) {
        this.user = await this.userService.getUserWithImageById(userId);
      }
      if (this.user?.imageUrl) {
        this.selectedImage = this.user.imageUrl;
      }
    } catch (error) {
      console.error('Failed to load user:', error);
    }
  }

  loadImages() {
    this.http.get<string[]>('http://localhost:5145/api/images').subscribe({
      next: (imgs) => {
        this.images = imgs;
        console.log('Loaded images:', this.images);
      },
      error: () => (this.images = []),
    });
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
        this.confirmPassword,
      );
      alert(message);
      this.resetForms();
      this.showPasswordForm = false;
    } catch (error: any) {
      alert(error.message || 'Password change failed');
    }
  }

  async changeUsername() {
    if (!this.user) return;
    try {
      const message = await this.userService.changeUsername(
        this.user.id,
        this.newUsername,
        this.usernamePassword,
      );
      alert(message);
      this.user.username = this.newUsername;
      this.resetForms();
      this.showUsernameForm = false;
    } catch (error: any) {
      alert(error.message || 'Username change failed');
    }
  }

  async updateProfileImage() {
    if (!this.user || !this.selectedImage) return;
    try {
      await this.userService.updateUserImage(this.user.id, this.selectedImage);
      this.user.imageUrl = this.selectedImage;
      this.showImageSelector = false;
      alert('Profile image updated!');
    } catch (err: any) {
      alert(err.message || 'Failed to update image!');
    }
  }

  getRoleString(role: UserRole): string {
    return UserRole[role];
  }

  selectImage(img: string) {
    this.selectedImage = img;
  }

  goBack() {
    this.router.navigate(['/player-dashboard']);
  }
}
