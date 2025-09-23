import { Routes } from '@angular/router';
import { authGuard } from './components/auth/auth.guard';
import { playerGuard } from './guards/player.guard';
import { organizerGuard } from './guards/organizer.guard';
import { roleRedirectGuard } from './guards/role-redirect.guard';
import { adminGuard } from './guards/admin.guard';

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
    path: 'matches-history',
    loadComponent: () =>
      import('./pages/matches-history/matches-history').then(
        (m) => m.MatchesHistory
      ),
    canActivate: [authGuard],
  },
  {
    path: 'rating-evolution',
    loadComponent: () =>
      import('./pages/rating-evolution/rating-evolution.component').then(
        (m) => m.RatingEvolutionComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'manage-accounts',
    loadComponent: () =>
      import('./pages/manage-accounts/manage-accounts.component').then(
        (m) => m.ManageAccountsComponent
      ),
    canActivate: [authGuard, organizerGuard],
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
    path: 'player-dashboard-availableMatches',
    loadComponent: () =>
      import(
        './pages/player-dashboard-availableMatches/player-dashboard-availableMatches.component'
      ).then((m) => m.PlayerDashboardAvailableMatchesComponent),
    canActivate: [authGuard, playerGuard],
  },
  {
    path: 'past-matches',
    loadComponent: () =>
      import('./pages/past-matches/past-matches.component').then(
        (m) => m.PastMatches
      ),
    canActivate: [authGuard],
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
    path: 'manage-players',
    loadComponent: () =>
      import('./pages/manage-players/manage-players').then(
        (m) => m.ManagePlayersComponent
      ),
    canActivate: [authGuard, organizerGuard],
  },
  {
    path: 'home',
    redirectTo: '/',
    pathMatch: 'full',
  },

  {
    path: 'user-impersonation',
    loadComponent: () =>
      import(
        './components/user-impersonation/user-impersonation.component'
      ).then((m) => m.UserImpersonationComponent),
    canActivate: [authGuard, adminGuard],
  },
];
