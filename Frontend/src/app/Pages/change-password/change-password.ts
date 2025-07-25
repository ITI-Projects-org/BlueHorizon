import { Component } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import { Router } from '@angular/router';
import { ChangePasswordRequestDTO } from '../../Models/change-password-request-dto';
import {
  FormControl,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { error } from 'console';
@Component({
  selector: 'app-change-password',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css',
})
export class ChangePassword {
  changePasswordReq: ChangePasswordRequestDTO = {
    currentPassword: '',
    newPassword: '',
  };

  changePasswordForm = new FormGroup({
    currentPassword: new FormControl('', Validators.required),
    newPassword: new FormControl('', Validators.required),
  });

  constructor(
    private authService: AuthenticationService,
    private router: Router
  ) {}

  get currentPassword() {
    return this.changePasswordForm.controls['currentPassword'];
  }

  get newPassword() {
    return this.changePasswordForm.controls['newPassword'];
  }
  changePassword() {
    this.changePasswordReq.currentPassword = this.currentPassword.value ?? '';
    this.changePasswordReq.newPassword = this.newPassword.value ?? '';

    this.authService.changePassword(this.changePasswordReq).subscribe({
      next: (res) => {
        Swal.fire({
          title: 'Success',
          text: res.msg,
          icon: 'success',
          draggable: true,
          confirmButtonText: 'Go to Login page',
        }).then(() => {
          this.router.navigateByUrl('/login');
        });
      },
      error: (error) => {
        Swal.fire({
          title: 'Changing Password Failed',
          text:
            error.error?.msg || 'An error occurred during changing password',
          icon: 'error',
          draggable: true,
        });
      },
    });
  }
}
