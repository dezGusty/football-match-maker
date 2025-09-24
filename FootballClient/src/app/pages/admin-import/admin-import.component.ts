import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ImportService,
  PreviewRatingData,
  ImportRatingsResult,
} from '../../services/import.service';
import { NotificationService } from '../../services/notification.service';
import { Header } from '../../components/header/header';

@Component({
  selector: 'app-admin-import',
  standalone: true,
  imports: [CommonModule, FormsModule, Header],
  templateUrl: './admin-import.component.html',
  styleUrls: ['./admin-import.component.css'],
})
export class AdminImportComponent {
  activeTab: string = 'admin';
  collectionName: string = 'ratings';
  fromDate: string = '';
  toDate: string = '';

  isLoadingPreview: boolean = false;
  isLoadingImport: boolean = false;

  previewData: PreviewRatingData[] = [];

  showPreview: boolean = false;

  constructor(
    private importService: ImportService,
    private notificationService: NotificationService
  ) {}

  previewImport(): void {
    this.isLoadingPreview = true;
    this.showPreview = false;

    const hasDateFilter = this.fromDate || this.toDate;

    const previewCall = hasDateFilter
      ? this.importService.previewRatingsImportFiltered(
          this.collectionName,
          this.fromDate,
          this.toDate
        )
      : this.importService.previewRatingsImport(this.collectionName);

    previewCall.subscribe({
      next: (response) => {
        this.previewData = response.previewData;
        this.showPreview = true;
        this.isLoadingPreview = false;

        this.notificationService.showSuccess(
          `Preview loaded: ${this.previewData.length} users found`
        );
      },
      error: (error) => {
        console.error('Preview failed:', error);
        this.isLoadingPreview = false;
        this.notificationService.showError('Failed to load preview data');
      },
    });
  }

  executeImport(): void {
    if (!this.showPreview || this.previewData.length === 0) {
      this.notificationService.showWarning('Please preview data first');
      return;
    }

    this.isLoadingImport = true;

    const hasDateFilter = this.fromDate || this.toDate;

    const importCall = hasDateFilter
      ? this.importService.importRatingsFiltered(
          this.collectionName,
          this.fromDate,
          this.toDate
        )
      : this.importService.importRatings(this.collectionName);

    importCall.subscribe({
      next: (response) => {
        this.isLoadingImport = false;

        const message = `Import completed: ${response.result.usersImported} users imported, ${response.result.existingUsersUpdated} updated`;
        this.notificationService.showSuccess(message);

        this.showPreview = false;
        this.previewData = [];
      },
      error: (error) => {
        console.error('Import failed:', error);
        this.isLoadingImport = false;
        this.notificationService.showError('Import failed');
      },
    });
  }

  clearAll(): void {
    this.previewData = [];
    this.showPreview = false;
    this.fromDate = '';
    this.toDate = '';
    this.notificationService.showInfo('Data cleared');
  }

  getPreviewSummary() {
    if (!this.previewData || this.previewData.length === 0) {
      return null;
    }

    const totalUsers = this.previewData.length;

    return {
      totalUsers,
    };
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString();
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}
