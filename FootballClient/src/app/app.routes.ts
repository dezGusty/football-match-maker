import { Routes } from '@angular/router';
import { authGuard } from './components/auth/auth.guard';
import { playerGuard } from './guards/player.guard';
import { organizerGuard } from './guards/organizer.guard';
import { roleRedirectGuard } from './guards/role-redirect.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [roleRedirectGuard],
    children: [],
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./pages/reset-password/reset-password').then(
        (m) => m.SetPasswordComponent
      ),
  },

  {
    path: 'account',
    loadComponent: () =>
      import('./pages/account/account').then((m) => m.Account),
    canActivate: [authGuard],
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register').then((m) => m.Register),
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
  },
  {
    path: 'match-formation',
    loadComponent: () =>
      import('./pages/match-formation/match-formation.component').then(
        (m) => m.MatchFormationComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'matches-history',
    loadComponent: () =>
      import('./pages/matches-history/matches-history').then(
        (m) => m.MatchesHistory
      ),
    canActivate: [authGuard],
  },
  {
    path: 'player-dashboard',
    loadComponent: () =>
      import('./pages/player-dashboard/player-dashboard.component').then(
        (m) => m.PlayerDashboardComponent
      ),
    canActivate: [authGuard, playerGuard],
  },
  {
    path: 'organizer-dashboard',
    loadComponent: () =>
      import('./pages/organizer-dashboard/organizer-dashboard.component').then(
        (m) => m.OrganizerDashboardComponent
      ),
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'home',
    redirectTo: '/',
    pathMatch: 'full',
  },
  {
    path: 'player-account',
    loadComponent: () =>
      import('./pages/player-account/player-account.component').then(
        (m) => m.PlayerAccountComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'create-match',
    loadComponent: () =>
      import('./pages/create-match/create-match.component').then(
        (m) => m.CreateMatchComponent
      ),
    canActivate: [authGuard],
  },
];
