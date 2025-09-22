import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user-role.enum';

export const roleRedirectGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const currentUrl = router.url;
  if (currentUrl.includes('/reset-password')) {
    return true;
  }

  // Check if user is authenticated
  if (!authService.isLoggedIn(false)) {
    router.navigate(['/login']);
    return false;
  }

  const userRole = authService.getUserRole();

  // Redirect based on user role
  if (userRole === UserRole.PLAYER) {
    router.navigate(['/player-dashboard']);
    return false;
  } else if (userRole === UserRole.ORGANISER || userRole === UserRole.ADMIN) {
    router.navigate(['/organizer-dashboard']);
    return false;
  } else {
    // If no valid role, redirect to login
    router.navigate(['/login']);
    return false;
  }
};
