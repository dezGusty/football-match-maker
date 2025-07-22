import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Home } from './home/home';
import { SelectPlayersComponent } from './selectPlayers/select-players.component';
import { MatchFormationComponent } from './match-formation/match-formation.component';
import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: 'login', component: Login },
    { path: 'home', component: Home, canActivate: [authGuard] },
    { path: 'select-players', component: SelectPlayersComponent, canActivate: [authGuard] },
    { path: 'match-formation', component: MatchFormationComponent, canActivate: [authGuard] }
];
