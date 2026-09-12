import { TestBed } from '@angular/core/testing';
import { RealtimeService } from '../realtime.service';

describe('RealtimeService', () => {
  let service: RealtimeService;
  let originalWebSocket: typeof WebSocket;

  beforeEach(() => {
    originalWebSocket = (global as any).WebSocket;

    // Mock WebSocket globally - just needs to exist
    (global as any).WebSocket = jest.fn(() => ({
      readyState: 0,  // CONNECTING
      onopen: null,
      onclose: null,
      onmessage: null,
      onerror: null,
      send: jest.fn(() => {}),  // Mock send to do nothing
      close: jest.fn(() => {}),
    }));

    TestBed.configureTestingModule({ providers: [RealtimeService] });
    service = TestBed.inject(RealtimeService);
  });

  afterEach(() => {
    (global as any).WebSocket = originalWebSocket;
    jest.clearAllMocks();
    jest.restoreAllMocks();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('connect() should not throw when supabaseUrl is placeholder', () => {
    // With placeholder URL, connect() returns early and doesn't create socket
    expect(() => service.connect()).not.toThrow();
  });

  it('subscribeToOrder() should not throw', () => {
    expect(() => service.subscribeToOrder('order-123', jest.fn())).not.toThrow();
  });

  it('subscribeToReservation() should not throw', () => {
    expect(() => service.subscribeToReservation('res-999', jest.fn())).not.toThrow();
  });

  it('unsubscribe() should not throw', () => {
    expect(() => service.unsubscribe('order-del', true)).not.toThrow();
  });
});
