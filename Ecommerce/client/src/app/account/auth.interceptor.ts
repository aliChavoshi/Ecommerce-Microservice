import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from './account.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);

  const headers: Record<string, string> = {};
  if (accountService.isAuthenticated()) {
    headers['Authorization'] = accountService.authorizationHeaderValue;
  }

  const request = req.clone({ setHeaders: headers });
  return next(request);
};
