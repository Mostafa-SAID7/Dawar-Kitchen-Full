import { Injectable, inject } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { ApiService } from '@core/http/api.service';
import { ToastService } from '@shared/services';
import { CreateCheckoutSessionRequest, CreateCheckoutSessionResponse, CreateOrderRequest, CreateOrderResponse } from '../models/order.model';

/**
 * CheckoutService: Encapsulates checkout business logic
 * - Manages checkout form state and validation
 * - Coordinates Stripe payment session creation
 * - Handles order submission
 * - Used by both checkout page and cart drawer
 */
@Injectable({ providedIn: 'root' })
export class CheckoutService {
  private readonly api = inject(ApiService);
  private readonly toast = inject(ToastService);

  private readonly checkoutInProgress$ = new Subject<boolean>();

  /**
   * Create Stripe checkout session for payment
   */
  createCheckoutSession(
    data: CreateCheckoutSessionRequest
  ): Observable<CreateCheckoutSessionResponse> {
    return this.api.createCheckoutSession(data);
  }

  /**
   * Submit order (after payment success or direct submission)
   */
  createOrder(data: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this.api.createOrder(data);
  }

  /**
   * Get checkout progress state (for multi-step flows)
   */
  getCheckoutProgress(): Observable<boolean> {
    return this.checkoutInProgress$;
  }

  /**
   * Set checkout progress state
   */
  setCheckoutProgress(inProgress: boolean): void {
    this.checkoutInProgress$.next(inProgress);
  }
}
