import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { ApiService } from '@core/http/api.service';
import { MenuItem } from '../models/menu.model';

/**
 * MenuService: Encapsulates menu business logic
 * - Fetches menu items with caching
 * - Handles category filtering
 * - Manages menu-related state
 */
@Injectable({ providedIn: 'root' })
export class MenuService {
  private readonly api = inject(ApiService);

  private readonly menuCache = new Map<string | undefined, Observable<MenuItem[]>>();

  /**
   * Get menu items, optionally filtered by category
   * Results are cached to prevent duplicate requests
   */
  getMenu(category?: string): Observable<MenuItem[]> {
    if (!this.menuCache.has(category)) {
      const menu$ = this.api.getMenu(category).pipe(
        shareReplay(1)  // Cache and share across subscribers
      );
      this.menuCache.set(category, menu$);
    }
    return this.menuCache.get(category)!;
  }

  /**
   * Clear menu cache (useful when data changes or on logout)
   */
  clearCache(): void {
    this.menuCache.clear();
  }
}
