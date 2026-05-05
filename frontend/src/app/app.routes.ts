import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'orders/1001',
  },
  {
    path: 'orders/:id',
    loadComponent: () =>
      import('./features/orders/pages/order-edit-page.component').then(
        (component) => component.OrderEditPageComponent,
      ),
  },
  {
    path: '**',
    redirectTo: 'orders/1001',
  },
];
