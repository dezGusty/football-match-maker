import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'info' | 'warning';
  message: string;
  duration?: number;
  dismissible?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notifications$ = new BehaviorSubject<Notification[]>([]);
  private idCounter = 0;

  constructor() {}

  getNotifications(): Observable<Notification[]> {
    return this.notifications$.asObservable();
  }

  private addNotification(notification: Omit<Notification, 'id'>): void {
    const id = `notification-${++this.idCounter}`;
    const newNotification: Notification = {
      id,
      dismissible: true,
      ...notification,
      duration: notification.duration ?? 4000,
    };

    const currentNotifications = this.notifications$.getValue();
    this.notifications$.next([...currentNotifications, newNotification]);

    if (newNotification.duration && newNotification.duration > 0) {
      setTimeout(() => {
        console.log('Auto-removing notification:', id);
        this.removeNotification(id);
      }, newNotification.duration);
    }
  }

  showSuccess(message: string, duration?: number): void {
    this.addNotification({
      type: 'success',
      message,
      duration: duration ?? 3000,
    });
  }

  showError(message: string, duration?: number): void {
    this.addNotification({
      type: 'error',
      message,
      duration: duration ?? 3000,
    });
  }

  showInfo(message: string, duration?: number): void {
    this.addNotification({
      type: 'info',
      message,
      duration: duration ?? 3000,
    });
  }

  showWarning(message: string, duration?: number): void {
    this.addNotification({
      type: 'warning',
      message,
      duration: duration ?? 3000,
    });
  }

  removeNotification(id: string): void {
    const currentNotifications = this.notifications$.getValue();
    const updatedNotifications = currentNotifications.filter(
      (n) => n.id !== id
    );
    this.notifications$.next(updatedNotifications);
  }

  clearAll(): void {
    this.notifications$.next([]);
  }
}
