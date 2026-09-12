/** A single item held in the shopping cart */
export interface CartItem {
  menuItemId: string;
  name: string;
  price: number;
  quantity: number;
  category: string;
}
