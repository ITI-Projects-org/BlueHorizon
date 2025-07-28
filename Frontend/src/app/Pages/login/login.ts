import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { LoginDTO } from '../../Models/loginDTO';
import { AuthenticationService } from './../../Services/authentication-service';
import Swal from 'sweetalert2';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
import { Verification } from '../../Services/verification-service';
import { ForgetPasswordRequest } from '../../Models/forget-password-request';
import e from 'express';
@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterModule, NgxSpinnerModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  // forgetRequest: ForgetPasswordRequest = { email: '' }; // Initialize the object
  constructor(
    private authenticationService: AuthenticationService,
    private spinner: NgxSpinnerService,
    private router: Router,
    private verificationService: Verification
  ) {}

  loginDTO!: LoginDTO;

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });
  onLogin() {
    // console.log(this.loginForm.value)
    this.loginDTO = {
      email: this.email.value,
      password: this.password.value,
      // role: this.role.value,
      
    };
    console.log(this.loginDTO);
    this.spinner.show();
    this.authenticationService.login(this.loginDTO).subscribe({
      next: () => {
        this.spinner.hide();
        Swal.fire({
          title: 'Login Successful!',
          text: 'You logged in successfully!',
          icon: 'success',
          draggable: true,
        }).then(() => {
          const role = this.authenticationService.getRole(
            localStorage.getItem('accessToken') ?? ''
          );
          let verified;
          this.verificationService.isVerified().subscribe({
            next: (res) => {
              console.log(res.isVerified);
              verified = res.isVerified;

              console.log('from login.ts' + verified);
              // console.log(verified)

              if (role === 'Owner' && !verified)
                this.router.navigateByUrl('/VerifyOwner');
              else this.router.navigateByUrl('/home');
              // ++ guard
            },
          });
        });
        this.router.navigateByUrl('/home');
      },
      error: (error) => {
        this.spinner.hide();
        Swal.fire({
          title: 'Login Failed',
          text: error.error?.msg || 'An error occurred during login',
          icon: 'error',
          draggable: true,
        });
      },
    });
  }

  OnGoogleLogin() {
    this.authenticationService.googleLogin();
  }

  OnForgotPassword() {
    if (this.email.value) {
      const forgetRequest: ForgetPasswordRequest = {
        email: this.email.value ?? '',
      };
      console.log(forgetRequest);
      this.spinner.show();
      this.authenticationService.forgotPassword(forgetRequest).subscribe({
        next: (res) => {
          this.spinner.hide();
          Swal.fire({
            title: 'Success',
            text: res.msg,
            icon: 'success',
            draggable: true,
          });
        },
        error: (e) => {
          this.spinner.hide();
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'An un excpected error ocurred',
          });
          console.log(e.error?.msg);
        },
      });
    } else {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'You have to enter your email first to reset your password',
      });
    }
  }
  get password() {
    return this.loginForm.controls['password'];
  }
  get email() {
    return this.loginForm.controls['email'];
  }
}
