import { Injectable, inject } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ApiService } from '@core/http/api.service';
import { CreateOrderRequest, CreateOrderResponse } from '../models/order.model';

/**
 * OrderService: Encapsulates order workflow logic
 * - Manages order state and tracking
 * - Coordinates realtime order status updates
 * - Provides order confirmation data
 */
@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly api = inject(ApiService);

  private readonly currentOrder$ = new BehaviorSubject<CreateOrderResponse | null>(null);
  private readonly orderLoading$ = new BehaviorSubject<boolean>(false);

  /**
   * Submit order (final step in checkout flow)
   */
  submitOrder(data: CreateOrderRequest): Observable<CreateOrderResponse> {
    this.orderLoading$.next(true);
    return new Observable(observer => {
      this.api.createOrder(data).subscribe({
        next: (order) => {
          this.currentOrder$.next(order);
          this.orderLoading$.next(false);
          observer.next(order);
          observer.complete();
        },
        error: (err) => {
          this.orderLoading$.next(false);
          observer.error(err);
        }
      });
    });
  }

  /**
   * Get current order being tracked
   */
  getCurrentOrder(): Observable<CreateOrderResponse | null> {
    return this.currentOrder$;
  }

  /**
   * Get current order synchronously (for template access)
   */
  getCurrentOrderSync(): CreateOrderResponse | null {
    return this.currentOrder$.value;
  }

  /**
   * Clear current order (e.g., on logout or new order)
   */
  clearCurrentOrder(): void {
    this.currentOrder$.next(null);
  }

  /**
   * Get loading state
   */
  getLoading(): Observable<boolean> {
    return this.orderLoading$;
  }
}
