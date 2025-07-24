import { Routes } from '@angular/router';
import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./login/login').then(m => m.Login)
    },
    {
        path: 'home',
        loadComponent: () => import('./home/home').then(m => m.Home),
        canActivate: [authGuard]
    },
    {
        path: 'select-players',
        loadComponent: () => import('./selectPlayers/select-players.component').then(m => m.SelectPlayersComponent),
        canActivate: [authGuard]
    },
    {
        path: 'account',
        loadComponent: () => import('./account/account').then(m => m.Account),
        canActivate: [authGuard]
    },
    {
        path: 'register',
        loadComponent: () => import('./register/register').then(m => m.Register)
    },
    {
        path: 'match-formation',
        loadComponent: () => import('./match-formation/match-formation.component').then(m => m.MatchFormationComponent),
        canActivate: [authGuard]
    },
    {
        path: 'matches-history',
        loadComponent: () => import('./matches-history/matches-history').then(m => m.MatchesHistory),
        canActivate: [authGuard]
    }
];
