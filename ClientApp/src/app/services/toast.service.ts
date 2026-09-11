import { Injectable, signal } from '@angular/core';
import { Toast, ToastType } from '../models/toast.model';

@Injectable({ providedIn: 'root' })
export class ToastService {
    private toastsSignal = signal<Toast[]>([]);
    readonly toasts = this.toastsSignal.asReadonly();

    show(message: string, type: ToastType = 'info', duration = 4000): void {
        const id = Math.random().toString(36).substring(2, 9);
        const toast: Toast = { id, message, type, duration };

        this.toastsSignal.update(list => [...list, toast]);

        if (duration > 0) {
            setTimeout(() => this.remove(id), duration);
        }
    }

    success(message: string, duration = 3500): void {
        this.show(message, 'success', duration);
    }

    error(message: string, duration = 4500): void {
        this.show(message, 'error', duration);
    }

    info(message: string, duration = 3500): void {
        this.show(message, 'info', duration);
    }

    warning(message: string, duration = 3500): void {
        this.show(message, 'warning', duration);
    }

    remove(id: string): void {
        this.toastsSignal.update(list => list.filter(t => t.id !== id));
    }
}
