import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.interface';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserRole } from '../../models/user-role.enum';
import { PlayerHeaderComponent } from '../../components/player-header/player-header.component';
import { PlayerProfileImageService } from '../../services/player-profile-image.service';
import { PlayerService } from '../../services/player.service';
import { Player } from '../../models/player.interface';
@Component({
  selector: 'app-player-account',
  standalone: true,
  imports: [CommonModule, FormsModule, PlayerHeaderComponent],
  templateUrl: './player-account.component.html',
  styleUrl: './player-account.component.css',
})
export class PlayerAccountComponent {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  user: User | null = null;
  player: Player | null = null;

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
    private playerService: PlayerService
  ) {
    this.loadUser();
  }

  async loadUser() {
    try {
      const userId = this.authService.getUserId();
      if (userId) {
        this.user = await this.userService.getUserById(userId);
        if (this.user && this.user.role === UserRole.PLAYER) {
          await this.loadPlayerData();
        }
      }
    } catch (error) {
      console.error('Failed to load user:', error);
    }
  }

  async loadPlayerData() {
    try {
      if (this.user?.email) {
        const players = await this.playerService.getPlayers();
        this.player =
          players.find((p: Player) => p.userEmail === this.user?.email) || null;
      }
    } catch (error) {
      console.error('Failed to load player data:', error);
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
        this.usernamePassword
      );
      alert(message);
      this.user.username = this.newUsername;
      this.resetForms();
      this.showUsernameForm = false;
    } catch (error: any) {
      alert(error.message || 'Username change failed');
    }
  }

  getRoleString(role: UserRole): string {
    return UserRole[role];
  }

  goBack() {
    this.router.navigate(['/player-dashboard']);
  }

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
    if (!this.selectedFile || !this.player || !this.player.id) return;

    this.isUploadingImage = true;
    this.uploadError = '';

    try {
      const response = await this.playerProfileImageService
        .uploadProfileImage(this.player.id, this.selectedFile)
        .toPromise();

      if (response) {
        this.player.profileImageUrl = response.imageUrl;
        alert('Profile image updated successfully!');
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
    if (!this.player || !this.player.id) return;

    if (!confirm('Are you sure you want to delete your profile image?')) {
      return;
    }

    try {
      await this.playerProfileImageService
        .deleteProfileImage(this.player.id)
        .toPromise();
      this.player.profileImageUrl =
        'http://localhost:5145/assets/default-avatar.png';
      alert('Profile image deleted successfully!');
    } catch (error: any) {
      alert(error.error?.message || 'Failed to delete image');
      console.error('Delete error:', error);
    }
  }

  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }
}
