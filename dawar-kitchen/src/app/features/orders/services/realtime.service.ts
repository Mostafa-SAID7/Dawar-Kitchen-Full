import { Injectable } from '@angular/core';

/**
 * RealtimeService: WebSocket-based order/reservation status tracking
 * Stub for realtime order updates via WebSocket
 */
@Injectable({ providedIn: 'root' })
export class RealtimeService {
  subscribeToOrder(orderId: string, callback: (status: any) => void): void {
    // Stub: Would connect to WebSocket for order updates
  }

  unsubscribe(orderId: string, cleanup: boolean = false): void {
    // Stub: Would disconnect from WebSocket
  }
}
