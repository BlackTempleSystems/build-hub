import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthenticationService } from '../services';

export const authenticationGuard: CanActivateFn = () => {
    const authenticationService = inject(AuthenticationService);
    const router = inject(Router);

    if (authenticationService.userData)
        return true;

    router.navigate(['/authentication/login'])
    return false;
};