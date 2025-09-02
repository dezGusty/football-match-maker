import { User } from './user.interface';

export interface PlayerHistory {
  user: User;
  teamId: number;
  status: number;
}
