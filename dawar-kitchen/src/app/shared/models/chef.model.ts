/** Raw chef shape returned by the API */
export interface Chef {
  id: string;
  name: string;
  title: string;
  bio: string;
  imageUrl: string | null;
  specialty: string;
  sortOrder: number;
}

/** Display-ready view model used by the chefs section component */
export interface ChefView {
  name: string;
  role: string;
  image: string;
  bio: string;
  specialty: string;
  initial: string;
}
