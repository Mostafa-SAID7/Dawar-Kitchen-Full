import { Component, CUSTOM_ELEMENTS_SCHEMA, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SeoService } from '@shared/services';
import { RealtimeService } from '../../../orders/services/realtime.service';
import { trigger, transition, style, animate, keyframes } from '@angular/animations';

type OrderStatus = 'pending' | 'confirmed' | 'preparing' | 'ready' | 'completed';

@Component({
  selector: 'app-order-confirmed',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  animations: [
    trigger('fadeUp', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(24px)' }),
        animate('500ms 200ms cubic-bezier(0.32,0.72,0,1)', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ]),
    trigger('checkmark', [
      transition(':enter', [
        animate('600ms 100ms ease-out', keyframes([
          style({ opacity: 0, transform: 'scale(0)', offset: 0 }),
          style({ opacity: 1, transform: 'scale(1.2)', offset: 0.7 }),
          style({ opacity: 1, transform: 'scale(1)', offset: 1 })
        ]))
      ])
    ])
  ],
  template: `
    <div data-cy="order-confirmation" class="min-h-screen bg-[#0a0a0a] flex items-center justify-center px-4 py-20">
      <div class="text-center max-w-lg w-full">

        <!-- Checkmark / Header -->
        <div @checkmark class="w-24 h-24 mx-auto mb-8 rounded-full bg-emerald-500/10 border border-emerald-500/20 flex items-center justify-center">
          <iconify-icon icon="solar:check-circle-bold" width="52" class="text-emerald-400"></iconify-icon>
        </div>

        <!-- Content -->
        <div @fadeUp>
          <span class="text-[#C65A1E] text-xs font-medium tracking-[0.2em] uppercase mb-3 block">{{ 'order.confirmed' | translate }}</span>
          <h1 class="font-['Forum'] text-3xl sm:text-4xl text-white tracking-tight mb-4">
            {{ 'order.receivedTitle' | translate }}
          </h1>
          
          <!-- Live Status Stepper -->
          <div class="my-10 bg-[#111] border border-white/5 rounded-2xl p-6 text-left">
            <h3 class="text-xs font-semibold text-neutral-500 uppercase tracking-widest mb-6 flex items-center justify-between">
              <span>{{ 'order.trackerTitle' | translate }}</span>
              <span class="flex items-center gap-1.5 normal-case text-emerald-400 font-sans font-normal">
                <span class="h-2 w-2 rounded-full bg-emerald-400 animate-ping"></span>
                {{ 'order.listening' | translate }}
              </span>
            </h3>
            
            <div class="space-y-6 relative before:absolute before:left-3 before:top-2 before:bottom-2 before:w-0.5 before:bg-white/5">
              <!-- Step 1: Received -->
              <div class="flex items-start gap-4 relative">
                <div class="w-6 h-6 rounded-full flex items-center justify-center shrink-0 z-10 transition-colors duration-300"
                     [ngClass]="isStepActive(1) ? 'bg-[#C65A1E] text-white' : 'bg-white/5 text-neutral-500'">
                  <iconify-icon icon="solar:checklist-minimalistic-bold" width="12"></iconify-icon>
                </div>
                <div>
                  <h4 class="text-sm font-medium transition-colors" [ngClass]="isStepActive(1) ? 'text-white' : 'text-neutral-500'">
                    {{ 'order.received' | translate }}
                  </h4>
                  <p class="text-xs text-neutral-500 mt-0.5">{{ 'order.receivedDescription' | translate }}</p>
                </div>
              </div>

              <!-- Step 2: Confirmed -->
              <div class="flex items-start gap-4 relative">
                <div class="w-6 h-6 rounded-full flex items-center justify-center shrink-0 z-10 transition-colors duration-300"
                     [ngClass]="isStepActive(2) ? 'bg-[#C65A1E] text-white' : 'bg-white/5 text-neutral-500'">
                  <iconify-icon icon="solar:phone-calling-bold" width="12"></iconify-icon>
                </div>
                <div>
                  <h4 class="text-sm font-medium transition-colors" [ngClass]="isStepActive(2) ? 'text-white' : 'text-neutral-500'">
                    {{ 'order.accepted' | translate }}
                  </h4>
                  <p class="text-xs text-neutral-500 mt-0.5">{{ 'order.acceptedDescription' | translate }}</p>
                </div>
              </div>

              <!-- Step 3: Preparing -->
              <div class="flex items-start gap-4 relative">
                <div class="w-6 h-6 rounded-full flex items-center justify-center shrink-0 z-10 transition-colors duration-300"
                     [ngClass]="isStepActive(3) ? 'bg-[#C65A1E] text-white' : 'bg-white/5 text-neutral-500'">
                  <iconify-icon icon="solar:fire-bold" width="12"></iconify-icon>
                </div>
                <div>
                  <h4 class="text-sm font-medium transition-colors" [ngClass]="isStepActive(3) ? 'text-white' : 'text-neutral-500'">
                    {{ 'order.preparing' | translate }}
                  </h4>
                  <p class="text-xs text-neutral-500 mt-0.5">{{ 'order.preparingDescription' | translate }}</p>
                </div>
              </div>

              <!-- Step 4: Ready -->
              <div class="flex items-start gap-4 relative">
                <div class="w-6 h-6 rounded-full flex items-center justify-center shrink-0 z-10 transition-colors duration-300"
                     [ngClass]="isStepActive(4) ? 'bg-emerald-500 text-white' : 'bg-white/5 text-neutral-500'">
                  <iconify-icon icon="solar:bag-5-bold" width="12"></iconify-icon>
                </div>
                <div>
                  <h4 class="text-sm font-medium transition-colors" [ngClass]="isStepActive(4) ? 'text-emerald-400 font-semibold' : 'text-neutral-500'">
                    {{ 'order.ready' | translate }}
                  </h4>
                  <p class="text-xs text-neutral-500 mt-0.5">{{ 'order.readyDescription' | translate }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Order ID / Invoice details -->
          <div *ngIf="orderId" class="mt-8 flex flex-col items-center gap-2 px-4 py-3 bg-[#111] border border-white/5 rounded-xl">
            <div class="flex items-center gap-2">
              <iconify-icon icon="solar:tag-linear" width="16" class="text-neutral-500"></iconify-icon>
              <span class="text-xs text-neutral-500">{{ 'order.reference' | translate }}</span>
              <span data-cy="order-reference" class="text-xs text-white font-medium font-mono">#{{ shortId }}</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="mt-8 flex flex-col sm:flex-row items-center justify-center gap-3">
            <button (click)="goHome()"
                    class="w-full sm:w-auto px-8 py-3.5 text-sm font-medium text-[#0a0a0a] bg-white rounded-xl hover:bg-[#C65A1E] hover:text-white hover:shadow-[0_0_20px_rgba(198,90,30,0.35)] transition-all duration-300">
              {{ 'order.backHome' | translate }}
            </button>
          </div>
        </div>

      </div>
    </div>
  `
})
export class OrderConfirmedComponent implements OnInit, OnDestroy {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly seo = inject(SeoService);
  private readonly realtime = inject(RealtimeService);

  orderId: string | null = null;
  shortId = '';
  status: OrderStatus = 'pending';

  ngOnInit(): void {
    this.seo.setOrderConfirmed();
    this.orderId = this.route.snapshot.queryParamMap.get('id');
    if (this.orderId) {
      this.shortId = this.orderId.split('-')[0].toUpperCase();
      // Listen for live updates
      this.realtime.subscribeToOrder(this.orderId, (newStatus) => {
        this.status = newStatus as OrderStatus;
      });
    }
  }

  ngOnDestroy(): void {
    if (this.orderId) {
      this.realtime.unsubscribe(this.orderId, true);
    }
  }

  isStepActive(step: number): boolean {
    switch (step) {
      case 1:
        return true; // Always active when order is received
      case 2:
        return ['confirmed', 'preparing', 'ready', 'completed'].includes(this.status);
      case 3:
        return ['preparing', 'ready', 'completed'].includes(this.status);
      case 4:
        return ['ready', 'completed'].includes(this.status);
      default:
        return false;
    }
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}
