// src/app/guards/auth-guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('token');
    // const userRole = localStorage.getItem('userRole'); // لم نعد نحتاجه هنا إذا أردنا السماح لأي دور

    if (token) { // ✅ فقط تحقق من وجود التوكن
      console.log("AuthGuard: Token found. Allowing access."); // لغرض الـ debugging
      return true; // اسمح بالدخول
    } else {
      console.log("AuthGuard: No token found. Redirecting to login."); // لغرض الـ debugging
      alert("ممكن تعمل تسجيل دخول ؟");
      this.router.navigate(['/login']);
      return false; // امنع الوصول للصفحة المحمية
    }
  }
}
