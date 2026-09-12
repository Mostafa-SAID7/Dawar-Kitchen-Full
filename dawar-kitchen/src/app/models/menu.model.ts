/** Raw shape returned by the API */
export interface MenuItem {
  id: string;
  name: string;
  description: string;
  price: number;
  category: string;
  isVegetarian: boolean;
  isVegan: boolean;
  isGlutenFree: boolean;
  isAvailable: boolean;
  imageUrl: string | null;
  sortOrder: number;
}

/** Flattened view model used by menu section & pages */
export interface MenuItemView {
  id: string;
  name: string;
  price: number;
  priceFormatted: string;
  description: string;
  category: string;
  isVegetarian: boolean;
  isVegan: boolean;
}
