import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app'; // تأكد أن هذا هو اسم الـ Root Component بتاعك
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { JwtHelperService, JWT_OPTIONS } from '@auth0/angular-jwt'; // 🔴🔴🔴 إضافة JWT_OPTIONS

bootstrapApplication(App, {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideRouter(routes),

    // 🔴🔴�🔴🔴 توفير JWT_OPTIONS و JwtHelperService معاً 🔴🔴🔴🔴🔴
    {
      provide: JWT_OPTIONS,
      useValue: {
        tokenGetter: () => localStorage.getItem('accessToken'), // ميثود لجلب التوكن
        allowedDomains: ['localhost:7083'], // تأكد من الـ domain بتاع الـ Backend
        disallowedRoutes: [] // أي routes مش عايز الـ token يتبعتلها
      }
    },
    JwtHelperService // لازم توفرها هنا برضه
  ]
});

