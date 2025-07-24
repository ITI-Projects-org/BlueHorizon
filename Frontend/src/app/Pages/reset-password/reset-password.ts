import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import { ActivatedRoute, Router } from '@angular/router';
import { ResetPasswordRequestDTO } from '../../Models/reset-password-request-dto';
import {
  FormControl,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { error } from 'console';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
@Component({
  selector: 'app-reset-password',
  imports: [CommonModule, ReactiveFormsModule, NgxSpinnerModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css',
})
export class ResetPassword implements OnInit {
  resetRequest: ResetPasswordRequestDTO = {
    email: '',
    token: '',
    newPassword: '',
  };

  newPasswordForm = new FormGroup({
    password: new FormControl('', Validators.required),
  });

  constructor(
    private authService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute,
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const encodedToken = params['token'];
      const encodedEmail = params['email'];
      if (encodedToken && encodedEmail) {
        const resetToken = decodeURIComponent(encodedToken);
        const email = decodeURIComponent(encodedEmail);
        this.resetRequest.email = email;
        this.resetRequest.token = resetToken;
      }
    });
  }

  get password() {
    return this.newPasswordForm.controls['password'];
  }

  resetPassword() {
    this.resetRequest.newPassword = this.password.value ?? '';
    this.spinner.show();
    this.authService.resetPassword(this.resetRequest).subscribe({
      next: (res) => {
        this.spinner.hide();
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
        this.spinner.hide();
        Swal.fire({
          title: 'Reset Password Failed',
          text: error.error?.msg || 'An error occurred during registration',
          icon: 'error',
          draggable: true,
        });
      },
    });
  }
}
