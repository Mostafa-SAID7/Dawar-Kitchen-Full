/** Individual item line in an order request */
export interface OrderItemRequest {
  menuItemId: string;
  menuItemName: string;
  unitPrice: number;
  quantity: number;
}

/** Response returned after creating an order */
export interface CreateOrderResponse {
  id: string;
}
