import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from './account.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);
  if (accountService.isAuthenticated()) {
    const request = req.clone({
      setHeaders: {
        Authorization: `${accountService.authorizationHeaderValue}`
      }
    });
    console.log("🚀 ~ authInterceptor ~ request:", request)
    return next(request);
  } else {
    return next(req);
  }
};
