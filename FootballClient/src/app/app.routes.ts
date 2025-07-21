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
        ///canActivate: [authGuard]
    },
    {
        path: 'select-players',
        loadComponent: () => import('./select-players.component').then(m => m.SelectPlayersComponent)
    }

];
