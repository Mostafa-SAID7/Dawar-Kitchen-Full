import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '@core/http/api.service';
import { CreateContactRequest, CreateContactResponse } from '../models/contact.model';

/**
 * ContactService: Encapsulates contact form logic
 * - Handles contact message submission
 * - Manages submission state
 */
@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly api = inject(ApiService);

  /**
   * Submit a contact message
   */
  sendContact(data: CreateContactRequest): Observable<CreateContactResponse> {
    return this.api.createContact(data);
  }
}
