import { Component } from '@angular/core';
import { Header } from "../header/header";
import { MatchService } from '../matches.service';
import { Match } from '../match.interface';
import { DatePipe } from '@angular/common';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

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
}

