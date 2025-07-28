import { Player } from './player.interface';

export interface PlayerHistory {
    player: Player;
    teamId: number;
    matchId: number;
}