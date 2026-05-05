import { Routes } from '@angular/router';
import { LoginPage } from './features/authentication/login/login.page';
import { RegisterPage } from './features/authentication/register/register.page';
import { DashboardPage } from './features/dashboard/dashboard.page';

export const routes: Routes = [
  {
    path: 'authentication',
    children: [
      {
        path: 'login',
        component: LoginPage
      },
      {
        path: 'register',
        component: RegisterPage
      },
    ],
  },
  { path: 'dashboard', component: DashboardPage },
  { path: '', redirectTo: 'authentication/login', pathMatch: 'full' },
  { path: '**', redirectTo: 'authentication/login' }
];
