import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
  const expiresAt = parseInt(localStorage.getItem('authExpiresAt') || '0', 10);
  const now = new Date().getTime();

  if (now < expiresAt) {
    return true;
  }
}

  router.navigate(['/']);
  authService.logout();
  return false;
}; 