import { HttpInterceptorFn } from '@angular/common/http';
import { AuthenticationService } from '../services/authentication/authentication.service';
import { inject } from '@angular/core';

export const authenticationTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const authenticationuthenticationService = inject(AuthenticationService);
  const token = authenticationuthenticationService.getToken();

  if (token) {
    const clonedRequest = request.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });

    return next(clonedRequest);
  }

  return next(request);
};
