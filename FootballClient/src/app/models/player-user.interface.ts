import { UserRole } from './user-role.enum';

export interface PlayerUser {
  email: string;
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  rating: number;
  role: UserRole;
}
