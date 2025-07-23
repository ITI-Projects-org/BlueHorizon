import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('token');
    const userRole = localStorage.getItem('userRole');

    // لو التوكن موجود والدور صح
    if (token && userRole === 'User') {
      return true; // اسمح بالدخول
    } else {
      // لو مش موجودين:
      alert("ممكن تعمل تسجيل دخول ؟"); // 👈 هتظهر رسالة الـ alert
      this.router.navigate(['/login']); // 👈 **هنا بيتم التوجيه لصفحة اللوجين**
      return false; // امنع الوصول للصفحة المحمية
    }
  }
}
