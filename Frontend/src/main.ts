import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app'; // تأكد أن هذا هو اسم الـ Root Component بتاعك
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';

// تأكد من وجود هذا السطر في بداية الملف
import 'zone.js';

bootstrapApplication(App, {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideRouter(routes),
    // لا تضع هنا أي providers لـ JwtHelperService أو JWT_OPTIONS أو APP_INITIALIZER
    // لأننا نتعامل معها يدوياً في AuthService
  ]
});
