import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user-role.enum';

export const organizerGuard = async () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const userRole = authService.getUserRole();
  
  // If no valid role, redirect to login
  if (userRole === null || userRole === undefined) {
    router.navigate(['/login']);
    return false;
  }

  // If player role, check if they are actually a delegated organizer
  if (userRole === UserRole.PLAYER) {
    const isDelegatedOrganizer = await authService.isDelegatedOrganizer();
    if (isDelegatedOrganizer) {
      // Delegated organizers should go to player dashboard, not organizer dashboard
      router.navigate(['/player-dashboard']);
      return false;
    }
    // Regular players should go to player dashboard
    router.navigate(['/player-dashboard']);
    return false;
  }

  // For ORGANISER and ADMIN roles, allow access (no more delegation check needed since delegated organizers are now PLAYER role)
  if (userRole === UserRole.ORGANISER || userRole === UserRole.ADMIN) {
    return true;
  }

  // Default redirect to login for any other case
  router.navigate(['/login']);
  return false;
};