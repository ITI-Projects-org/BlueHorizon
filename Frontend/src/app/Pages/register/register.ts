import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import { HttpClient } from '@angular/common/http';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RegisterDTO } from '../../Models/register-dto';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgxSpinnerModule], // Add NgxSpinnerModule
  templateUrl: './register.html',
  styleUrls: ['./register.css'],
})
export class Register {
  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService,
    private spinner: NgxSpinnerService,
    private router: Router
  ) {}
  registerForm = new FormGroup(
    {
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
      confirmpassword: new FormControl('', [Validators.required]),
      username: new FormControl('', [Validators.required]),
      role: new FormControl('Tenant'),
    },
    {
      validators: (control: AbstractControl) => {
        return this.passwordsMatch(control as FormGroup);
      },
    }
  );

  registerDTO!: RegisterDTO;

  Register() {
    console.log(this.registerForm.value);
    this.registerDTO = {
      email: this.email.value,
      username: this.username.value,
      password: this.password.value,
      confirmPassword: this.confirmpassword.value,
      role: this.role.value,
    };
    // this.registerDTO = {...this.registerForm.value};
    this.spinner.show();
    this.authenticationService.register(this.registerDTO).subscribe({
      next: () => {
        this.spinner.hide();
        Swal.fire({
          title: 'Registration Successful!',
          text: 'A confirmation email was sent, please check it to login',
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
          title: 'Registration Failed',
          text: error.error?.msg || 'An error occurred during registration',
          icon: 'error',
          draggable: true,
        });
      },
    });
  }

  googleSignup(role: string) {
    this.authenticationService.googleSignup(role as 'Tenant' | 'Owner');
  }

  private passwordsMatch(group: FormGroup) {
    const pass = group.get('password')?.value;
    const confirm = group.get('confirmpassword')?.value;
    return pass === confirm ? null : { mismatch: true };
  }

  get username() {
    return this.registerForm.controls['username'];
  }
  get password() {
    return this.registerForm.controls['password'];
  }
  get email() {
    return this.registerForm.controls['email'];
  }
  get role() {
    return this.registerForm.controls['role'];
  }
  get confirmpassword() {
    return this.registerForm.controls['confirmpassword'];
  }
}
