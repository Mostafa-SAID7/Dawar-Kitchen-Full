import { Component, CUSTOM_ELEMENTS_SCHEMA, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CartService } from '@features/checkout/services/cart.service';
import { ApiService } from '@core/http/api.service';
import { ToastService } from '@shared/services';
import { SeoService } from '@shared/services';
import { CustomDropdownComponent } from '@shared/components/custom-dropdown/custom-dropdown.component';
import { AppValidators } from '@shared/validators';
import { PricePipe } from '@shared/pipes';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, CustomDropdownComponent, TranslateModule, PricePipe],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './checkout.component.html'
})
export class CheckoutComponent implements OnInit {
  private readonly fb        = inject(FormBuilder);
  readonly cart              = inject(CartService);
  private readonly api       = inject(ApiService);
  private readonly router    = inject(Router);
  private readonly toast     = inject(ToastService);
  private readonly seo       = inject(SeoService);
  private readonly translate = inject(TranslateService);

  form!: FormGroup;
  submitting = false;

  get orderTypeOptions(): string[] {
    return [
      this.translate.instant('reservations.pickup'),
      this.translate.instant('reservations.delivery'),
      this.translate.instant('reservations.dineIn')
    ];
  }

  ngOnInit(): void {
    this.seo.setCheckout();
    if (this.cart.isEmpty()) {
      this.router.navigate(['/']);
      return;
    }

    this.form = this.fb.group({
      customerName:         ['', AppValidators.fullName],
      email:                ['', AppValidators.email],
      phoneNumber:          ['', AppValidators.phone],
      type:                 ['pickup', AppValidators.required],
      deliveryAddress:      [''],
      tableReservationName: [''],
      notes:                ['', Validators.maxLength(300)]
    });

    this.form.get('type')!.valueChanges.subscribe(val => {
      const addr  = this.form.get('deliveryAddress')!;
      const table = this.form.get('tableReservationName')!;

      addr.clearValidators();
      addr.setValue('');
      table.clearValidators();
      table.setValue('');

      if (val === 'delivery') {
        addr.setValidators([Validators.required, Validators.minLength(5)]);
      } else if (val === 'dine-in') {
        table.setValidators([Validators.required, Validators.minLength(2)]);
      }

      addr.updateValueAndValidity();
      table.updateValueAndValidity();
    });
  }

  get orderType(): string { return this.form?.get('type')?.value ?? ''; }
  get isDelivery(): boolean { return this.orderType === 'delivery'; }
  get isDineIn():   boolean { return this.orderType === 'dine-in'; }
  get isPickup():   boolean { return this.orderType === 'pickup'; }

  get f() { return this.form.controls; }

  err(field: string): boolean {
    const c = this.form.get(field);
    return !!(c && c.invalid && (c.touched || c.dirty));
  }

  updateQty(itemId: string, event: Event): void {
    const input = event.target as HTMLInputElement;
    const val = parseInt(input.value, 10);
    this.cart.setQuantity(itemId, val);
  }

  /** Builds the absolute URL for Stripe redirect callbacks. */
  private appUrl(): string {
    return window.location.origin;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting = true;
    const val = this.form.value;

    // Map 'pickup' → 'collection' for the backend
    const apiType = val.type === 'pickup' ? 'collection' : val.type;

    const origin = this.appUrl();

    this.api.createCheckoutSession({
      customerName:         val.customerName,
      email:                val.email,
      phoneNumber:          val.phoneNumber,
      notes:                val.notes || undefined,
      type:                 apiType as 'collection' | 'delivery' | 'dine-in',
      deliveryAddress:      val.deliveryAddress  || undefined,
      tableReservationName: val.tableReservationName || undefined,
      items: this.cart.items().map(i => ({
        menuItemId:   i.menuItemId,
        menuItemName: i.name,
        unitPrice:    i.price,
        quantity:     i.quantity
      })),
      successUrl: `${origin}/payment-success`,
      cancelUrl:  `${origin}/payment-cancelled`
    }).subscribe({
      next: (res: any) => {
        // Clear cart immediately — Stripe redirect takes the user off-site
        this.cart.clear();
        // Hard-navigate to Stripe hosted checkout page
        window.location.href = res.sessionUrl;
      },
      error: (err: any) => {
        this.submitting = false;
        const msg = err?.error?.errors
          ? Object.values(err.error.errors).flat().join('. ')
          : err?.error?.title ?? this.translate.instant('common.tryAgain');
        this.toast.error(msg as string);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/menu']);
  }

  onOrderTypeSelected(orderType: string): void {
    // Map translated display names back to internal values
    const reverseMap: { [key: string]: string } = {};
    reverseMap[this.translate.instant('reservations.pickup')] = 'pickup';
    reverseMap[this.translate.instant('reservations.delivery')] = 'delivery';
    reverseMap[this.translate.instant('reservations.dineIn')] = 'dine-in';
    
    const value = reverseMap[orderType] || 'pickup';
    this.form.get('type')?.setValue(value);
  }

  onTableSelected(table: string): void {
    this.form.get('tableReservationName')?.setValue(table);
  }

  get orderTypeDisplay(): string {
    const displayMap: { [key: string]: string } = {
      'pickup': this.translate.instant('reservations.pickup'),
      'delivery': this.translate.instant('reservations.delivery'),
      'dine-in': this.translate.instant('reservations.dineIn')
    };
    const value = this.form?.get('type')?.value || 'pickup';
    return displayMap[value] || this.translate.instant('reservations.pickup');
  }
}
