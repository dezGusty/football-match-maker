export interface RatingHistory {
  id: number;
  userId: number;
  userName: string;
  newRating: number;
  changeReason: string;
  matchId?: number;
  matchDetails?: string;
  ratingSystem?: string;
  createdAt: Date;
}

export interface RatingTrend {
  userId: number;
  userName: string;
  currentRating: number;
  highestRating: number;
  lowestRating: number;
  averageRating: number;
  totalMatches: number;
  lastMatchDate?: Date;
  ratingPoints: RatingPoint[];
}

export interface RatingPoint {
  date: Date;
  rating: number;
  changeReason: string;
  matchDetails?: string;
}

export interface GetRatingHistoryFilters {
  matchId?: number;
  changeReason?: string;
  fromDate?: Date;
  toDate?: Date;
  page?: number;
  pageSize?: number;
}

export interface RatingStatistics {
  userId: number;
  userName: string;
  currentRating: number;
  startingRating: number;
  highestRating: number;
  lowestRating: number;
  totalRatingChanges: number;
  matchesPlayed: number;
  manualAdjustments: number;
  firstRatingChange?: Date;
  lastRatingChange?: Date;
  changeReasonBreakdown: { [key: string]: number };
}
