import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('token');

    if (token) {
      console.log("AuthGuard: Token found. Allowing access.");
      return true;
    } else {
      console.log("AuthGuard: No token found. Redirecting to login.");
      this.router.navigate(['/login']);
      return false;
    }
  }
}
