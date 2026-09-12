/** Payload sent to the API for a contact enquiry */
export interface CreateContactRequest {
  name: string;
  email: string;
  phoneNumber?: string;
  subject: string;
  message: string;
}

/** Response returned after submitting a contact enquiry */
export interface CreateContactResponse {
  id: string;
}
