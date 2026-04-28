import { Routes } from '@angular/router';
import { MainLayout } from './layouts/main-layout/main-layout';
import { Dashboard } from './features/dashboard/dashboard';
import { Notifications } from './features/notifications/notifications';
import { LoginPage } from './features/authentication/login/login.page';
import { RegisterPage } from './features/authentication/register/register.page';

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
    ]
  },
   { path: '', redirectTo: 'authentication/login', pathMatch: 'full' },
  { path: '**', redirectTo: 'authentication/login' }
];
