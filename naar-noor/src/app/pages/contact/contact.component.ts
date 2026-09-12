import { Component, OnInit, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SeoService } from '../../services/seo.service';
import { CustomDropdownComponent } from '../../components/custom-dropdown/custom-dropdown.component';

@Component({
  selector: 'app-contact-page',
  standalone: true,
  imports: [CommonModule, FormsModule, CustomDropdownComponent, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  template: `
    <div class="min-h-screen pt-32 pb-20 px-6 bg-[#0a0a0a]">
      <div class="max-w-5xl mx-auto">

        <!-- Header -->
        <div class="text-center mb-14 space-y-4">
          <span class="text-[#C65A1E] text-xs font-medium tracking-[0.2em] uppercase">{{ 'contact.getInTouch' | translate }}</span>
          <h1 class="font-['Forum'] text-4xl sm:text-5xl text-white tracking-tight">{{ 'contact.contactDawarKitchen' | translate }}</h1>
          <p class="text-sm text-neutral-400 leading-relaxed font-light max-w-md mx-auto">
            {{ 'contact.description' | translate }}
          </p>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-5 gap-8">

          <!-- Info column -->
          <div class="lg:col-span-2 space-y-6">

            <!-- Address -->
            <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-3">
              <div class="flex items-center gap-2 text-[#C65A1E]">
                <iconify-icon icon="solar:map-point-bold" width="18"></iconify-icon>
                <span class="text-[10px] font-medium tracking-widest uppercase text-neutral-400">{{ 'footer.address' | translate }}</span>
              </div>
              <p class="text-sm text-neutral-300 font-light leading-relaxed">
                5 Magdy El-Khouly,<br/>
                Ezbet Khairallah, Old Cairo,<br/>
                Cairo Governorate, Egypt
              </p>
            </div>

            <!-- Hours -->
            <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-3">
              <div class="flex items-center gap-2 text-[#C65A1E]">
                <iconify-icon icon="solar:clock-circle-bold" width="18"></iconify-icon>
                <span class="text-[10px] font-medium tracking-widest uppercase text-neutral-400">{{ 'contact.openingHours' | translate }}</span>
              </div>
              <div class="grid grid-cols-2 gap-x-4 gap-y-1.5 text-xs text-neutral-300 font-light">
                <span class="text-neutral-500">{{ 'contact.production' | translate }}</span>  <span>9:00 AM – 5:00 PM</span>
                <span class="text-neutral-500">{{ 'contact.deliveryHours' | translate }}</span>  <span>10:00 AM – 8:00 PM</span>
                <span class="text-neutral-500">{{ 'contact.days' | translate }}</span>  <span>Monday – Sunday</span>
              </div>
            </div>

            <!-- Phone & Email -->
            <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
              <div class="flex items-center gap-3">
                <iconify-icon icon="solar:phone-bold" width="18" class="text-[#C65A1E] shrink-0"></iconify-icon>
                <span class="text-sm text-neutral-300 font-light">+20 10 33737764</span>
              </div>
              <div class="flex items-center gap-3">
                <iconify-icon icon="solar:letter-bold" width="18" class="text-[#C65A1E] shrink-0"></iconify-icon>
                <span class="text-sm text-neutral-300 font-light">info&#64;dawarkitchen.com</span>
              </div>
            </div>

          </div>

          <!-- Contact form -->
          <div class="lg:col-span-3 p-8 rounded-2xl bg-[#0d0d0d] border border-white/5">

            <div *ngIf="sent" class="text-center py-12 space-y-4">
              <div class="w-16 h-16 mx-auto rounded-full bg-emerald-500/10 border border-emerald-500/20 flex items-center justify-center text-emerald-400 text-3xl">✓</div>
              <h3 class="font-['Forum'] text-2xl text-white">{{ 'contact.messageSent' | translate }}</h3>
              <p class="text-sm text-neutral-400 font-light">{{ 'contact.willBeInTouch' | translate }}</p>
              <button (click)="sent = false"
                      class="mt-4 px-6 py-2.5 text-sm text-white border border-white/15 rounded-xl hover:bg-white/5 transition-all">
                {{ 'contact.sendAnother' | translate }}
              </button>
            </div>

            <form *ngIf="!sent" (ngSubmit)="submit()" class="space-y-5" #contactForm="ngForm">
              <h2 class="font-['Forum'] text-2xl text-white mb-6">{{ 'contact.sendMessage' | translate }}</h2>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
                <div class="space-y-1.5">
                  <label class="text-xs font-medium text-neutral-400 uppercase">{{ 'contact.name' | translate }}</label>
                  <input
                    type="text"
                    name="name"
                    [(ngModel)]="form.name"
                    [placeholder]="'contact.yourName' | translate"
                    required
                    class="nn-input"
                  />
                </div>
                <div class="space-y-1.5">
                  <label class="text-xs font-medium text-neutral-400 uppercase">{{ 'contact.email' | translate }}</label>
                  <input
                    type="email"
                    name="email"
                    [(ngModel)]="form.email"
                    [placeholder]="'contact.yourEmail' | translate"
                    required
                    class="nn-input"
                  />
                </div>
              </div>

              <div class="space-y-1.5">
                <label class="text-xs font-medium text-neutral-400 uppercase">{{ 'contact.subject' | translate }}</label>
                <app-custom-dropdown
                  [options]="subjectOptions"
                  [selectedValue]="subjectDisplay"
                  [placeholder]="'common.selectOption' | translate"
                  [icon]="'solar:tag-linear'"
                  (valueSelected)="onSubjectSelected($event)">
                </app-custom-dropdown>
              </div>

              <div class="space-y-1.5">
                <label class="text-xs font-medium text-neutral-400 uppercase">{{ 'contact.message' | translate }}</label>
                <textarea
                  name="message"
                  [(ngModel)]="form.message"
                  rows="5"
                  [placeholder]="'contact.tellUsMore' | translate"
                  required
                  class="nn-input resize-none"
                ></textarea>
              </div>

              <button
                type="submit"
                [disabled]="submitting || !contactForm.valid"
                class="w-full py-3.5 text-sm font-medium text-white bg-[#C65A1E] rounded-xl hover:bg-[#a84915] hover:shadow-[0_0_24px_rgba(198,90,30,0.4)] transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed">
                <span *ngIf="!submitting">{{ 'contact.submit' | translate }}</span>
                <span *ngIf="submitting" class="flex items-center justify-center gap-2">
                  <span class="inline-block w-4 h-4 border-2 border-current border-t-transparent rounded-full animate-spin"></span>
                  {{ 'common.loading' | translate }}
                </span>
              </button>
            </form>

          </div>
        </div>

      </div>
    </div>
  `
})
export class ContactPageComponent implements OnInit {
  private readonly seo = inject(SeoService);
  private readonly translate = inject(TranslateService);

  form = { name: '', email: '', subject: '', message: '' };
  submitting = false;
  sent = false;

  get subjectOptions(): string[] {
    return [
      this.translate.instant('contact.subjectReservation'),
      this.translate.instant('contact.subjectPrivateEvent'),
      this.translate.instant('contact.subjectFeedback'),
      this.translate.instant('contact.subjectOther')
    ];
  }

  submit(): void {
    if (!this.form.name || !this.form.email || !this.form.message) return;
    this.submitting = true;
    setTimeout(() => {
      this.submitting = false;
      this.sent = true;
      this.form = { name: '', email: '', subject: '', message: '' };
    }, 800);
  }

  ngOnInit(): void {
    this.seo.set({
      title:        'Contact Dawar Kitchen',
      description:  'Contact Dawar Kitchen. Find our address in Ezbet Khairallah, Cairo. Call +20 10 33737764 or email for orders, catering, or inquiries about our social enterprise.',
      keywords:     'contact Dawar Kitchen, Cairo restaurant, Egyptian Syrian cuisine, Ezbet Khairallah, food delivery Cairo, restaurant phone',
      canonicalUrl: 'https://www.dawarkitchen.com/contact',
      ogUrl:        'https://www.dawarkitchen.com/contact',
    });
  }

  onSubjectSelected(subject: string): void {
    // Map translated display names back to internal values
    const reverseMap: { [key: string]: string } = {};
    reverseMap[this.translate.instant('contact.subjectReservation')] = 'reservation';
    reverseMap[this.translate.instant('contact.subjectPrivateEvent')] = 'private';
    reverseMap[this.translate.instant('contact.subjectFeedback')] = 'feedback';
    reverseMap[this.translate.instant('contact.subjectOther')] = 'other';
    
    this.form.subject = reverseMap[subject] || '';
  }

  get subjectDisplay(): string {
    const displayMap: { [key: string]: string } = {
      'reservation': this.translate.instant('contact.subjectReservation'),
      'private': this.translate.instant('contact.subjectPrivateEvent'),
      'feedback': this.translate.instant('contact.subjectFeedback'),
      'other': this.translate.instant('contact.subjectOther')
    };
    return displayMap[this.form.subject] || '';
  }
}
