/** Payload sent to the API to create a table reservation */
export interface CreateReservationRequest {
  customerName: string;
  email: string;
  phoneNumber: string;
  reservationDate: string;
  reservationTime: string;
  partySize: number;
  specialRequests?: string;
}

/** Response returned after creating a reservation */
export interface CreateReservationResponse {
  id: string;
}

/**
 * Local form model shared by HeroComponent and ReservationComponent.
 * `specialRequests` is optional because the hero form omits that field.
 */
export interface ReservationForm {
  fullName: string;
  email: string;
  phone: string;
  date: Date | null;
  time: string;
  guests: string;
  specialRequests?: string;
}
