import { Routes } from '@angular/router';
import { authGuard } from './components/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
  },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home').then((m) => m.Home),
    canActivate: [authGuard],
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./pages/reset-password/reset-password').then((m) => m.SetPasswordComponent),
  
  },
  {
    path: 'select-players',
    loadComponent: () =>
      import('./pages/selectPlayers/select-players.component').then(
        (m) => m.SelectPlayersComponent,
      ),
    canActivate: [authGuard],
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
        (m) => m.MatchFormationComponent,
      ),
    canActivate: [authGuard],
  },
  {
    path: 'matches-history',
    loadComponent: () =>
      import('./pages/matches-history/matches-history').then(
        (m) => m.MatchesHistory,
      ),
    canActivate: [authGuard],
  },
  {
    path: 'player-dashboard',
    loadComponent: () =>
      import('./pages/player-dashboard/player-dashboard.component').then(
        (m) => m.PlayerDashboardComponent,
      ),
    canActivate: [authGuard],
  },
  {
    path: 'player-account',
    loadComponent: () =>
      import('./pages/player-account/player-account.component').then(
        (m) => m.PlayerAccountComponent,
      ),
    canActivate: [authGuard],
  },
];
