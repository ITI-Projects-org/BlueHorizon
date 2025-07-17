import { AuthenticationService } from './../../Services/authentication-service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {
  constructor(private authenticationService: AuthenticationService){}
  
  ngOnInit(): void {  // change on init to onsubmit
    this.authenticationService.login().subscribe({
      next:(res)=>{
        console.log(res);
      }
    })
  }

}
