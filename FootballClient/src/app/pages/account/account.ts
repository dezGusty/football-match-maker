import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Header } from '../../components/header/header';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../components/auth/auth.service';
import { User } from '../../models/user.interface';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatchService } from '../../services/match.service';
import { PlayerService } from '../../services/player.service';
import { Match } from '../../models/match.interface';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-account',
  imports: [Header, CommonModule, FormsModule],
  templateUrl: './account.html',
  styleUrl: './account.css'
})
export class Account {
  user: User | null = null;
  newPassword = '';
  confirmPassword = '';
  currentPassword = '';
  images: string[] = [];
  selectedImage: string = '';
  showImageSelector = false;
  futureMatches: Match[] = [];

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private http: HttpClient,
    private matchService: MatchService,
    private playerService: PlayerService
  ) {
    this.loadUser();
    this.loadImages();
    this.loadFutureMatches();
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

  async loadFutureMatches() {
    try {
      this.futureMatches = await this.matchService.getFutureMatches();
    } catch (error) {
      console.error('Failed to load future matches:', error);
    }
  }

  async beginScheduledMatch(match: Match) {
    try {
      // Obțin jucătorii pentru meciul programat
      const playerIds = await this.matchService.getPlayersForScheduledMatch(match.id!);

      // Setez jucătorii ca disponibili
      await this.playerService.setMultiplePlayersAvailable(playerIds);

      // Redirecționez la select-players
      this.router.navigate(['/select-players']);
    } catch (error) {
      console.error('Failed to begin scheduled match:', error);
      alert('Failed to begin scheduled match. Please try again.');
    }
  }

  loadImages() {
    this.http.get<string[]>('http://localhost:5145/api/images').subscribe({
      next: (imgs) => {
        this.images = imgs;
        console.log('Loaded images:', this.images); // vezi dacă e gol sau nu
      },
      error: () => this.images = []
    });
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
      this.currentPassword = '';
      this.newPassword = '';
      this.confirmPassword = '';
    } catch (error: any) {
      alert(error.message || 'Password change failed');
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

  logout() {
    this.router.navigate(['/']);
    this.authService.logout();
  }

  getRoleString(role: UserRole): string {
    return UserRole[role];
  }
  UserRole = UserRole;
}
