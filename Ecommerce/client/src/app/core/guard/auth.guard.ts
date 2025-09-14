import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../../account/account.service';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  console.log('🚀 ~ authGuard ~ state.url:', state.url);
  return accountService.currentUser$.pipe(
    map((auth) => {
      if (auth) return true;
      else {
        //TODO
        router.navigate(['/account/login'], { queryParams: { returnUrl: state.url, replaceUrl: true } });
        return false;
      }
    })
  );
};
