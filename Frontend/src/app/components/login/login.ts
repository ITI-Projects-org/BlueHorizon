// src/app/components/login/login.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
  standalone: true,
  imports: [FormsModule, CommonModule]
})
export class LoginComponent {
  email = '';
  password = '';

  constructor(private authService: AuthService, private router: Router) {}

  login(): void {
    const credentials = {
      email: this.email,
      password: this.password
    };

    this.authService.login(credentials).subscribe({
      next: (response: any) => {
        console.log("Login successful! Token received:", response.token); // 👈 تأكيد استقبال التوكن
        localStorage.setItem('token', response.token);
        localStorage.setItem('userRole', response.roles && response.roles.length > 0 ? response.roles[0] : 'Unknown');

        // ✅ هذا هو الجزء الذي يجب تعديله لإضافة الـ debugging
        this.router.navigate(['/chat']).then(success => {
          if (success) {
            console.log("Navigation to /chat successful!"); // 👈 سيظهر لو التوجيه نجح
          } else {
            console.log("Navigation to /chat failed! (Router returned false)"); // 👈 سيظهر لو التوجيه فشل
          }
        }).catch(err => {
          console.error("Navigation error during router.navigate:", err); // 👈 سيظهر لو حدث خطأ أثناء التوجيه
        });
      },
      error: (err) => {
        console.error('Login error:', err);
        let errorMessage = 'Login failed. Please check your credentials.';
        if (err.error && err.error.errors) {
            errorMessage = Object.values(err.error.errors).flat().join('. ');
        } else if (err.error && typeof err.error === 'string') {
            errorMessage = err.error;
        } else if (err.error && err.error.title) {
            errorMessage = err.error.title;
        }
        alert(errorMessage);
      },
    });
  }
}
