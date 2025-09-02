export interface CreateMatchRequest {
  matchDate: string;
  status: number;
  location?: string;
  cost?: number;
  teamAName?: string;
  teamBName?: string;
}

export interface CreateMatchResponse {
  id: number;
  matchDate: string;
  isPublic: boolean;
  status: number;
  location?: string;
  cost?: number;
  organiserId: number;
}

export interface MatchDisplay {
  id: number;
  matchDate: string;
  location?: string;
  cost?: number;
  teamAName?: string;
  teamBName?: string;
  status: number;
  isPublic: boolean;
  teamAPlayerCount?: number;
  teamBPlayerCount?: number;
  myTeam?: 'A' | 'B' | null;
  canJoin?: boolean;
  canManage?: boolean;
}
