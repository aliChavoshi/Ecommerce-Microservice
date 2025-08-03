import { Routes } from '@angular/router';
import { StoreComponent } from './store/store.component';

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
    component: StoreComponent,
    children: [
      {
        path: 'detail/:id',
        loadComponent: () => import('./store/product-details/product-details.component').then((x) => x.ProductDetailsComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'home',
    pathMatch: 'full'
  }
];
