import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  styleUrls: ['./header.css'],
})
export class Header implements OnInit {
  username: string = '';
  isMenuOpen = false;

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
    }
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
    this.isMenuOpen = false;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
    this.isMenuOpen = false;
  }
}
