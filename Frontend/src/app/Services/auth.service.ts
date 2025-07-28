// src/app/Services/auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable, Inject, PLATFORM_ID } from '@angular/core'; // Added Inject and PLATFORM_ID
import { isPlatformBrowser } from '@angular/common'; // Added isPlatformBrowser
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
// No JwtHelperService import as per previous decision

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = 'https://localhost:7083/api/Authentication';
  private isBrowser: boolean; // Property to store platform check result

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object // Inject PLATFORM_ID
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId); // Determine if running in browser
  }

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Login`, credentials).pipe(
      tap((response: any) => {
        if (this.isBrowser && response && response.accessToken) {
          // Guard localStorage access
          localStorage.setItem('accessToken', response.accessToken);
          console.log('Access Token saved:', response.accessToken);

          if (response.userId) {
            // Guard localStorage access
            localStorage.setItem('userId', response.userId);
            console.log('User ID saved from login response:', response.userId);
          } else {
            const decodedPayload = this.decodeTokenPayload(
              response.accessToken
            );
            const extractedUserId =
              decodedPayload?.nameid || decodedPayload?.sub;
            if (extractedUserId) {
              // Guard localStorage access
              localStorage.setItem('userId', extractedUserId);
              console.log(
                'User ID extracted from token and saved:',
                extractedUserId
              );
            } else {
              console.warn(
                'User ID not found in login response or JWT token payload.'
              );
            }
          }
        } else if (!this.isBrowser) {
          console.warn(
            'Skipping localStorage operations during SSR for login.'
          );
        } else {
          console.warn(
            'Login successful but no access token found in response.'
          );
        }
      })
    );
  }

  logout() {
    if (this.isBrowser) {
      // Guard localStorage access
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('role');
      localStorage.removeItem('username');
      localStorage.removeItem('userId');
      console.log('Access Token and User ID removed. User logged out.');
    } else {
      console.warn('Skipping localStorage operations during SSR for logout.');
    }
    this.router.navigate(['/login']);
  }

  getCurrentUserId(): string | null {
    if (this.isBrowser) {
      // Guard localStorage access
      return localStorage.getItem('userId');
    }
    return null; // Return null if not in browser
  }

  getToken(): string | null {
    if (this.isBrowser) {
      // Guard localStorage access
      return localStorage.getItem('accessToken');
    }
    return null; // Return null if not in browser
  }

  isTokenExpired(token: string): boolean {
    if (!token || !this.isBrowser) {
      // Guard localStorage access and check token
      return true;
    }
    const decoded = this.decodeTokenPayload(token);
    if (!decoded || !decoded.exp) {
      return true;
    }
    const expirationDate = new Date(0);
    expirationDate.setUTCSeconds(decoded.exp);

    return expirationDate.valueOf() < new Date().valueOf();
  }

  isLoggedIn(): boolean {
    const token = this.getToken(); // getToken already handles platform check
    return token !== null && !this.isTokenExpired(token);
  }

  getCurrentUserName(): string | null {
    const token = this.getToken(); // getToken already handles platform check
    if (token && !this.isTokenExpired(token)) {
      const decodedToken = this.decodeTokenPayload(token);
      return decodedToken?.username || decodedToken?.name || null;
    }
    return null;
  }

  private decodeTokenPayload(token: string): any | null {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
          })
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch (e) {
      console.error('Error parsing token payload:', e);
      return null;
    }
  }
}
