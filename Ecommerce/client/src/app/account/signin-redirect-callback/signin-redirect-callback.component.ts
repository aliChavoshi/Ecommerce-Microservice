import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-signin-redirect-callback',
  imports: [],
  templateUrl: './signin-redirect-callback.component.html',
  styleUrl: './signin-redirect-callback.component.css'
})
export class SigninRedirectCallbackComponent implements OnInit {
  returnUrl?: string;
  constructor(private router: Router, private accountService: AccountService, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    //TODO
    this.activatedRoute.queryParams.subscribe((res) => {
      this.returnUrl = res['returnUrl'] ?? '';
    });
    this.accountService.finishLogin().then((x) => {
      console.log("🚀 ~ SigninRedirectCallbackComponent ~ ngOnInit ~ x:", x)
      //TODO : check this
      this.router.navigate(['/home', { replaceUrl: true }]);
    });
  }
}
