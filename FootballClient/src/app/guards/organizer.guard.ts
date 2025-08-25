import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user-role.enum';

export const organizerGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const userRole = authService.getUserRole();
  
  // Allow ORGANISER and ADMIN roles to access organizer dashboard
  if (userRole === UserRole.ORGANISER || userRole === UserRole.ADMIN) {
    return true;
  }

  // If player role, redirect to player dashboard
  if (userRole === UserRole.PLAYER) {
    router.navigate(['/player-dashboard']);
    return false;
  }

  // If no valid role, redirect to login
  router.navigate(['/login']);
  return false;
};