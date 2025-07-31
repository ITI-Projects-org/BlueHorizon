import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const tenantGuardGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const role = localStorage.getItem('role');
  if (role === 'tenant' || role === 'Tenant') {
    return true;
  }
  router.navigate(['/home']);
  return false;
};
