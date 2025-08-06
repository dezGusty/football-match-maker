import { PlayerHistory } from './player-history.interface';

export interface Match {
    id?: number;
    matchDate?: string;
    teamAId: number;
    teamBId: number;
    teamAName?: string;
    teamBName?: string;
    scoreA?: number;
    scoreB?: number;
    teamAGoals?: number;
    teamBGoals?: number;
    playerHistory: PlayerHistory[];
}