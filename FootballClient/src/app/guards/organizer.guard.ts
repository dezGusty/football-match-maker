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

  // If player role, redirect to player dashboard
  if (userRole === UserRole.PLAYER) {
    router.navigate(['/player-dashboard']);
    return false;
  }

  // For ORGANISER and ADMIN roles, check delegation status
  if (userRole === UserRole.ORGANISER || userRole === UserRole.ADMIN) {
    // Check if organizer is delegated
    const isDelegated = await authService.isDelegatedOrganizer();
    if (isDelegated) {
      // Redirect delegated organizers to special page
      router.navigate(['/delegated-organizer']);
      return false;
    }
    
    // Allow access for non-delegated organizers and admins
    return true;
  }

  // Default redirect to login for any other case
  router.navigate(['/login']);
  return false;
};