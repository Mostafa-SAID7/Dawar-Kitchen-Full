import {
  ChangeDetectionStrategy, ChangeDetectorRef, Component, CUSTOM_ELEMENTS_SCHEMA, inject, signal, OnInit, OnDestroy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { trigger, transition, style, animate } from '@angular/animations';
import { Subscription } from 'rxjs';
import { AppValidators } from '@shared/validators';
import { CartService } from '@features/checkout/services/cart.service';
import { ApiService } from '@core/http/api.service';
import { ToastService } from '@shared/services';
import { DrawerStep } from '@features/checkout/models/drawer.model';
import { PricePipe } from '@shared/pipes';

@Component({
  selector: 'app-cart-drawer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PricePipe, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    trigger('backdrop', [
      transition(':enter', [style({ opacity: 0 }), animate('200ms ease', style({ opacity: 1 }))]),
      transition(':leave', [animate('200ms ease', style({ opacity: 0 }))])
    ]),
    trigger('drawer', [
      transition(':enter', [
        style({ transform: 'translateX(100%)' }),
        animate('300ms cubic-bezier(0.32, 0.72, 0, 1)', style({ transform: 'translateX(0)' }))
      ]),
      transition(':leave', [
        animate('250ms cubic-bezier(0.32, 0.72, 0, 1)', style({ transform: 'translateX(100%)' }))
      ])
    ]),
    trigger('stepIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateX(20px)' }),
        animate('260ms cubic-bezier(0.32, 0.72, 0, 1)', style({ opacity: 1, transform: 'translateX(0)' }))
      ])
    ])
  ],
  template: `
    <ng-container *ngIf="cart.isOpen()">

      <!-- Backdrop -->
      <div @backdrop (click)="closeDrawer()"
           class="fixed inset-0 bg-black/60 backdrop-blur-sm z-[200]"></div>

      <!-- Drawer panel -->
      <div @drawer data-cy="cart-drawer"
           class="fixed right-0 top-0 h-full w-full max-w-sm sm:max-w-md bg-[#0f0f0f] border-l border-white/5 z-[201] flex flex-col shadow-2xl">

        <!-- ── Header ── -->
        <div class="flex items-center justify-between px-6 py-5 border-b border-white/5 shrink-0">
          <div class="flex items-center gap-3">
            <button *ngIf="step() === 2" (click)="goBack()"
                    class="text-neutral-500 hover:text-white transition-colors p-1 -ml-1">
              <iconify-icon icon="solar:arrow-left-linear" width="20"></iconify-icon>
            </button>
            <iconify-icon
              [attr.icon]="step() === 3 ? 'solar:check-circle-bold' : 'solar:cart-large-2-bold'"
              width="22"
              [ngClass]="step() === 3 ? 'text-emerald-400' : 'text-[#C65A1E]'">
            </iconify-icon>
            <h2 class="font-['Forum'] text-xl text-white tracking-tight">
              <ng-container [ngSwitch]="step()">
                <span *ngSwitchCase="1">{{ 'cart.title' | translate }}</span>
                <span *ngSwitchCase="2">{{ 'checkout.yourOrder' | translate }}</span>
                <span *ngSwitchCase="3">{{ 'order.receivedTitle' | translate }}</span>
              </ng-container>
            </h2>
            <span *ngIf="step() === 1 && !cart.isEmpty()"
                  class="text-xs font-medium text-neutral-400 bg-white/5 px-2 py-0.5 rounded-full">
              {{ cart.count() }} {{ cart.count() === 1 ? ('checkout.item' | translate) : ('checkout.items' | translate) }}
            </span>
          </div>
          <button (click)="closeDrawer()" data-cy="cart-drawer-close" class="text-neutral-500 hover:text-white transition-colors p-1">
            <iconify-icon icon="solar:close-circle-linear" width="24"></iconify-icon>
          </button>
        </div>

        <!-- Step progress bar (steps 1 & 2) -->
        <div *ngIf="step() !== 3" class="flex items-center gap-1.5 px-6 pt-4 pb-1 shrink-0">
          <div class="h-0.5 rounded-full flex-1 transition-all duration-300"
               [ngClass]="step() >= 1 ? 'bg-[#C65A1E]' : 'bg-white/8'"></div>
          <div class="h-0.5 rounded-full flex-1 transition-all duration-300"
               [ngClass]="step() >= 2 ? 'bg-[#C65A1E]' : 'bg-white/8'"></div>
          <div class="h-0.5 rounded-full flex-1 transition-all duration-300 bg-white/8"></div>
        </div>

        <!-- ══════════ STEP 1 — CART ══════════ -->
        <ng-container *ngIf="step() === 1">

          <!-- Empty state -->
          <div *ngIf="cart.isEmpty()" @stepIn
               class="flex-1 flex flex-col items-center justify-center px-8 text-center">
            <div class="w-16 h-16 rounded-full bg-white/5 flex items-center justify-center mb-5">
              <iconify-icon icon="solar:cart-large-2-linear" width="28" class="text-neutral-500"></iconify-icon>
            </div>
            <p class="text-white font-['Forum'] text-xl mb-2">{{ 'cart.empty' | translate }}</p>
            <p class="text-sm text-neutral-500 leading-relaxed">{{ 'cartDrawer.emptyDescription' | translate }}</p>
            <button (click)="closeDrawer()"
                    class="mt-6 px-6 py-2.5 text-sm text-white border border-white/15 rounded-xl hover:bg-white/5 transition-all">
              {{ 'cartDrawer.browseMenu' | translate }}
            </button>
          </div>

          <!-- Items list -->
          <div *ngIf="!cart.isEmpty()" @stepIn class="flex-1 overflow-y-auto px-6 py-4 space-y-3">
            <div *ngFor="let item of cart.items(); trackBy: trackByCartItem"
                 data-cy="cart-item"
                 class="flex items-start gap-4 p-4 bg-[#111] rounded-xl border border-white/5">
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-white leading-tight">{{ item.name }}</p>
                <p class="text-xs text-neutral-500 mt-0.5">{{ item.category }}</p>
                <p class="text-sm text-[#C65A1E] font-medium mt-1.5" data-cy="item-price">
                  {{ (item.price * item.quantity) | price }}
                </p>
              </div>
              <div class="flex items-center gap-2 shrink-0">
                <button (click)="cart.decrement(item.menuItemId)"
                        class="w-7 h-7 rounded-lg bg-white/5 hover:bg-white/10 flex items-center justify-center text-white transition-colors">
                  <iconify-icon icon="solar:minus-square-linear" width="16"></iconify-icon>
                </button>
                <span class="text-sm text-white w-5 text-center font-medium">{{ item.quantity }}</span>
                <button (click)="cart.increment(item.menuItemId)"
                        class="w-7 h-7 rounded-lg bg-white/5 hover:bg-white/10 flex items-center justify-center text-white transition-colors">
                  <iconify-icon icon="solar:add-square-linear" width="16"></iconify-icon>
                </button>
                <button (click)="cart.remove(item.menuItemId)"
                        class="w-7 h-7 rounded-lg hover:bg-red-500/10 flex items-center justify-center text-neutral-500 hover:text-red-400 transition-colors ml-1">
                  <iconify-icon icon="solar:trash-bin-trash-linear" width="15"></iconify-icon>
                </button>
              </div>
            </div>
          </div>

          <!-- Step 1 footer -->
          <div *ngIf="!cart.isEmpty()" class="px-6 py-5 border-t border-white/5 space-y-3 shrink-0">
            <div class="flex items-center justify-between text-sm">
              <span class="text-neutral-400">{{ 'cart.subtotal' | translate }}</span>
              <span class="text-white font-medium" data-cy="order-total">{{ cart.formattedTotal() }}</span>
            </div>
            <p class="text-xs text-neutral-600">{{ 'cartDrawer.deliveryFeeNextStep' | translate }}</p>
            <button (click)="goToStep2()"
                    class="w-full py-3.5 text-sm font-medium text-[#0a0a0a] bg-white rounded-xl hover:bg-[#C65A1E] hover:text-white hover:shadow-[0_0_20px_rgba(198,90,30,0.35)] transition-all duration-300 flex items-center justify-center gap-2">
              {{ 'cartDrawer.continueCheckout' | translate }}
              <iconify-icon icon="solar:arrow-right-linear" width="16"></iconify-icon>
            </button>
          </div>

        </ng-container>

        <!-- ══════════ STEP 2 — CHECKOUT FORM ══════════ -->
        <ng-container *ngIf="step() === 2 && form">
          <div @stepIn class="flex-1 overflow-y-auto px-6 py-5 space-y-5" [formGroup]="form">

            <!-- Mini order summary -->
            <div class="flex items-center justify-between py-3 px-4 bg-[#111] rounded-xl border border-white/5">
              <div class="flex items-center gap-2 text-xs text-neutral-400">
                <iconify-icon icon="solar:cart-large-2-linear" width="14" class="text-neutral-500"></iconify-icon>
                {{ cart.count() }} {{ cart.count() === 1 ? ('checkout.item' | translate) : ('checkout.items' | translate) }}
              </div>
              <span class="text-sm font-medium text-white font-['Forum']">{{ cart.formattedTotal() }}</span>
            </div>

            <!-- Order type -->
            <div>
              <p class="text-xs text-neutral-500 tracking-[0.15em] uppercase mb-3">{{ 'cartDrawer.orderTypePrompt' | translate }}</p>
              <select formControlName="type" name="orderType" data-cy="order-type-select" class="nn-input w-full mb-3">
                <option value="dine-in">{{ 'reservations.dineIn' | translate }}</option>
                <option value="delivery">{{ 'reservations.delivery' | translate }}</option>
                <option value="collection">{{ 'reservations.pickup' | translate }}</option>
              </select>
              <div class="grid grid-cols-3 gap-2">
                <label *ngFor="let t of orderTypes; trackBy: trackByType"
                       class="flex flex-col items-center gap-1.5 p-3 rounded-xl border cursor-pointer transition-all duration-200"
                       [ngClass]="orderType === t.value
                         ? 'border-[#C65A1E] bg-[#C65A1E]/8 shadow-[0_0_12px_rgba(198,90,30,0.12)]'
                         : 'border-white/8 bg-[#111] hover:border-white/20'">
                  <input type="radio" formControlName="type" [value]="t.value" class="sr-only">
                  <div class="w-8 h-8 rounded-lg flex items-center justify-center transition-colors"
                       [ngClass]="orderType === t.value ? 'bg-[#C65A1E]/15' : 'bg-white/5'">
                    <iconify-icon [attr.icon]="t.icon" width="16"
                                  [ngClass]="orderType === t.value ? 'text-[#C65A1E]' : 'text-neutral-500'">
                    </iconify-icon>
                  </div>
                  <span class="text-xs font-medium text-center"
                        [ngClass]="orderType === t.value ? 'text-white' : 'text-neutral-400'">
                    {{ t.label }}
                  </span>
                  <span class="text-[10px] text-neutral-600 text-center leading-tight">{{ t.sub }}</span>
                </label>
              </div>
            </div>

            <!-- Contact details -->
            <div class="space-y-3">
              <p class="text-xs text-neutral-500 tracking-[0.15em] uppercase">{{ 'cartDrawer.contactDetails' | translate }}</p>

              <div>
                <input formControlName="customerName" type="text" [placeholder]="'cartDrawer.fullName' | translate"
                       name="customerName"
                       data-cy="customer-name"
                       class="nn-input"
                       [ngClass]="err('customerName') ? 'nn-field--error' : ''">
                <p *ngIf="err('customerName')" class="mt-1.5 text-xs text-red-400 flex items-center gap-1">
                  <iconify-icon icon="solar:danger-circle-linear" width="12"></iconify-icon>
                  <span *ngIf="f['customerName'].errors?.['required']">{{ 'cartDrawer.nameRequired' | translate }}</span>
                  <span *ngIf="f['customerName'].errors?.['minlength']">{{ 'cartDrawer.nameMin' | translate }}</span>
                </p>
              </div>

              <div>
                <input formControlName="email" type="email" [placeholder]="'cartDrawer.emailAddress' | translate"
                       class="nn-input"
                       [ngClass]="err('email') ? 'nn-field--error' : ''">
                <p *ngIf="err('email')" class="mt-1.5 text-xs text-red-400 flex items-center gap-1">
                  <iconify-icon icon="solar:danger-circle-linear" width="12"></iconify-icon>
                  <span *ngIf="f['email'].errors?.['required']">{{ 'cartDrawer.emailRequired' | translate }}</span>
                  <span *ngIf="f['email'].errors?.['email']">{{ 'cartDrawer.emailInvalid' | translate }}</span>
                </p>
              </div>

              <div>
                <input formControlName="phoneNumber" type="tel" [placeholder]="'cartDrawer.phoneNumber' | translate"
                       name="phone"
                       data-cy="phone"
                       class="nn-input"
                       [ngClass]="err('phoneNumber') ? 'nn-field--error' : ''">
                <p *ngIf="err('phoneNumber')" class="mt-1.5 text-xs text-red-400 flex items-center gap-1">
                  <iconify-icon icon="solar:danger-circle-linear" width="12"></iconify-icon>
                  <span *ngIf="f['phoneNumber'].errors?.['required']">{{ 'cartDrawer.phoneRequired' | translate }}</span>
                  <span *ngIf="f['phoneNumber'].errors?.['minlength']">{{ 'cartDrawer.phoneInvalid' | translate }}</span>
                </p>
              </div>
            </div>

            <!-- Delivery address -->
            <div *ngIf="isDelivery" class="space-y-2">
              <p class="text-xs text-neutral-500 tracking-[0.15em] uppercase">{{ 'cartDrawer.deliveryAddress' | translate }}</p>
              <textarea formControlName="deliveryAddress" rows="3"
                        [placeholder]="'cartDrawer.deliveryAddressPlaceholder' | translate"
                        name="address"
                        data-cy="delivery-address"
                        class="nn-input resize-none"
                        [ngClass]="err('deliveryAddress') ? 'nn-field--error' : ''"></textarea>
              <p *ngIf="err('deliveryAddress')" class="text-xs text-red-400 flex items-center gap-1">
                <iconify-icon icon="solar:danger-circle-linear" width="12"></iconify-icon>
                {{ 'cartDrawer.deliveryAddressRequired' | translate }}
              </p>
            </div>

            <!-- Pickup time (for takeaway) -->
            <div *ngIf="orderType === 'collection'" class="space-y-2">
              <p class="text-xs text-neutral-500 tracking-[0.15em] uppercase">{{ 'cartDrawer.pickupTime' | translate }}</p>
              <input formControlName="pickupTime" type="time"
                     data-cy="pickup-time"
                     class="nn-input"
                     [ngClass]="err('pickupTime') ? 'nn-field--error' : ''">
              <p *ngIf="err('pickupTime')" class="text-xs text-red-400 flex items-center gap-1">
                <iconify-icon icon="solar:danger-circle-linear" width="12"></iconify-icon>
                {{ 'cartDrawer.pickupTimeRequired' | translate }}
              </p>
            </div>

            <!-- Table selection (for dine-in) -->
            <div *ngIf="isDineIn" class="space-y-2">
              <p class="text-xs text-neutral-500 tracking-[0.15em] uppercase">{{ 'cartDrawer.selectTable' | translate }}</p>
              <select formControlName="tableNumber"
                      data-cy="table-selection"
                      class="nn-input"
                      [ngClass]="err('tableNumber') ? 'nn-field--error' : ''">
                <option value="" disabled>{{ 'cartDrawer.chooseTable' | translate }}</option>
                <option value="1">{{ 'cartDrawer.tableOne' | translate }}</option>
                <option value="2">{{ 'cartDrawer.tableTwo' | translate }}</option>
                <option value="3">{{ 'cartDrawer.tableThree' | translate }}</option>
                <option value="4">{{ 'cartDrawer.tableFour' | translate }}</option>
                <option value="5">{{ 'cartDrawer.tableFive' | translate }}</option>
                <option value="6">{{ 'cartDrawer.tableSix' | translate }}</option>
              </select>
              <p *ngIf="err('tableNumber')" class="text-xs text-red-400 flex items-center gap-1">
                <iconify-icon icon="solar:danger-circle-linear" width="12"></iconify-icon>
                {{ 'cartDrawer.tableRequired' | translate }}
              </p>
            </div>

            <!-- Dine-in reservation name -->
            <div *ngIf="isDineIn" class="space-y-2">
              <p class="text-xs text-neutral-500 tracking-[0.15em] uppercase">{{ 'cartDrawer.reservationDetails' | translate }}</p>
              <div class="rounded-xl border border-[#C65A1E]/20 bg-[#C65A1E]/5 px-4 py-3 flex items-start gap-3">
                <iconify-icon icon="solar:info-circle-linear" width="14" class="text-[#C65A1E] mt-0.5 shrink-0"></iconify-icon>
                <p class="text-xs text-neutral-400 leading-relaxed">
                  {{ 'cartDrawer.reservationDetailsHint' | translate }}
                </p>
              </div>
              <input formControlName="tableReservationName" type="text"
                     [placeholder]="'cartDrawer.reservationNamePlaceholder' | translate"
                     class="nn-input"
                     [ngClass]="err('tableReservationName') ? 'nn-field--error' : ''">
              <p *ngIf="err('tableReservationName')" class="text-xs text-red-400 flex items-center gap-1">
                <iconify-icon icon="solar:danger-circle-linear" width="12"></iconify-icon>
                {{ 'cartDrawer.reservationNameRequired' | translate }}
              </p>
            </div>

            <!-- Special requests -->
            <div class="space-y-2">
              <p class="text-xs text-neutral-500 tracking-[0.15em] uppercase">
                {{ 'cartDrawer.specialRequests' | translate }} <span class="normal-case text-neutral-700">{{ 'checkout.optional' | translate }}</span>
              </p>
              <textarea formControlName="notes" rows="2"
                        [placeholder]="'cartDrawer.allergiesPlaceholder' | translate"
                        class="nn-input resize-none">
              </textarea>
            </div>

            <div class="h-1"></div>
          </div>

          <!-- Step 2 footer -->
          <div class="px-6 py-5 border-t border-white/5 space-y-3 shrink-0">
            <button (click)="submit()" [disabled]="submitting"
                    data-cy="pay-button"
                    class="w-full py-3.5 text-sm font-medium rounded-xl transition-all duration-300 flex items-center justify-center gap-2"
                    [ngClass]="submitting
                      ? 'bg-neutral-800 text-neutral-500 cursor-not-allowed'
                      : 'bg-white text-[#0a0a0a] hover:bg-[#C65A1E] hover:text-white hover:shadow-[0_0_24px_rgba(198,90,30,0.4)]'">
              <iconify-icon *ngIf="submitting" icon="solar:spinner-line-duotone" width="18" class="animate-spin"></iconify-icon>
              <span>{{ submitting ? ('cartDrawer.placingOrder' | translate) : ('cartDrawer.placeOrder' | translate) }}</span>
              <iconify-icon *ngIf="!submitting" icon="solar:check-circle-linear" width="18"></iconify-icon>
            </button>
            <p class="text-xs text-neutral-600 text-center">{{ 'cartDrawer.confirmByPhone' | translate }}</p>
          </div>
        </ng-container>

        <!-- ══════════ STEP 3 — CONFIRMATION ══════════ -->
        <ng-container *ngIf="step() === 3">
          <div @stepIn class="flex-1 overflow-y-auto px-6 py-10 flex flex-col items-center text-center">

            <!-- Checkmark -->
            <div class="w-20 h-20 rounded-full bg-emerald-500/10 border border-emerald-500/20 flex items-center justify-center mb-6">
              <iconify-icon icon="solar:check-circle-bold" width="44" class="text-emerald-400"></iconify-icon>
            </div>

            <span class="text-[#C65A1E] text-[10px] font-medium tracking-[0.22em] uppercase mb-2">{{ 'order.confirmed' | translate }}</span>
            <h3 class="font-['Forum'] text-2xl text-white tracking-tight mb-3">{{ 'order.receivedTitle' | translate }}</h3>
            <p class="text-sm text-neutral-400 leading-relaxed max-w-xs">
              {{ 'cartDrawer.confirmByPhone' | translate }}
            </p>

            <!-- Order reference -->
            <div *ngIf="confirmedShortId"
                 class="mt-6 inline-flex items-center gap-2 px-4 py-2.5 bg-[#111] border border-white/5 rounded-xl">
              <iconify-icon icon="solar:tag-linear" width="14" class="text-neutral-500"></iconify-icon>
              <span class="text-xs text-neutral-500">{{ 'cartDrawer.orderRef' | translate }}</span>
              <span class="text-xs text-white font-medium font-mono tracking-wider">{{ confirmedShortId }}</span>
            </div>

            <!-- What happens next -->
            <div class="mt-8 w-full bg-[#111] border border-white/5 rounded-2xl p-5 text-left space-y-4">
              <h4 class="text-xs font-medium text-neutral-500 uppercase tracking-widest mb-1">{{ 'cartDrawer.whatNext' | translate }}</h4>
              <div *ngFor="let s of nextSteps; let i = index; trackBy: trackByIndex" class="flex items-start gap-3">
                <div class="w-6 h-6 rounded-full bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center shrink-0 mt-0.5">
                  <span class="text-[10px] font-bold text-[#C65A1E]">{{ i + 1 }}</span>
                </div>
                <div>
                  <p class="text-sm text-white leading-snug">{{ s.title }}</p>
                  <p class="text-xs text-neutral-500 mt-0.5 leading-relaxed">{{ s.desc }}</p>
                </div>
              </div>
            </div>

          </div>

          <!-- Step 3 footer -->
          <div class="px-6 py-5 border-t border-white/5 shrink-0 space-y-3">
            <button (click)="startOver()"
                    data-cy="back-to-menu"
                    class="w-full py-3.5 text-sm font-medium text-[#0a0a0a] bg-white rounded-xl hover:bg-[#C65A1E] hover:text-white hover:shadow-[0_0_20px_rgba(198,90,30,0.35)] transition-all duration-300">
              {{ 'cartDrawer.backToMenu' | translate }}
            </button>
          </div>
        </ng-container>

      </div>
    </ng-container>
  `
})
export class CartDrawerComponent implements OnInit, OnDestroy {
  readonly cart          = inject(CartService);
  private readonly fb    = inject(FormBuilder);
  private readonly api   = inject(ApiService);
  private readonly toast = inject(ToastService);
  private readonly cdr   = inject(ChangeDetectorRef);
  private readonly translate = inject(TranslateService);

  step = signal<DrawerStep>(1);
  form!: FormGroup;
  submitting = false;
  confirmedShortId = '';

  private typeSub?: Subscription;

  get orderTypes() {
    return [
      { value: 'collection', label: this.translate.instant('reservations.pickup'), sub: this.translate.instant('cartDrawer.pickupSub'), icon: 'solar:bag-5-linear' },
      { value: 'delivery', label: this.translate.instant('reservations.delivery'), sub: this.translate.instant('cartDrawer.deliverySub'), icon: 'solar:delivery-linear' },
      { value: 'dine-in', label: this.translate.instant('reservations.dineIn'), sub: this.translate.instant('cartDrawer.dineInSub'), icon: 'solar:tea-cup-linear' }
    ];
  }

  get nextSteps() {
    return [
      { title: this.translate.instant('cartDrawer.nextReviewTitle'), desc: this.translate.instant('cartDrawer.nextReviewDesc') },
      { title: this.translate.instant('cartDrawer.nextCallTitle'), desc: this.translate.instant('cartDrawer.nextCallDesc') },
      { title: this.translate.instant('cartDrawer.nextPreparedTitle'), desc: this.translate.instant('cartDrawer.nextPreparedDesc') }
    ];
  }

  trackByCartItem(_index: number, item: { menuItemId: string }): string { return item.menuItemId; }
  trackByType(_index: number, t: { value: string }): string { return t.value; }
  trackByIndex(_index: number): number { return _index; }

  ngOnInit(): void {
    this.buildForm();
  }

  ngOnDestroy(): void {
    this.typeSub?.unsubscribe();
  }

  private buildForm(): void {
    this.typeSub?.unsubscribe();
    this.form = this.fb.group({
      customerName:         ['', AppValidators.fullName],
      email:                ['', AppValidators.email],
      phoneNumber:          ['', AppValidators.phone],
      type:                 ['collection', AppValidators.required],
      deliveryAddress:      [''],
      pickupTime:           [''],
      tableNumber:          [''],
      tableReservationName: [''],
      notes:                ['', Validators.maxLength(300)]
    });

    this.typeSub = this.form.get('type')!.valueChanges.subscribe(val => {
      const addr  = this.form.get('deliveryAddress')!;
      const table = this.form.get('tableReservationName')!;
      const pickup = this.form.get('pickupTime')!;
      const tableNum = this.form.get('tableNumber')!;
      
      addr.clearValidators();  addr.setValue('');
      table.clearValidators(); table.setValue('');
      pickup.clearValidators(); pickup.setValue('');
      tableNum.clearValidators(); tableNum.setValue('');
      
      if (val === 'delivery') addr.setValidators([Validators.required, Validators.minLength(5)]);
      if (val === 'dine-in') {
        table.setValidators([Validators.required, Validators.minLength(2)]);
        tableNum.setValidators([Validators.required]);
      }
      if (val === 'collection') pickup.setValidators([Validators.required]);
      
      addr.updateValueAndValidity();
      table.updateValueAndValidity();
      pickup.updateValueAndValidity();
      tableNum.updateValueAndValidity();
    });
  }

  get orderType(): string   { return this.form?.get('type')?.value ?? ''; }
  get isDelivery(): boolean { return this.orderType === 'delivery'; }
  get isDineIn():   boolean { return this.orderType === 'dine-in'; }
  get f()                   { return this.form.controls; }

  err(field: string): boolean {
    const c = this.form.get(field);
    return !!(c && c.invalid && c.touched);
  }

  goToStep2(): void  { this.step.set(2); }
  goBack(): void     { this.step.set(1); }

  closeDrawer(): void {
    this.cart.close();
    setTimeout(() => {
      if (this.step() === 3) {
        this.step.set(1);
        this.confirmedShortId = '';
        this.buildForm();
      }
    }, 300);
  }

  startOver(): void {
    this.step.set(1);
    this.confirmedShortId = '';
    this.buildForm();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting = true;
    const val = this.form.value;

    this.api.createOrder({
      customerName:         val.customerName,
      email:                val.email,
      phoneNumber:          val.phoneNumber,
      notes:                val.notes || undefined,
      type:                 val.type,
      deliveryAddress:      val.deliveryAddress      || undefined,
      tableReservationName: val.tableReservationName || undefined,
      items: this.cart.items().map(i => ({
        menuItemId:   i.menuItemId,
        menuItemName: i.name,
        unitPrice:    i.price,
        quantity:     i.quantity
      }))
    }).subscribe({
      next: (res) => {
        this.submitting = false;
        this.confirmedShortId = res.id ? res.id.split('-')[0].toUpperCase() : '';
        this.cart.clear();
        this.step.set(3);
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.submitting = false;
        const msg = err?.error?.errors
          ? Object.values(err.error.errors).flat().join('. ')
          : this.translate.instant('common.tryAgain');
        this.toast.error(msg as string);
        this.cdr.markForCheck();
      }
    });
  }
}
