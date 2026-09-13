import { Component, OnInit, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { SeoService } from '@shared/services';

@Component({
  selector: 'app-privacy-page',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  template: `
    <div class="min-h-screen pt-32 pb-20 px-6 bg-[#0a0a0a]">
      <div class="max-w-3xl mx-auto">

        <!-- Header -->
        <div class="mb-12 space-y-3">
          <span class="text-[#C65A1E] text-xs font-medium tracking-[0.2em] uppercase">{{ 'privacy.legal' | translate }}</span>
          <h1 class="font-['Forum'] text-4xl sm:text-5xl text-white tracking-tight">{{ 'privacy.title' | translate }}</h1>
          <p class="text-sm text-neutral-500 font-light">{{ 'privacy.lastUpdated' | translate }}</p>
        </div>

        <!-- Intro -->
        <p class="text-sm text-neutral-400 leading-relaxed font-light mb-10">
          {{ 'privacy.intro' | translate }}
        </p>

        <div class="space-y-10">

          <!-- Section 1 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:user-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'privacy.section1Title' | translate }}</h2>
            </div>
            <ul class="text-sm text-neutral-400 font-light leading-relaxed space-y-2 pl-1">
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span><strong class="text-neutral-300 font-medium">{{ 'privacy.section1Account' | translate }}</strong></span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span><strong class="text-neutral-300 font-medium">{{ 'privacy.section1Reservation' | translate }}</strong></span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span><strong class="text-neutral-300 font-medium">{{ 'privacy.section1Order' | translate }}</strong></span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span><strong class="text-neutral-300 font-medium">{{ 'privacy.section1Contact' | translate }}</strong></span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span><strong class="text-neutral-300 font-medium">{{ 'privacy.section1Technical' | translate }}</strong></span></li>
            </ul>
          </div>

          <!-- Section 2 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:shield-check-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'privacy.section2Title' | translate }}</h2>
            </div>
            <ul class="text-sm text-neutral-400 font-light leading-relaxed space-y-2 pl-1">
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section2Item1' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section2Item2' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section2Item3' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section2Item4' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section2Item5' | translate }}</span></li>
            </ul>
          </div>

          <!-- Section 3 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:share-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'privacy.section3Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'privacy.section3Text' | translate }}
            </p>
          </div>

          <!-- Section 4 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:lock-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'privacy.section4Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'privacy.section4Text' | translate }}
            </p>
          </div>

          <!-- Section 5 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:star-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'privacy.section5Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed mb-3">
              {{ 'privacy.section5Intro' | translate }}
            </p>
            <ul class="text-sm text-neutral-400 font-light leading-relaxed space-y-2 pl-1">
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section5Item1' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section5Item2' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section5Item3' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section5Item4' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'privacy.section5Item5' | translate }}</span></li>
            </ul>
            <p class="text-sm text-neutral-400 font-light leading-relaxed mt-3">
              {{ 'privacy.section5ContactText' | translate }}
              <a href="mailto:info@dawarkitchen.com" class="text-[#C65A1E] hover:underline">info&#64;dawarkitchen.com</a>.
            </p>
          </div>

          <!-- Section 6 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:clock-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'privacy.section6Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'privacy.section6Text' | translate }}
            </p>
          </div>

          <!-- Section 7 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:refresh-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'privacy.section7Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'privacy.section7Text' | translate }}
            </p>
          </div>

        </div>

        <!-- Contact CTA -->
        <div class="mt-12 p-6 rounded-2xl bg-[#0d0d0d] border border-[#C65A1E]/20 text-center space-y-4">
          <h3 class="font-['Forum'] text-xl text-white">{{ 'privacy.questionsTitle' | translate }}</h3>
          <p class="text-sm text-neutral-400 font-light">
            {{ 'privacy.questionsText' | translate }}
          </p>
          <a routerLink="/contact"
             class="inline-block px-8 py-3 text-sm font-medium text-white bg-[#C65A1E] rounded-xl hover:bg-[#a84915] hover:shadow-[0_0_24px_rgba(198,90,30,0.4)] transition-all duration-300">
            {{ 'contact.getInTouch' | translate }}
          </a>
        </div>

        <!-- Back link -->
        <div class="mt-8 text-center">
          <a routerLink="/" class="text-xs text-neutral-600 hover:text-neutral-400 transition-colors">
            {{ 'privacy.backHome' | translate }}
          </a>
        </div>

      </div>
    </div>
  `
})
export class PrivacyPageComponent implements OnInit {
  private readonly seo = inject(SeoService);

  ngOnInit(): void {
    this.seo.set({
      title:        'Privacy Policy',
      description:  'Learn how Dawar Kitchen collects, uses, and protects your personal information. Read our full privacy policy.',
      keywords:     'privacy policy, Dawar Kitchen privacy, data protection, Cairo food delivery',
      canonicalUrl: 'https://www.dawarkitchen.com/privacy',
      ogUrl:        'https://www.dawarkitchen.com/privacy',
    });
  }
}
