import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, retry, throwError } from 'rxjs';

export const errorHandlingInterceptor: HttpInterceptorFn = (req, next) => {
  let router = inject(Router);

  return next(req).pipe(
    retry({ count: 1, delay: 1000 }),
    catchError((error) => {
      if (error.status === 404) router.navigate(['/not-found']);
      if (error.status === 500) router.navigate(['/server-error']);
      if (error.status === 401) router.navigate(['/unauthorized']);
      if (error.status === 0) router.navigate(['/no-internet']);
      return throwError(() => new Error(error));
    })
  );
};
