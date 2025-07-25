import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import Swal from 'sweetalert2';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
import { Router } from '@angular/router';
@Component({
  selector: 'app-profile',
  imports: [NgxSpinnerModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile implements OnInit {
  email: string = '';
  username: string = '';
  constructor(
    private authService: AuthenticationService,
    private spinner: NgxSpinnerService,
    private router: Router
  ) {}
  ngOnInit(): void {
    // Add debug info
    this.authService.logTokenInfo();

    this.authService.getProfile().subscribe({
      next: (res) => {
        console.log('Profile loaded successfully:', res);
        this.email = res.email;
        this.username = res.username;
      },
      error: (e) => {
        console.error('Profile loading failed:', e);
        console.log('Response status:', e.status);
        console.log('Response message:', e.message);
      },
    });
  }

  forgotPassowrd() {
    this.spinner.show();
    this.authService.forgotPassword({ email: this.email }).subscribe({
      next: (res) => {
        this.spinner.hide();
        Swal.fire({
          title: 'Success',
          text: res.msg,
          icon: 'success',
          draggable: true,
          confirmButtonText: 'Ok',
        });
      },
      error: (error) => {
        this.spinner.hide();
        console.log(error);
      },
    });
  }

  changePassowrd() {
    this.router.navigateByUrl('/change-password');
  }
}
