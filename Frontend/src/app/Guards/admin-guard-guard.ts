import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const adminGuardGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const role = localStorage.getItem('role');
  if (role === 'admin' || role === 'Admin') {
    return true;
  }
  router.navigate(['/home']);
  return false;
};
