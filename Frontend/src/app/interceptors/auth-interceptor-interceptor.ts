import { isPlatformBrowser } from '@angular/common';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { AuthenticationService } from '../Services/authentication-service';
import { catchError, switchMap, throwError, finalize } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);
  const authService = inject(AuthenticationService);

  // Skip auth for login, register, and refresh endpoints
  if (
    req.url.includes('/Login') ||
    req.url.includes('/Register') ||
    req.url.includes('/refresh-token') ||
    req.url.includes('/forgot-password') ||
    req.url.includes('/reset-password')
  ) {
    return next(req);
  }

  // Only add the header in the browser
  if (isPlatformBrowser(platformId)) {
    const accessToken = authService.getAccessToken();

    if (accessToken) {
      // Check if token is expired
      if (authService.isTokenExpired(accessToken)) {
        console.log('Access token expired, attempting refresh...');
        // Token is expired, try to refresh it
        return authService.refreshToken().pipe(
          switchMap((tokenResponse) => {
            console.log('Token refreshed successfully, retrying request');
            // Use the new access token for the original request
            const clonedReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${tokenResponse.accessToken}`,
              },
            });
            return next(clonedReq);
          }),
          catchError((error) => {
            console.error('Token refresh failed in interceptor:', error);
            // Refresh failed, clear tokens and redirect to login
            authService.clearTokens();
            window.location.href = '/login';
            return throwError(() => error);
          })
        );
      } else {
        // Token is still valid, use it
        const clonedReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${accessToken}`,
          },
        });
        return next(clonedReq);
      }
    }
  }

  // No token or not in browser, proceed without auth header
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle 401 errors (token might have expired during the request)
      if (error.status === 401 && isPlatformBrowser(platformId)) {
        console.log('Got 401 error, attempting token refresh...');
        const refreshToken = authService.getRefreshToken();
        if (refreshToken) {
          // Try to refresh the token
          return authService.refreshToken().pipe(
            switchMap((tokenResponse) => {
              console.log('Token refreshed after 401, retrying request');
              // Retry the original request with new token
              const clonedReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${tokenResponse.accessToken}`,
                },
              });
              return next(clonedReq);
            }),
            catchError((refreshError) => {
              console.error('Token refresh failed on 401:', refreshError);
              // Refresh failed, clear tokens and redirect
              authService.clearTokens();
              window.location.href = '/login';
              return throwError(() => error);
            })
          );
        } else {
          // No refresh token, clear everything and redirect
          console.log('No refresh token available, redirecting to login');
          authService.clearTokens();
          window.location.href = '/login';
        }
      }
      return throwError(() => error);
    })
  );
};
