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
import Swal from 'sweetalert';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterModule, NgxSpinnerModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  constructor(
    private authenticationService: AuthenticationService,
    private spinner: NgxSpinnerService,
    private router: Router
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
        Swal({
          title: 'Login Successful!',
          text: 'You logged in successfully!',
          icon: 'success',
        }).then(() => {
          this.router.navigateByUrl('/home');
        });
      },
      error: (error) => {
        this.spinner.hide();
        Swal({
          title: 'Login Failed',
          text: error.error?.msg || 'An error occurred during login',
          icon: 'error',
        });
      },
    });
  }

  OnGoogleLogin() {
    this.authenticationService.googleLogin();
  }

  get password() {
    return this.loginForm.controls['password'];
  }
  get email() {
    return this.loginForm.controls['email'];
  }
}
