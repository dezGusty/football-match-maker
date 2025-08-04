import { PlayerHistory } from './player-history.interface';

export interface MatchCreated {
    id: number;
    matchDate?: string;
    teamAId: number;
    teamBId: number;
    teamAName?: string;
    teamBName?: string;
}