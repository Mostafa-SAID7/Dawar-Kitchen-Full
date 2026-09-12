import { Injectable, computed, signal, effect } from '@angular/core';
import { CartItem } from '../models';

export type { CartItem };

const STORAGE_KEY = 'nn_cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  readonly items = signal<CartItem[]>([]);
  readonly isOpen = signal(false);

  readonly count = computed(() => this.items().reduce((sum, i) => sum + i.quantity, 0));
  readonly total = computed(() => this.items().reduce((sum, i) => sum + i.price * i.quantity, 0));
  readonly isEmpty = computed(() => this.items().length === 0);

  constructor() {
    // Load cart from localStorage on initialization
    const savedCart = this.loadFromStorage();
    if (savedCart.length > 0) {
      this.items.set(savedCart);
    }

    // Auto-persist cart to localStorage whenever items change
    effect(() => {
      const itemsList = this.items();
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(itemsList));
      } catch (e) {
        console.error('Failed to save cart to localStorage', e);
      }
    });
  }

  private loadFromStorage(): CartItem[] {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      return stored ? JSON.parse(stored) : [];
    } catch (e) {
      console.error('Failed to load cart from localStorage', e);
      return [];
    }
  }

  open()  { this.isOpen.set(true); }
  close() { this.isOpen.set(false); }
  toggle(){ this.isOpen.update(v => !v); }

  add(item: Omit<CartItem, 'quantity'>): void {
    this.items.update(list => {
      const existing = list.find(i => i.menuItemId === item.menuItemId);
      if (existing) {
        return list.map(i =>
          i.menuItemId === item.menuItemId ? { ...i, quantity: i.quantity + 1 } : i
        );
      }
      return [...list, { ...item, quantity: 1 }];
    });
  }

  increment(id: string): void {
    this.items.update(list =>
      list.map(i => i.menuItemId === id ? { ...i, quantity: i.quantity + 1 } : i)
    );
  }

  decrement(id: string): void {
    this.items.update(list => {
      const item = list.find(i => i.menuItemId === id);
      if (!item) return list;
      if (item.quantity <= 1) return list.filter(i => i.menuItemId !== id);
      return list.map(i => i.menuItemId === id ? { ...i, quantity: i.quantity - 1 } : i);
    });
  }

  remove(id: string): void {
    this.items.update(list => list.filter(i => i.menuItemId !== id));
  }

  clear(): void {
    this.items.set([]);
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch (e) {
      console.error('Failed to clear cart from localStorage', e);
    }
  }

  setQuantity(id: string, qty: number): void {
    if (qty <= 0) {
      this.remove(id);
      return;
    }
    this.items.update(list =>
      list.map(i => i.menuItemId === id ? { ...i, quantity: qty } : i)
    );
  }

  formattedTotal(): string {
    return `£${this.total().toFixed(2)}`;
  }
}
