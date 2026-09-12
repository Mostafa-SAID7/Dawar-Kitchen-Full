import { Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private socket: WebSocket | null = null;
  private heartbeatInterval: any = null;
  private refCounter = 0;
  private isConnected = signal(false);

  // Active subscriptions: topic -> callback
  private listeners = new Map<string, (payload: any) => void>();

  connect(): void {
    if (this.socket) return;

    const isPlaceholder = !environment.supabaseUrl
      || environment.supabaseUrl.includes('your-project')
      || environment.supabaseUrl.includes('placeholder');

    if (isPlaceholder) return;

    const wsUrl = environment.supabaseUrl.replace('https://', 'wss://') + '/realtime/v1/websocket';

    if (!wsUrl) return;

    const fullUrl = `${wsUrl}?apikey=${environment.supabaseAnonKey}&vsn=1.0.0`;
    
    this.socket = new WebSocket(fullUrl);

    this.socket.onopen = () => {
      this.isConnected.set(true);
      this.startHeartbeat();
      // Re-join any active topics
      for (const topic of this.listeners.keys()) {
        this.joinTopic(topic);
      }
    };

    this.socket.onmessage = (event) => {
      try {
        const msg = JSON.parse(event.data);
        if (msg.event === 'phx_reply' && msg.payload?.status === 'ok') {
          // Join success, etc.
        } else if (msg.event === 'postgres_changes' || msg.event === 'status_update') {
          const topic = msg.topic;
          const callback = this.listeners.get(topic);
          if (callback && msg.payload) {
            callback(msg.payload);
          }
        }
      } catch (e) {
        console.error('Error parsing Realtime message:', e);
      }
    };

    this.socket.onclose = () => {
      this.isConnected.set(false);
      this.stopHeartbeat();
      this.socket = null;
      // Reconnect after 5 seconds
      setTimeout(() => this.connect(), 5000);
    };

    this.socket.onerror = (err) => {
      console.error('Realtime WebSocket error:', err);
    };
  }

  /** Subscribe to updates on a specific order status */
  subscribeToOrder(orderId: string, callback: (status: string) => void): void {
    this.connect();
    const topic = `realtime:public:orders:id=eq.${orderId}`;
    
    this.listeners.set(topic, (payload) => {
      if (payload.record && payload.record.status) {
        callback(payload.record.status);
      }
    });

    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      this.joinTopic(topic);
    }
  }

  /** Subscribe to updates on a specific reservation status */
  subscribeToReservation(reservationId: string, callback: (status: string) => void): void {
    this.connect();
    const topic = `realtime:public:reservations:id=eq.${reservationId}`;
    
    this.listeners.set(topic, (payload) => {
      if (payload.record && payload.record.status) {
        callback(payload.record.status);
      }
    });

    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      this.joinTopic(topic);
    }
  }

  unsubscribe(orderIdOrReservationId: string, isOrder = true): void {
    const table = isOrder ? 'orders' : 'reservations';
    const topic = `realtime:public:${table}:id=eq.${orderIdOrReservationId}`;
    
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      this.socket.send(JSON.stringify({
        topic,
        event: 'phx_leave',
        payload: {},
        ref: String(++this.refCounter)
      }));
    }
    this.listeners.delete(topic);
  }

  private joinTopic(topic: string): void {
    if (!this.socket) return;
    this.socket.send(JSON.stringify({
      topic,
      event: 'phx_join',
      payload: {
        config: {
          postgres_changes: [
            {
              event: 'UPDATE',
              schema: 'public'
            }
          ]
        }
      },
      ref: String(++this.refCounter)
    }));
  }

  private startHeartbeat(): void {
    this.heartbeatInterval = setInterval(() => {
      if (this.socket?.readyState === WebSocket.OPEN) {
        this.socket.send(JSON.stringify({
          topic: 'phoenix',
          event: 'heartbeat',
          payload: {},
          ref: String(++this.refCounter)
        }));
      }
    }, 30000);
  }

  private stopHeartbeat(): void {
    if (this.heartbeatInterval) {
      clearInterval(this.heartbeatInterval);
      this.heartbeatInterval = null;
    }
  }
}
