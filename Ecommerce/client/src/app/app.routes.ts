import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
    data: {
      breadcrumb: 'Home'
    }
  },
  {
    path: 'home',
    loadComponent: () => import('./home/home.component').then((x) => x.HomeComponent),
    data: {
      breadcrumb: { skip: true }
    }
  },
  {
    path: 'store',
    loadComponent: () => import('./store/store.component').then((x) => x.StoreComponent),
    data: {
      breadcrumb: 'Store'
    },
    children: [
      {
        path: '',
        loadComponent: () => import('./store/home-store/home-store.component').then((x) => x.HomeStoreComponent)
      },
      {
        path: ':id',
        loadComponent: () => import('./store/product-details/product-details.component').then((x) => x.ProductDetailsComponent),
        data: {
          breadcrumb: {
            alias: 'productDetail'
          }
        }
      }
    ]
  },
  {
    path: 'basket',
    loadComponent: () => import('./basket/basket.component').then((x) => x.BasketComponent),
    children: [
      {
        path: '',
        loadComponent: () => import('./basket/home-basket/home-basket.component').then((x) => x.HomeBasketComponent)
      }
    ]
  },
  {
    path: 'contact-us',
    loadComponent: () => import('./core/contact-us/contact-us.component').then((x) => x.ContactUsComponent)
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
