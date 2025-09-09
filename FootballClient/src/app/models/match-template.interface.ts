export interface MatchTemplate {
  id?: number;
  location: string;
  cost: number | null;
  name: string;
  teamAName?: string;
  teamBName?: string;
}

export interface CreateMatchTemplateRequest {
  location: string;
  cost: number | null;
  name: string;
  teamAName?: string;
  teamBName?: string;
}

export interface UpdateMatchTemplateRequest {
  location: string;
  cost: number | null;
  name: string;
  teamAName?: string;
  teamBName?: string;
}
