import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { Layout } from './components/layout/layout';
import { Properties } from './components/properties/properties';
import { Users } from './components/users/users';
import { Logs } from './components/logs/logs';
import { AreaAnalysis } from './components/area-analysis/area-analysis';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      { path: 'properties', component: Properties },
      { path: 'users', component: Users },
      { path: 'logs', component: Logs },
      { path: 'area-analysis', component: AreaAnalysis },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
