import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./login/login').then(m => m.Login)
    },
    {
        path: 'home',
        loadComponent: () => import('./home/home').then(m => m.Home)
    }
];
