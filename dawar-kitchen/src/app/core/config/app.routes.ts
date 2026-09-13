import { Routes } from '@angular/router';
import { HomeComponent } from '../../features/home/pages/home/home.component';
import { authGuard } from '../guards/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent, data: { preload: true } },
  {
    path: 'menu',
    loadComponent: () => import('../../features/menu/pages/menu/menu.component').then(m => m.MenuPageComponent),
    data: { preload: true }  // ✅ High priority - preload after home
  },
  {
    path: 'reservations',
    loadComponent: () => import('../../features/reservations/pages/reservations/reservations.component').then(m => m.ReservationsPageComponent),
    data: { preload: true }  // ✅ High priority
  },
  {
    path: 'about',
    loadComponent: () => import('../../features/static/pages/about/about.component').then(m => m.AboutPageComponent)
  },
  {
    path: 'contact',
    loadComponent: () => import('../../features/contact/pages/contact/contact.component').then(m => m.ContactPageComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('../../features/auth/pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('../../features/auth/pages/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'checkout',
    loadComponent: () => import('../../features/checkout/pages/checkout/checkout.component').then(m => m.CheckoutComponent)
  },
  {
    path: 'order-confirmed',
    loadComponent: () => import('../../features/orders/pages/order-confirmed/order-confirmed.component').then(m => m.OrderConfirmedComponent),
    canActivate: [authGuard]
  },
  {
    path: 'payment-success',
    loadComponent: () => import('../../features/checkout/pages/payment-success/payment-success.component').then(m => m.PaymentSuccessComponent)
  },
  {
    path: 'payment-cancelled',
    loadComponent: () => import('../../features/checkout/pages/payment-cancelled/payment-cancelled.component').then(m => m.PaymentCancelledComponent)
  },
  {
    path: 'privacy',
    loadComponent: () => import('../../features/static/pages/privacy/privacy.component').then(m => m.PrivacyPageComponent)
  },
  {
    path: 'terms',
    loadComponent: () => import('../../features/static/pages/terms/terms.component').then(m => m.TermsPageComponent)
  },
  {
    path: '**',
    loadComponent: () => import('../../features/static/pages/not-found/not-found.component').then(m => m.NotFoundComponent)
  }
];
