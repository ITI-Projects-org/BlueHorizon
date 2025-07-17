import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import { HttpClient } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterDTO } from '../../Models/register-dto';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule], // Add CommonModule for ngClass
  templateUrl: './register.html',
  standalone: true,
  styleUrl: './register.css'
})
export class Register implements OnInit{

  constructor(private http:HttpClient, private authenticationService:AuthenticationService){}
  registerForm = new FormGroup({
    email: new FormControl('',[Validators.required,Validators.email]),
    password: new FormControl('',[Validators.required]),
    confirmpassword: new FormControl('',[Validators.required]),
    username: new FormControl('',[Validators.required]),
    role:new FormControl('Tenant')
  });

  ngOnInit(): void {
    // this.authenticationService.register().subscribe({
    //   next:(res)=>{
    //     console.log(res);
    //   }
  // });
  }
  
  registerDTO!:RegisterDTO;

  Submit(){
    console.log(this.registerForm.value);
    this.registerDTO=
    {
      email: this.email.value,
      username: this.username.value,
      password: this.password.value,
      confirmPassword: this.confirmpassword.value,
      role : this.role.value,
    }
    // this.registerDTO = {...this.registerForm.value};
    this.authenticationService.register(this.registerDTO).subscribe();
  }


  get username() {
    return this.registerForm.controls['username']
  }
  get password(){
    return this.registerForm.controls["password"] 
  }
   get email(){
    return this.registerForm.controls["email"]
  }
  get role(){
    return this.registerForm.controls["role"]
  }
  get confirmpassword(){
    return this.registerForm.controls["confirmpassword"]
  }
  
}
