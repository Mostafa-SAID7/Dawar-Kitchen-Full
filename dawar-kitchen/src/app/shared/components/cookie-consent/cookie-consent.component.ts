import { Component, OnInit, signal, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { StorageUtil } from '../../utils';

@Component({
  selector: 'app-cookie-consent',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  template: `
    <div *ngIf="show()"
         class="fixed bottom-4 left-4 right-4 md:left-auto md:right-4 md:w-96 p-4 rounded-2xl bg-[#0d0d0d] border border-white/10 shadow-[0_24px_80px_rgba(0,0,0,0.6)] z-[400] flex flex-col gap-4 animate-slide-up">
      <div class="flex items-start gap-3">
        <div class="p-2 rounded-full bg-[#C65A1E]/10 text-[#C65A1E]">
          <iconify-icon icon="solar:cookie-bold" width="24"></iconify-icon>
        </div>
        <div>
          <h3 class="text-white font-medium text-sm">{{ 'app.cookieTitle' | translate }}</h3>
          <p class="text-neutral-400 text-xs mt-1 leading-relaxed">{{ 'app.cookieMessage' | translate }}</p>
        </div>
      </div>
      <div class="flex justify-end gap-3 mt-1">
        <button (click)="accept()" class="px-5 py-2 text-xs font-medium text-white bg-[#C65A1E] hover:bg-[#a84915] rounded-xl transition-colors">
          {{ 'app.cookieAccept' | translate }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    @keyframes slideUp {
      from { opacity: 0; transform: translateY(24px) scale(0.97); }
      to   { opacity: 1; transform: translateY(0) scale(1); }
    }
    .animate-slide-up { animation: slideUp 0.3s cubic-bezier(0.32,0.72,0,1) both; }
  `]
})
export class CookieConsentComponent implements OnInit {
  show = signal(false);
  private readonly CONSENT_KEY = 'dawar_cookie_consent';

  ngOnInit(): void {
    // Small delay so it doesn't pop up immediately on first paint
    setTimeout(() => {
      const consent = StorageUtil.get(this.CONSENT_KEY);
      if (!consent) {
        this.show.set(true);
      }
    }, 1500);
  }

  accept(): void {
    StorageUtil.set(this.CONSENT_KEY, 'true');
    this.show.set(false);
  }
}
