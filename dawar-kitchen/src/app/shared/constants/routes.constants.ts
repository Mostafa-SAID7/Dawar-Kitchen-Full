/**
 * routes.constants.ts
 * All application route paths in one place — no more scattered string literals.
 */
export const ROUTES = {
  HOME:              '/',
  MENU:              '/menu',
  RESERVATIONS:      '/reservations',
  ABOUT:             '/about',
  CONTACT:           '/contact',
  LOGIN:             '/login',
  REGISTER:          '/register',
  CHECKOUT:          '/checkout',
  ORDER_CONFIRMED:   '/order-confirmed',
  PAYMENT_SUCCESS:   '/payment-success',
  PAYMENT_CANCELLED: '/payment-cancelled',
  PRIVACY:           '/privacy',
  TERMS:             '/terms',
  NOT_FOUND:         '**',
} as const;

export type AppRoute = typeof ROUTES[keyof typeof ROUTES];
