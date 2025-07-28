import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const ownerGuardGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const role = localStorage.getItem('role');
  if (role === 'owner' || role === 'Owner') {
    return true;
  }
  router.navigate(['/home']);
  return false;
};
