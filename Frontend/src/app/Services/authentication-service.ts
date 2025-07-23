import { RouterOutlet } from '@angular/router';
import {
  HttpClient,
  HttpHeaderResponse,
  HttpHeaders,
} from '@angular/common/http';
import { LoginDTO } from '../Models/loginDTO';
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { RegisterDTO } from '../Models/register-dto';
import { isPlatformBrowser } from '@angular/common';
import { ForgetPasswordRequest } from '../Models/forget-password-request';
import { ResetPasswordRequestDTO } from '../Models/reset-password-request-dto';
import { ChangePasswordRequestDTO } from '../Models/change-password-request-dto';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  loginDTO!: LoginDTO;
  registerDTO!: RegisterDTO;
  authUrl: string = 'https://localhost:7083/api/Authentication';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  getUserName(token: string): string | undefined {
    try {
      const decoded: any = jwtDecode(token);
      const userNameClaim =
        decoded['username'] ||
        decoded[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/username'
        ];
      return userNameClaim?.toString();
    } catch {
      return 'Guest';
    }
  }
  getRole(token: string): string | undefined {
    try {
      const decoded: any = jwtDecode(token);
      const roleClaim =
        decoded['role'] ||
        decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      if (Array.isArray(roleClaim)) {
        return roleClaim[0];
      }
      return roleClaim?.toString();
    } catch {
      return '';
    }
  }

  getUserId(token: string): string | undefined {
    try {
      const decoded: any = jwtDecode(token);
      const userIdClaim =
        decoded['userId'] ||
        decoded[
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
        ];
      return userIdClaim?.toString();
    } catch {
      return '';
    }
  }

  getAccessToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('accessToken');
    }
    return null;
  }

  getRefreshToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('refreshToken');
    }
    return null;
  }

  clearTokens(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('role');
      localStorage.removeItem('username');
      localStorage.removeItem('userId');
    }
  }

  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }
  // Check if token is expired
  isTokenExpired(token: string): boolean {
    try {
      const decoded: any = jwtDecode(token);
      const currentTime = Date.now() / 1000;
      return decoded.exp < currentTime;
    } catch {
      return true; // If we can't decode, consider it expired
    }
  }

  googleSignup(role: 'Tenant' | 'Owner'): void {
    window.location.href = `${this.authUrl}/google-signup/${role}`;
  }

  googleLogin(): void {
    window.location.href = `${this.authUrl}/google-login`;
  }

  forgotPassword(request: ForgetPasswordRequest): Observable<any> {
    return this.http.post<any>(`${this.authUrl}/forgot-password`, request);
  }

  resetPassword(request: ResetPasswordRequestDTO): Observable<any> {
    return this.http.post<any>(`${this.authUrl}/reset-password`, request);
  }

  changePassword(request: ChangePasswordRequestDTO): Observable<any> {
    return this.http.post<any>(`${this.authUrl}/change-password`, request);
  }

  // Refresh token method
  refreshToken(): Observable<{ accessToken: string; refreshToken: string }> {
    const refreshToken = this.getRefreshToken();
    const accessToken = this.getAccessToken();
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    return this.http
      .post<{ accessToken: string; refreshToken: string }>(
        `${this.authUrl}/refresh-token`,
        { accessToken, refreshToken }
      )
      .pipe(
        tap((res) => {
          if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem('accessToken', res.accessToken);
            localStorage.setItem('refreshToken', res.refreshToken);
            localStorage.setItem(
              'role',
              this.getRole(res.accessToken)?.toString() ?? ''
            );
            localStorage.setItem(
              'username',
              this.getUserName(res.accessToken)?.toString() ?? 'Guest'
            );
            localStorage.setItem(
              'userId',
              this.getUserId(res.accessToken)?.toString() ?? ''
            );
          }
        })
      );
  }

  login(
    loginDTO: LoginDTO
  ): Observable<{ accessToken: string; refreshToken: string }> {
    let observable;
    // delete this
    // this.loginDTO = {
    //   email: 'ElSabagh@gmail.com',
    //   role: 'Owner',
    //   password: '123',
    //   username: 'Mohamed_ElSabagh',

    // };

    return this.http
      .post<{ accessToken: string; refreshToken: string }>(
        `${this.authUrl}/Login`,
        loginDTO
      )
      .pipe(
        tap((res) => {
          if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem('accessToken', res.accessToken);
            localStorage.setItem('refreshToken', res.refreshToken);
            localStorage.setItem(
              'role',
              this.getRole(res.accessToken)?.toString() ?? ''
            );
            localStorage.setItem(
              'username',
              this.getUserName(res.accessToken)?.toString() ?? 'Guest'
            );
            localStorage.setItem(
              'userId',
              this.getUserId(res.accessToken)?.toString() ?? ''
            );
          }
        })
      );
  }

  register(registerDTO: RegisterDTO): Observable<any> {
    // this.registerDTO = {
    //   email: 'ElSabagh2@gmail.com',
    //   role: 'Admin',
    //   password: '123',
    //   username: 'Mohamed_ElSabagh',
    //   confirmPassword: '123',
    // };
    return this.http
      .post<any>(`${this.authUrl}/Register`, registerDTO, {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      })
      .pipe(
        tap((res) => {
          console.log(res);
        })
      );
  }
}
