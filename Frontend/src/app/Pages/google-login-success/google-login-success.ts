import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-google-login-success',
  imports: [],
  templateUrl: './google-login-success.html',
  styleUrl: './google-login-success.css',
})
export class GoogleLoginSuccess implements OnInit {
  constructor(
    private authService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}
  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const encodedAccessToken = params['accessToken'];
      const encodedRefreshToken = params['refreshToken'];
      if (encodedAccessToken && encodedRefreshToken) {
        const accessToken = decodeURIComponent(encodedAccessToken);
        const refreshToken = decodeURIComponent(encodedRefreshToken);
        this.authService.handleGoogleLoginCallback(accessToken, refreshToken);
        this.router.navigateByUrl('/home');
      }
    });
  }
}
