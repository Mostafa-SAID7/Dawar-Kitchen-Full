import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, retry } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { MenuItem } from '@features/menu/models/menu.model';
import { Chef } from '@features/chefs/models/chef.model';
import {
  CreateReservationRequest,
  CreateReservationResponse,
} from '@features/reservations/models/reservation.model';
import {
  CreateContactRequest,
  CreateContactResponse,
} from '@features/contact/models/contact.model';
import {
  CreateOrderRequest,
  CreateOrderResponse,
  CreateCheckoutSessionRequest,
  CreateCheckoutSessionResponse,
} from '@features/checkout/models/order.model';

export type {
  MenuItem,
  Chef,
  CreateReservationRequest,
  CreateReservationResponse,
  CreateContactRequest,
  CreateContactResponse,
  CreateOrderRequest,
  CreateOrderResponse,
  CreateCheckoutSessionRequest,
  CreateCheckoutSessionResponse,
};

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api`;

  // Mock Data Fallbacks
  private readonly mockMenu: MenuItem[] = [
    { id: '1', name: 'Koshari', description: "Egypt's national dish — lentils, rice, pasta, chickpeas, crispy onions & tomato sauce", price: 14.00, category: 'Mains', isVegetarian: true, isVegan: true, isGlutenFree: false, isAvailable: true, imageUrl: null, sortOrder: 1 },
    { id: '2', name: 'Charcoal Kofta', description: 'Spiced minced lamb and beef kebabs grilled over natural charcoal with tahini', price: 28.00, category: 'Grill & Kofta', isVegetarian: false, isVegan: false, isGlutenFree: true, isAvailable: true, imageUrl: null, sortOrder: 2 },
    { id: '3', name: 'Ta\'meya & Egyptian Mezze', description: 'Crispy fava bean falafel, ful medames, tahini, and warm baladi bread', price: 16.00, category: 'Egyptian Starters', isVegetarian: true, isVegan: true, isGlutenFree: false, isAvailable: true, imageUrl: null, sortOrder: 3 },
    { id: '4', name: 'Om Ali', description: 'Classic Egyptian warm bread pudding with sweet milk, raisins, and toasted nuts', price: 12.00, category: 'Beverages & Desserts', isVegetarian: true, isVegan: false, isGlutenFree: false, isAvailable: true, imageUrl: null, sortOrder: 4 },
    { id: '5', name: 'Hibiscus Karkadeh Tea', description: 'Refreshing Egyptian iced hibiscus tea infused with mint', price: 5.50, category: 'Beverages & Desserts', isVegetarian: true, isVegan: true, isGlutenFree: true, isAvailable: true, imageUrl: null, sortOrder: 5 }
  ];

  private readonly mockChefs: Chef[] = [
    { id: 'c1', name: 'Chef Tarek Al-Masri', title: 'Executive Chef', bio: 'Over 20 years of experience in Cairo & Alexandria culinary heritage.', imageUrl: null, specialty: 'Charcoal Grill & Egyptian Mains', sortOrder: 1 }
  ];

  getPublicImageUrl(bucket: string, path: string | null): string | null {
    if (!path) return null;
    if (path.startsWith('http://') || path.startsWith('https://') || path.startsWith('assets/')) return path;
    const isPlaceholder = !environment.supabaseUrl
      || environment.supabaseUrl.includes('your-project')
      || environment.supabaseUrl.includes('placeholder');
    if (isPlaceholder) return null;
    return `${environment.supabaseUrl}/storage/v1/object/public/${bucket}/${path}`;
  }

  getMenu(category?: string): Observable<MenuItem[]> {
    const url = category
      ? `${this.baseUrl}/menu?category=${encodeURIComponent(category)}`
      : `${this.baseUrl}/menu`;
    return this.http.get<MenuItem[]>(url).pipe(
      map(items => items.map(item => ({
        ...item,
        imageUrl: this.getPublicImageUrl('menu-item-images', item.imageUrl)
      }))),
      // Free-tier host may be waking from sleep (cold start): retry twice with 2 s delay.
      retry({ count: 2, delay: 2000 }),
      catchError(() => {
        let items = [...this.mockMenu];
        if (category) {
          items = items.filter(i => i.category === category);
        }
        return of(items);
      })
    );
  }

  getChefs(): Observable<Chef[]> {
    return this.http.get<Chef[]>(`${this.baseUrl}/chefs`).pipe(
      map(chefs => chefs.map(chef => ({
        ...chef,
        imageUrl: this.getPublicImageUrl('chef-images', chef.imageUrl)
      }))),
      // Free-tier host may be waking from sleep (cold start): retry twice with 2 s delay.
      retry({ count: 2, delay: 2000 }),
      catchError(() => of(this.mockChefs))
    );
  }

  createReservation(data: CreateReservationRequest): Observable<CreateReservationResponse> {
    return this.http.post<CreateReservationResponse>(`${this.baseUrl}/reservations`, data).pipe(
      catchError(() => of({ id: 'RES-' + Math.floor(1000 + Math.random() * 9000) }))
    );
  }

  createContact(data: CreateContactRequest): Observable<CreateContactResponse> {
    return this.http.post<CreateContactResponse>(`${this.baseUrl}/contact`, data).pipe(
      catchError(() => of({ id: 'CON-' + Math.floor(1000 + Math.random() * 9000) }))
    );
  }

  createOrder(data: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this.http.post<CreateOrderResponse>(`${this.baseUrl}/orders`, data).pipe(
      catchError(() => of({ id: 'ORD-' + Math.floor(1000 + Math.random() * 9000) }))
    );
  }

  createCheckoutSession(data: CreateCheckoutSessionRequest): Observable<CreateCheckoutSessionResponse> {
    return this.http.post<CreateCheckoutSessionResponse>(
      `${this.baseUrl}/payments/create-checkout-session`,
      data
    );
  }
}
