import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'store',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('./home/home.component').then((x) => x.HomeComponent)
  },
  {
    path: 'contact-us',
    loadComponent: () => import('./core/contact-us/contact-us.component').then((x) => x.ContactUsComponent)
  },
  {
    path: 'store',
    loadComponent: () => import('./store/store.component').then((x) => x.StoreComponent),
    children: [
      {
        path: '',
        loadComponent: () => import('./store/home-store/home-store.component').then((x) => x.HomeStoreComponent)
      },
      {
        path: ':id',
        loadComponent: () => import('./store/product-details/product-details.component').then((x) => x.ProductDetailsComponent)
      }
    ]
  },
  {
    path: 'not-found',
    loadComponent: () => import('./core/not-found/not-found.component').then((x) => x.NotFoundComponent)
  },
  {
    path: 'server-error',
    loadComponent: () => import('./core/server-error/server-error.component').then((x) => x.ServerErrorComponent)
  },
  {
    path: 'unauthorized',
    loadComponent: () => import('./core/unauthorized/unauthorized.component').then((x) => x.UnauthorizedComponent)
  },
  {
    path: '**',
    redirectTo: 'home',
    pathMatch: 'full'
  }
];
