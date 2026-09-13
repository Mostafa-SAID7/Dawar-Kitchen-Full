import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '@core/http/api.service';
import { CreateReservationRequest, CreateReservationResponse } from '../models/reservation.model';

/**
 * ReservationService: Encapsulates reservation business logic
 * - Handles reservation form submission
 * - Manages reservation state
 * - Coordinates with API for reservation creation
 */
@Injectable({ providedIn: 'root' })
export class ReservationService {
  private readonly api = inject(ApiService);

  /**
   * Submit a new reservation request
   */
  createReservation(data: CreateReservationRequest): Observable<CreateReservationResponse> {
    return this.api.createReservation(data);
  }
}
