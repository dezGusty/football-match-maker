import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Header } from '../../components/header/header';
import { RatingHistoryService } from '../../services/rating-history.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import {
  RatingHistory,
  RatingTrend,
  GetRatingHistoryFilters,
} from '../../models/rating-history.interface';
import { UserRole } from '../../models/user-role.enum';
import { User } from '../../models/user.interface';
import {
  Chart,
  ChartConfiguration,
  ChartData,
  ChartOptions,
  registerables,
} from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-rating-evolution',
  standalone: true,
  imports: [CommonModule, FormsModule, Header],
  templateUrl: './rating-evolution.component.html',
  styleUrls: ['./rating-evolution.component.css'],
})
export class RatingEvolutionComponent implements OnInit {
  ratingHistory: RatingHistory[] = [];
  ratingTrend: RatingTrend | null = null;
  chart: Chart | null = null;
  loading = false;
  error = '';

  fromDate = '';
  toDate = '';

  selectedUserId: number | null = null;
  allUsers: User[] = [];
  isAdmin = false;
  currentUserId: number | null = null;

  constructor(
    private ratingHistoryService: RatingHistoryService,
    private authService: AuthService,
    private userService: UserService
  ) {}

  async ngOnInit() {
    this.currentUserId = this.authService.getUserId();
    this.isAdmin = this.authService.getUserRole() === UserRole.ADMIN;

    if (this.isAdmin) {
      await this.loadAllUsers();
    } else {
      this.selectedUserId = this.currentUserId;
    }

    this.setDefaultDateRange();
    await this.loadRatingData();
  }

  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  async loadAllUsers() {
    try {
      this.allUsers = await this.userService.getAllUsers();
    } catch (error) {
      console.error('Error loading users:', error);
      this.error = 'Failed to load users';
    }
  }

  setDefaultDateRange() {
    const today = new Date();
    this.toDate = today.toISOString().split('T')[0];

    const oneYearAgo = new Date();
    oneYearAgo.setFullYear(today.getFullYear() - 1);
    this.fromDate = oneYearAgo.toISOString().split('T')[0];
  }

  async loadRatingData() {
    if (!this.selectedUserId) {
      this.error = 'Please select a user';
      return;
    }

    this.loading = true;
    this.error = '';

    try {
      const filters: GetRatingHistoryFilters = {};

      if (this.fromDate) {
        filters.fromDate = new Date(this.fromDate);
      }

      if (this.toDate) {
        const toDateObj = new Date(this.toDate);
        toDateObj.setHours(23, 59, 59, 999);
        filters.toDate = toDateObj;
      }

      this.ratingHistory = await this.ratingHistoryService.getUserRatingHistory(
        this.selectedUserId,
        filters
      );
      this.ratingTrend = await this.ratingHistoryService.getUserRatingTrend(
        this.selectedUserId
      );

      this.createChart();
    } catch (error) {
      console.error('Error loading rating data:', error);
      this.error = 'Failed to load rating data';
    } finally {
      this.loading = false;
    }
  }

  async onUserChange() {
    if (this.selectedUserId) {
      await this.loadRatingData();
    }
  }

  async onDateChange() {
    if (this.selectedUserId) {
      await this.loadRatingData();
    }
  }

  createChart() {
    if (this.chart) {
      this.chart.destroy();
    }

    if (!this.ratingTrend || this.ratingTrend.ratingPoints.length === 0) {
      return;
    }

    const canvas = document.getElementById('ratingChart') as HTMLCanvasElement;
    if (!canvas) {
      console.error('Canvas element not found');
      return;
    }

    let filteredPoints = this.ratingTrend.ratingPoints;

    if (this.fromDate || this.toDate) {
      filteredPoints = this.ratingTrend.ratingPoints.filter((point) => {
        const pointDate = new Date(point.date);

        if (this.fromDate && pointDate < new Date(this.fromDate)) {
          return false;
        }

        if (this.toDate) {
          const toDateObj = new Date(this.toDate);
          toDateObj.setHours(23, 59, 59, 999);
          if (pointDate > toDateObj) {
            return false;
          }
        }

        return true;
      });
    }

    const chartData: ChartData<'line'> = {
      labels: filteredPoints.map((point) =>
        new Date(point.date).toLocaleDateString()
      ),
      datasets: [
        {
          label: 'Rating',
          data: filteredPoints.map((point) => point.rating),
          borderColor: '#4CAF50',
          backgroundColor: 'rgba(76, 175, 80, 0.1)',
          borderWidth: 2,
          fill: true,
          tension: 0.1,
          pointBackgroundColor: '#4CAF50',
          pointBorderColor: '#4CAF50',
          pointRadius: 4,
          pointHoverRadius: 6,
        },
      ],
    };

    const chartOptions: ChartOptions<'line'> = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: `Rating Evolution - ${this.ratingTrend.userName}`,
          font: {
            size: 16,
            weight: 'bold',
          },
        },
        legend: {
          display: true,
          position: 'top',
        },
        tooltip: {
          callbacks: {
            title: (tooltipItems) => {
              const index = tooltipItems[0].dataIndex;
              const point = filteredPoints[index];
              return new Date(point.date).toLocaleDateString();
            },
            label: (tooltipItem) => {
              const index = tooltipItem.dataIndex;
              const point = filteredPoints[index];
              let label = `Rating: ${point.rating.toFixed(2)}`;
              if (point.matchDetails) {
                label += `\nMatch: ${point.matchDetails}`;
              }
              if (point.changeReason) {
                label += `\nReason: ${point.changeReason}`;
              }
              return label;
            },
          },
          titleFont: {
            size: 14,
          },
          bodyFont: {
            size: 12,
          },
        },
      },
      scales: {
        x: {
          display: true,
          title: {
            display: true,
            text: 'Date',
            font: {
              size: 14,
              weight: 'bold',
            },
          },
          grid: {
            display: true,
            color: 'rgba(0, 0, 0, 0.1)',
          },
        },
        y: {
          display: true,
          title: {
            display: true,
            text: 'Rating',
            font: {
              size: 14,
              weight: 'bold',
            },
          },
          grid: {
            display: true,
            color: 'rgba(0, 0, 0, 0.1)',
          },
          beginAtZero: false,
          min: Math.max(
            0,
            Math.min(...filteredPoints.map((p) => p.rating)) - 0.5
          ),
          max: Math.max(...filteredPoints.map((p) => p.rating)) + 0.5,
        },
      },
      interaction: {
        intersect: false,
        mode: 'index',
      },
    };

    const config: ChartConfiguration<'line'> = {
      type: 'line',
      data: chartData,
      options: chartOptions,
    };

    this.chart = new Chart(canvas, config);
  }

  getRatingClass(rating: number): string {
    if (rating >= 8) return 'rating-high';
    if (rating >= 6) return 'rating-medium';
    return 'rating-low';
  }

  getRatingChangeClass(current: number, previous: number): string {
    if (current > previous) return 'rating-increase';
    if (current < previous) return 'rating-decrease';
    return 'rating-neutral';
  }

  getRatingChangeIcon(current: number, previous: number): string {
    if (current > previous) return '↗️';
    if (current < previous) return '↘️';
    return '➡️';
  }

  formatDate(date: Date | string): string {
    return new Date(date).toLocaleDateString();
  }

  formatDateTime(date: Date | string): string {
    return new Date(date).toLocaleString();
  }
}
