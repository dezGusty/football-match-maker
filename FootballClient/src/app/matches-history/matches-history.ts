import { Component } from '@angular/core';
import { Header } from "../header/header";
import { MatchService } from '../match.service';
import { Match } from '../match.interface';
import { DatePipe } from '@angular/common';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerHistory } from '../player-history.interface';
import { Player } from '../player.interface';

@Component({
  selector: 'app-matches-history',
  imports: [Header, DatePipe, CommonModule],
  templateUrl: './matches-history.html',
  styleUrl: './matches-history.css'
})

export class MatchesHistory implements OnInit {
  matches: Match[] = [];

  constructor(private MatchService: MatchService) {}


  async ngOnInit() {
    await this.init();
  }

  async init() {
    try {
      this.matches = await this.MatchService.getMatches();
      for (const match of this.matches) {
        match.teamAName = await this.MatchService.getTeamById(match.teamAId);
        match.teamBName = await this.MatchService.getTeamById(match.teamBId);
      }
    } catch (error) {
      console.error('Error fetching matches:', error);
    }
  }

selectedTeamAName: string = '';
selectedTeamBName: string = '';
selectedTeamAPlayers: string[] = [];
selectedTeamBPlayers: string[] = [];
modalOpen: boolean = false;
selectedMatch: Match | null = null;

async openPlayersModal(match: Match) {
  try {
    this.selectedMatch = match;
    this.selectedTeamAName = match.teamAName!;
    this.selectedTeamBName = match.teamBName!;

    this.selectedTeamAPlayers = match.playerHistory
      .filter(p => p.teamId === match.teamAId && p.player)
      .map(p => `${p.player.firstName} ${p.player.lastName}`);

    this.selectedTeamBPlayers = match.playerHistory
      .filter(p => p.teamId === match.teamBId && p.player)
      .map(p => `${p.player.firstName} ${p.player.lastName}`);

    this.modalOpen = true;
  } catch (error) {
    console.error('Error loading player data:', error);
  }
}

closeModal() {
  this.modalOpen = false;
}

 getPlayers(match: Match | null, teamId?: number): string[] {
    if (!match || !teamId) return [];
    return match.playerHistory
      .filter(ph => ph.teamId === teamId && ph.player)
      .map(ph => `${ph.player.firstName} ${ph.player.lastName}`);
  }
}

