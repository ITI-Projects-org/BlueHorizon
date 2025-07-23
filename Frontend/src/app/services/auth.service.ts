import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators'; // استورد tap عشان نحفظ التوكن بعد اللوجين

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // تأكد إن الـ URL ده هو الـ Base URL الخاص بالـ API بتاعك لعمليات الـ Authentication
  private baseUrl = 'https://localhost:7083/api/Authentication';

  constructor(private http: HttpClient, private router: Router) {}

  /**
   * دالة لتسجيل دخول المستخدم.
   * بتقوم بإرسال بيانات الاعتماد (credentials) للباك إند.
   * بعد الاستجابة الناجحة، بتقوم بحفظ التوكن في الـ Local Storage.
   * @param credentials كائن يحتوي على بيانات تسجيل الدخول (مثلاً: { username, password })
   * @returns Observable يحتوي على استجابة الـ API (غالباً كائن به التوكن)
   */
  login(credentials: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Login`, credentials).pipe(
      // استخدام 'tap' لمتابعة الاستجابة وحفظ التوكن
      tap((response: any) => {
        if (response && response.token) {
          localStorage.setItem('token', response.token); // 👈 حفظ التوكن هنا بإسم 'token'
          console.log('Token saved:', response.token);
        } else {
          console.warn('Login successful but no token found in response.');
        }
      })
    );
  }

  /**
   * دالة لتسجيل خروج المستخدم.
   * بتقوم بحذف التوكن من الـ Local Storage وبتوجه المستخدم لصفحة تسجيل الدخول.
   */
  logout() {
    localStorage.removeItem('token'); // ✅ حذف التوكن من المفتاح 'token'
    console.log('Token removed. User logged out.');
    this.router.navigate(['/login']);
  }

  /**
   * دالة للتحقق مما إذا كان المستخدم مسجلاً للدخول.
   * بتتحقق من وجود التوكن في الـ Local Storage.
   * @returns boolean true لو التوكن موجود، false لو مش موجود.
   */
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token'); // ✅ التحقق من وجود التوكن في المفتاح 'token'
  }

  /**
   * دالة لاسترجاع الـ JWT Token من الـ Local Storage.
   * دي الدالة اللي SignalR service هتستخدمها عشان تحصل على التوكن.
   * @returns string | null الـ JWT Token أو null لو مش موجود.
   */
  public getToken(): string | null {
    return localStorage.getItem('token'); // 👈 تم توحيد اسم المفتاح إلى 'token'
  }

  /**
   * دالة لاستخراج الـ User ID من الـ JWT Token.
   * @returns string | null الـ User ID أو null لو التوكن مش موجود أو غير صالح.
   */
  getUserIdFromToken(): string | null {
    const token = localStorage.getItem('token'); // ✅ استخدام المفتاح 'token'
    if (!token) return null;

    try {
      // فك تشفير الجزء الأوسط من التوكن (payload)
      const payload = JSON.parse(atob(token.split('.')[1]));
      // 'nameid' هو الـ Claim Type الافتراضي لـ UserId في .NET Identity مع JWT
      // 'sub' هو اختصار لـ Subject، وهو غالباً بيمثل الـ User ID أيضاً.
      return payload?.nameid || payload?.sub || null;
    } catch (e) {
      console.error('Error decoding token:', e);
      return null;
    }
  }
}
