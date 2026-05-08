import { Routes } from '@angular/router';
import { LoginPage } from './features/authentication/login/login.page';
import { RegisterPage } from './features/authentication/register/register.page';
import { DashboardPage } from './features/dashboard/dashboard.page';
import { authenticationGuard } from './core/guards/authentication.guard';

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
  {
    path: '',
    canActivate: [authenticationGuard],
    children: [
      { path: 'dashboard', component: DashboardPage },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: '**', redirectTo: 'dashboard' }
    ]
  }
];
