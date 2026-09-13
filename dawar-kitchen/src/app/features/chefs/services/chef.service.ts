import { Injectable, inject } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { ApiService } from '@core/http/api.service';
import { Chef } from '../models/chef.model';

/**
 * ChefService: Encapsulates chef data management
 * - Fetches chef list with caching
 * - Manages chef-related state
 */
@Injectable({ providedIn: 'root' })
export class ChefService {
  private readonly api = inject(ApiService);

  private readonly chefsCache$ = this.api.getChefs().pipe(
    shareReplay(1)  // Cache and share across subscribers
  );

  /**
   * Get all chefs (cached)
   */
  getChefs(): Observable<Chef[]> {
    return this.chefsCache$;
  }
}
