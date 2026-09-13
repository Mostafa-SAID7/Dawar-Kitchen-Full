import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { CartItem } from '../models/cart.model';

const CART_KEY = 'nn_cart';

/**
 * CartService: Shopping cart state management
 * - Manages cart items (add, remove, update, clear)
 * - Persists to localStorage
 * - Provides cart total and count
 */
@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly items$ = signal<CartItem[]>(this.loadCart());
  private readonly isOpen$ = signal(false);

  // Computed state
  readonly items = computed(() => this.items$());
  readonly count = computed(() => 
    this.items$().reduce((sum, item) => sum + item.quantity, 0)
  );
  readonly total = computed(() =>
    this.items$().reduce((sum, item) => sum + (item.price * item.quantity), 0)
  );
  readonly isOpen = computed(() => this.isOpen$());
  readonly isEmpty = computed(() => this.items$().length === 0);

  constructor() {
    // Persist cart to localStorage whenever it changes
    effect(() => {
      const items = this.items$();
      localStorage.setItem(CART_KEY, JSON.stringify(items));
    });
  }

  // Public API
  toggle(): void {
    this.isOpen$.set(!this.isOpen$());
  }

  open(): void {
    this.isOpen$.set(true);
  }

  close(): void {
    this.isOpen$.set(false);
  }

  addItem(item: CartItem): void {
    const items = this.items$();
    const existing = items.find(i => i.menuItemId === item.menuItemId);

    if (existing) {
      this.updateItem(item.menuItemId, { ...existing, quantity: existing.quantity + item.quantity });
    } else {
      this.items$.set([...items, item]);
    }
    this.open();
  }

  removeItem(menuItemId: string): void {
    this.items$.set(this.items$().filter(i => i.menuItemId !== menuItemId));
  }

  /** Alias for removeItem — used by cart-drawer and checkout templates */
  remove(menuItemId: string): void {
    this.removeItem(menuItemId);
  }

  /** Increment quantity by 1 */
  increment(menuItemId: string): void {
    const item = this.items$().find(i => i.menuItemId === menuItemId);
    if (item) this.updateItem(menuItemId, { quantity: item.quantity + 1 });
  }

  /** Decrement quantity by 1 (removes if reaches 0) */
  decrement(menuItemId: string): void {
    const item = this.items$().find(i => i.menuItemId === menuItemId);
    if (item) this.setQuantity(menuItemId, item.quantity - 1);
  }

  /** Formatted total string e.g. £12.50 */
  formattedTotal(): string {
    return `£${this.total().toFixed(2)}`;
  }

  updateItem(menuItemId: string, updates: Partial<CartItem>): void {
    const items = this.items$();
    const updated = items.map(i =>
      i.menuItemId === menuItemId ? { ...i, ...updates } : i
    );
    this.items$.set(updated);
  }

  setQuantity(menuItemId: string, quantity: number): void {
    if (quantity <= 0) {
      this.removeItem(menuItemId);
    } else {
      this.updateItem(menuItemId, { quantity });
    }
  }

  clear(): void {
    this.items$.set([]);
    this.close();
  }

  private loadCart(): CartItem[] {
    try {
      const stored = localStorage.getItem(CART_KEY);
      return stored ? JSON.parse(stored) : [];
    } catch {
      return [];
    }
  }
}
