import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-google-signup-fail',
  imports: [RouterLink],
  templateUrl: './google-signup-fail.html',
  styleUrl: './google-signup-fail.css',
})
export class GoogleSignupFail implements OnInit {
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
