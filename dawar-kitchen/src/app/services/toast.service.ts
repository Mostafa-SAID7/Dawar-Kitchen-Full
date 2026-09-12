import { Injectable, signal } from '@angular/core';
import { Toast, ToastType } from '../models';

export type { Toast, ToastType };


@Injectable({ providedIn: 'root' })
export class ToastService {
  private counter = 0;
  readonly toasts = signal<Toast[]>([]);

  success(message: string, title = 'Booking Confirmed!') {
    this.add({ type: 'success', title, message, duration: 7000 });
  }

  error(message: string, title = 'Something went wrong') {
    this.add({ type: 'error', title, message, duration: 5000 });
  }

  warning(message: string, title = 'Please check') {
    this.add({ type: 'warning', title, message, duration: 5000 });
  }

  info(message: string, title = 'Info') {
    this.add({ type: 'info', title, message, duration: 4000 });
  }

  dismiss(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  private add(toast: Omit<Toast, 'id'> & { duration: number }): void {
    const id = ++this.counter;
    this.toasts.update(list => [...list, { ...toast, id }]);
    setTimeout(() => this.dismiss(id), toast.duration);
  }
}
