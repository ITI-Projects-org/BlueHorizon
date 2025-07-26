// src/app/Services/auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt'; // تأكد من وجود هذا الـ import

@Injectable({
  providedIn: 'root' // 🔴🔴🔴 هذا السطر يجب أن يكون موجوداً
})
export class AuthService {
  private baseUrl = 'https://localhost:7083/api/Authentication';

  constructor(private http: HttpClient, private router: Router, private jwtHelper: JwtHelperService) {}

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Login`, credentials).pipe(
      tap((response: any) => {
        if (response && response.accessToken) {
          localStorage.setItem('accessToken', response.accessToken);
          console.log('Access Token saved:', response.accessToken);

          if (response.userId) {
            localStorage.setItem('userId', response.userId);
            console.log('User ID saved from login response:', response.userId);
          } else {
            const decodedPayload = this.decodeTokenPayload(response.accessToken);
            const extractedUserId = decodedPayload?.nameid || decodedPayload?.sub;
            if (extractedUserId) {
              localStorage.setItem('userId', extractedUserId);
              console.log('User ID extracted from token and saved:', extractedUserId);
            } else {
              console.warn('User ID not found in login response or JWT token payload.');
            }
          }
        } else {
          console.warn('Login successful but no access token found in response.');
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('userId');
    console.log('Access Token and User ID removed. User logged out.');
    this.router.navigate(['/login']);
  }

  getCurrentUserId(): string | null {
    return localStorage.getItem('userId');
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    return token !== null && !this.jwtHelper.isTokenExpired(token);
  }

  getCurrentUserName(): string | null {
    const token = this.getToken();
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      const decodedToken = this.jwtHelper.decodeToken(token);
      return decodedToken?.username || decodedToken?.name || null;
    }
    return null;
  }

  private decodeTokenPayload(token: string): any | null {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      return JSON.parse(jsonPayload);
    } catch (e) {
      console.error('Error parsing token payload:', e);
      return null;
    }
  }
}
