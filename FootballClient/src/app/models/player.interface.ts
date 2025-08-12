export interface Player {
  id?: number;
  firstName?: string;
  lastName?: string;
  rating?: number;
  isAvailable?: boolean;
  isEnabled?: boolean;
  isSelected?: boolean;
  imageUrl?: string;
  locked?: boolean;
  email: string;
  speed: number; // 1 = Low, 2 = Medium, 3 = High
  stamina: number; // 1 = Low, 2 = Medium, 3 = High
  errors: number; // 1 = Low, 2 = Medium, 3 = High (Low = Few errors, High = Many errors)
}
