import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LoginDTO } from '../../Models/loginDTO';
import { AuthenticationService } from './../../Services/authentication-service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {
  constructor(private authenticationService: AuthenticationService){}
  
  loginDTO!:LoginDTO;

  ngOnInit(): void {}
  loginForm = new FormGroup({
    email: new FormControl('',[Validators.required,Validators.email]),
    username: new FormControl('',[Validators.required]),
    password: new FormControl('',[Validators.required]),
    role: new FormControl('',[Validators.required]),
  });

  onLogin(){
    // console.log(this.loginForm.value)
    this.loginDTO={
      email:this.email.value ,
      username: this.username.value,
      password: this.password.value,
      // role: this.role.value,
    }
    this.authenticationService.login(this.loginDTO).subscribe();
  }
  
  get username() {
    return this.loginForm.controls['username']
  }
  get password(){
    return this.loginForm.controls["password"] 
  }
   get email(){
    return this.loginForm.controls["email"]
  }
  get role(){
    return this.loginForm.controls["role"]
  }
}
