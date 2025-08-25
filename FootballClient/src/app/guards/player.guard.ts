import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user-role.enum';

export const playerGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const userRole = authService.getUserRole();
  
  // Allow only PLAYER role to access player dashboard
  if (userRole === UserRole.PLAYER) {
    return true;
  }

  // If organizer/admin role, redirect to organizer dashboard
  if (userRole === UserRole.ORGANISER || userRole === UserRole.ADMIN) {
    router.navigate(['/organizer-dashboard']);
    return false;
  }

  // If no valid role, redirect to login
  router.navigate(['/login']);
  return false;
};