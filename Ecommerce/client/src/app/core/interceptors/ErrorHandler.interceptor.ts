import type { HttpEvent, HttpHandler, HttpInterceptor,  HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error.status === 404) this.router.navigate(['/not-found']);
        if (error.status === 500) this.router.navigate(['/server-error']);
        if (error.status === 401) this.router.navigate(['/unauthorized']);
        return throwError(() => new Error(error));
      })
    );
  }
}
