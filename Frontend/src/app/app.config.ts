import {
  APP_INITIALIZER,
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { routes } from './app.routes';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { MessageService } from 'primeng/api';
import { AccessTokenInterceptor, AuthenticationService } from './core';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(),
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {},
      },
    }),

    // NgRx global setup
    provideStore(),
    provideEffects(),
    provideStoreDevtools({ maxAge: 50 }),
    MessageService,
    provideHttpClient(withInterceptorsFromDi()),  // 👈 needed for class-based interceptors
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AccessTokenInterceptor,
      multi: true
    },
    provideAppInitializer(() => {
      //TODO System status here.

      const authenticationService = inject(AuthenticationService);
      return authenticationService.tryToAuthenticateUser();
    }),
  ]
};
