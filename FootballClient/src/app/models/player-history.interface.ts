import { Player } from './player.interface';

export interface PlayerHistory {
  player: Player;
  teamId: number;
  matchId: number;
  status: number; // 1 = addedByOrganiser, 2 = joined, 3 = Open
}
