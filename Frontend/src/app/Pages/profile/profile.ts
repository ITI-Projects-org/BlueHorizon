import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import Swal from 'sweetalert2';
import { NgxSpinnerService, NgxSpinnerModule } from 'ngx-spinner';
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
    private spinner: NgxSpinnerService
  ) {}
  ngOnInit(): void {
    this.authService.getProfile().subscribe({
      next: (res) => {
        this.email = res.email;
        this.username = res.username;
      },
      error: (e) => {
        console.log(e);
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
}
