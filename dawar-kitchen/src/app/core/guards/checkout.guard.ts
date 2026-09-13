import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../../features/checkout/services/cart.service';
import { ToastService } from '../../shared/services/toast.service';
import { TranslateService } from '@ngx-translate/core';

/**
 * Route guard — redirects users with an empty cart away from checkout.
 */
export const checkoutGuard: CanActivateFn = () => {
  const cart      = inject(CartService);
  const router    = inject(Router);
  const toast     = inject(ToastService);
  const translate = inject(TranslateService);

  if (!cart.isEmpty()) {
    return true;
  }

  // Notify the user that their cart is empty
  toast.info(translate.instant('cart.empty'));
  
  router.navigate(['/menu']);
  return false;
};
