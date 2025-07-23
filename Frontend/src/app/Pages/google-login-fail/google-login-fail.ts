import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-google-login-fail',
  imports: [RouterLink],
  templateUrl: './google-login-fail.html',
  styleUrl: './google-login-fail.css',
})
export class GoogleLoginFail implements OnInit {
  errorMsg: string = '';
  constructor(private router: Router, private route: ActivatedRoute) {}
  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const encodedErrorMsg = params['msg'];
      if (encodedErrorMsg) {
        this.errorMsg = decodeURIComponent(encodedErrorMsg);
      }
    });
  }
}
