import { UserRole } from './user-role.enum';

export interface User {
  id: number;
  email: string;
  username: string;
  role: UserRole;

  // Player properties integrated
  firstName: string;
  lastName: string;
  rating: number;
  isDeleted: boolean;
  createdAt: Date;
  updatedAt: Date;
  deletedAt?: Date;
  speed: number;
  stamina: number;
  errors: number;
  profileImageUrl?: string;
}
