import { Component, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { trigger, transition, style, animate, query, stagger } from '@angular/animations';
import { ToastService, Toast } from '../../services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  animations: [
    trigger('toastSlide', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(110%) scale(0.9)' }),
        animate('320ms cubic-bezier(0.34, 1.56, 0.64, 1)',
          style({ opacity: 1, transform: 'translateX(0) scale(1)' }))
      ]),
      transition(':leave', [
        animate('200ms ease-in',
          style({ opacity: 0, transform: 'translateX(110%) scale(0.9)' }))
      ])
    ]),
    trigger('successSlide', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-30px) scale(0.85)' }),
        animate('400ms cubic-bezier(0.34, 1.56, 0.64, 1)',
          style({ opacity: 1, transform: 'translateY(0) scale(1)' }))
      ]),
      transition(':leave', [
        animate('220ms ease-in',
          style({ opacity: 0, transform: 'translateY(-20px) scale(0.9)' }))
      ])
    ]),
    trigger('checkmark', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0)' }),
        animate('300ms 150ms cubic-bezier(0.34, 1.56, 0.64, 1)',
          style({ opacity: 1, transform: 'scale(1)' }))
      ])
    ])
  ],
  template: `
    <div class="fixed inset-0 pointer-events-none z-[9999]">

      <!-- Success Toasts - Top Center -->
      <div class="flex flex-col items-center pt-6 gap-3 px-4">
        <ng-container *ngFor="let toast of successToasts(); trackBy: trackById">
          <div [@successSlide]
               class="pointer-events-auto w-full max-w-sm bg-[#111] border border-[#C65A1E]/30 rounded-2xl shadow-2xl shadow-black/60 overflow-hidden">
            <div class="h-1 bg-gradient-to-r from-[#C65A1E] to-[#e07a42]"></div>
            <div class="p-5 flex items-start gap-4">
              <div [@checkmark] class="flex-shrink-0 w-11 h-11 rounded-full bg-[#C65A1E]/15 flex items-center justify-center ring-1 ring-[#C65A1E]/30">
                <iconify-icon icon="solar:check-circle-bold" width="24" class="text-[#C65A1E]"></iconify-icon>
              </div>
              <div class="flex-1 min-w-0">
                <p class="font-['Forum'] text-lg text-white tracking-tight leading-tight">{{ toast.title }}</p>
                <p class="text-xs text-neutral-400 mt-1 leading-relaxed">{{ toast.message }}</p>
              </div>
              <button (click)="dismiss(toast.id)"
                      class="flex-shrink-0 text-neutral-600 hover:text-neutral-300 transition-colors ml-1">
                <iconify-icon icon="solar:close-circle-linear" width="18"></iconify-icon>
              </button>
            </div>
          </div>
        </ng-container>
      </div>

      <!-- Other Toasts - Top Right -->
      <div class="absolute top-6 right-4 md:right-6 flex flex-col gap-3 items-end w-full max-w-xs">
        <ng-container *ngFor="let toast of otherToasts(); trackBy: trackById">
          <div [@toastSlide]
               class="pointer-events-auto w-full rounded-xl shadow-2xl shadow-black/50 overflow-hidden"
               [ngClass]="{
                 'bg-[#111] border border-red-500/25': toast.type === 'error',
                 'bg-[#111] border border-amber-500/25': toast.type === 'warning',
                 'bg-[#111] border border-blue-500/25': toast.type === 'info'
               }">
            <div class="h-0.5"
                 [ngClass]="{
                   'bg-red-500': toast.type === 'error',
                   'bg-amber-500': toast.type === 'warning',
                   'bg-blue-400': toast.type === 'info'
                 }"></div>
            <div class="px-4 py-3.5 flex items-start gap-3">
              <iconify-icon [icon]="iconFor(toast.type)" width="20"
                            class="flex-shrink-0 mt-0.5"
                            [ngClass]="{
                              'text-red-400': toast.type === 'error',
                              'text-amber-400': toast.type === 'warning',
                              'text-blue-400': toast.type === 'info'
                            }"></iconify-icon>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-white">{{ toast.title }}</p>
                <p class="text-xs text-neutral-400 mt-0.5 leading-relaxed">{{ toast.message }}</p>
              </div>
              <button (click)="dismiss(toast.id)"
                      class="flex-shrink-0 text-neutral-600 hover:text-neutral-300 transition-colors">
                <iconify-icon icon="solar:close-circle-linear" width="16"></iconify-icon>
              </button>
            </div>
          </div>
        </ng-container>
      </div>

    </div>
  `
})
export class ToastComponent {
  private readonly toastService = inject(ToastService);

  successToasts = () => this.toastService.toasts().filter(t => t.type === 'success');
  otherToasts = () => this.toastService.toasts().filter(t => t.type !== 'success');

  trackById(_: number, toast: Toast) { return toast.id; }

  iconFor(type: string): string {
    const icons: Record<string, string> = {
      error: 'solar:close-circle-bold',
      warning: 'solar:danger-triangle-bold',
      info: 'solar:info-circle-bold'
    };
    return icons[type] ?? 'solar:info-circle-bold';
  }

  dismiss(id: number) { this.toastService.dismiss(id); }
}
