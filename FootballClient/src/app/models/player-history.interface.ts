import { User } from './user.interface';

export interface PlayerHistory {
  user: User;
  teamId: number;
  matchId: number;
  status: number; // 1 = addedByOrganiser, 2 = joined, 3 = Open
}
