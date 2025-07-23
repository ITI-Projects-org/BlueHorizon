import { isPlatformBrowser } from '@angular/common';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { AuthenticationService } from '../Services/authentication-service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);
  const authService = inject(AuthenticationService);

  // Only add the header in the browser
  if (isPlatformBrowser(platformId)) {
    const accessToken = authService.getAccessToken();

    if (accessToken) {
      // Check if token is expired
      if (authService.isTokenExpired(accessToken)) {
        // Token is expired, try to refresh it
        return authService.refreshToken().pipe(
          switchMap((tokenResponse) => {
            // Use the new access token for the original request
            localStorage.setItem('accessToken', tokenResponse.accessToken);
            localStorage.setItem('refreshToken', tokenResponse.refreshToken);
            const clonedReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${tokenResponse.accessToken}`,
              },
            });
            return next(clonedReq);
          }),
          catchError((error) => {
            // Refresh failed, clear tokens and redirect to login
            authService.clearTokens();
            // You might want to redirect to login page here
            console.error('Token refresh failed:', error);
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
        const refreshToken = authService.getRefreshToken();
        if (refreshToken) {
          // Try to refresh the token
          return authService.refreshToken().pipe(
            switchMap((tokenResponse) => {
              localStorage.setItem('accessToken', tokenResponse.accessToken);
              localStorage.setItem('refreshToken', tokenResponse.refreshToken);
              // Retry the original request with new token
              const clonedReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${tokenResponse.accessToken}`,
                },
              });
              return next(clonedReq);
            }),
            catchError((refreshError) => {
              // Refresh failed, clear tokens
              authService.clearTokens();
              console.error('Token refresh failed on 401:', refreshError);
              return throwError(() => error);
            })
          );
        } else {
          // No refresh token, clear everything
          authService.clearTokens();
        }
      }
      return throwError(() => error);
    })
  );
};
