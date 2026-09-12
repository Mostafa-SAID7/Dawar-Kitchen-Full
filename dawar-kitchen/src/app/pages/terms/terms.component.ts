import { Component, OnInit, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { SeoService } from '../../services/seo.service';

@Component({
  selector: 'app-terms-page',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  template: `
    <div class="min-h-screen pt-32 pb-20 px-6 bg-[#0a0a0a]">
      <div class="max-w-3xl mx-auto">

        <!-- Header -->
        <div class="mb-12 space-y-3">
          <span class="text-[#C65A1E] text-xs font-medium tracking-[0.2em] uppercase">{{ 'terms.legal' | translate }}</span>
          <h1 class="font-['Forum'] text-4xl sm:text-5xl text-white tracking-tight">{{ 'terms.title' | translate }}</h1>
          <p class="text-sm text-neutral-500 font-light">{{ 'terms.lastUpdated' | translate }}</p>
        </div>

        <!-- Intro -->
        <p class="text-sm text-neutral-400 leading-relaxed font-light mb-10">
          {{ 'terms.intro' | translate }}
        </p>

        <div class="space-y-10">

          <!-- Section 1 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:global-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section1Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'terms.section1Text' | translate }}
            </p>
          </div>

          <!-- Section 2 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:calendar-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section2Title' | translate }}</h2>
            </div>
            <ul class="text-sm text-neutral-400 font-light leading-relaxed space-y-2 pl-1">
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section2Item1' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section2Item2' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section2Item3' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section2Item4' | translate }}</span></li>
            </ul>
          </div>

          <!-- Section 3 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:bag-5-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section3Title' | translate }}</h2>
            </div>
            <ul class="text-sm text-neutral-400 font-light leading-relaxed space-y-2 pl-1">
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section3Item1' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section3Item2' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section3Item3' | translate }}</span></li>
              <li class="flex gap-2"><span class="text-[#C65A1E] mt-1">–</span><span>{{ 'terms.section3Item4' | translate }}</span></li>
            </ul>
          </div>

          <!-- Section 4 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:user-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section4Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'terms.section4Text' | translate }}
              <a href="mailto:info@dawarkitchen.com" class="text-[#C65A1E] hover:underline">info&#64;dawarkitchen.com</a>
              {{ 'terms.section4TextCont' | translate }}
            </p>
          </div>

          <!-- Section 5 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:copyright-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section5Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'terms.section5Text' | translate }}
            </p>
          </div>

          <!-- Section 6 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:danger-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section6Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'terms.section6Text' | translate }}
            </p>
          </div>

          <!-- Section 7 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:map-point-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section7Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'terms.section7Text' | translate }}
            </p>
          </div>

          <!-- Section 8 -->
          <div class="p-6 rounded-2xl bg-[#0d0d0d] border border-white/5 space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E] shrink-0">
                <iconify-icon icon="solar:refresh-bold" width="16"></iconify-icon>
              </div>
              <h2 class="font-['Forum'] text-xl text-white">{{ 'terms.section8Title' | translate }}</h2>
            </div>
            <p class="text-sm text-neutral-400 font-light leading-relaxed">
              {{ 'terms.section8Text' | translate }}
            </p>
          </div>

        </div>

        <!-- Contact CTA -->
        <div class="mt-12 p-6 rounded-2xl bg-[#0d0d0d] border border-[#C65A1E]/20 text-center space-y-4">
          <h3 class="font-['Forum'] text-xl text-white">{{ 'terms.questionsTitle' | translate }}</h3>
          <p class="text-sm text-neutral-400 font-light">
            {{ 'terms.questionsText' | translate }}
          </p>
          <a routerLink="/contact"
             class="inline-block px-8 py-3 text-sm font-medium text-white bg-[#C65A1E] rounded-xl hover:bg-[#a84915] hover:shadow-[0_0_24px_rgba(198,90,30,0.4)] transition-all duration-300">
            {{ 'contact.getInTouch' | translate }}
          </a>
        </div>

        <!-- Back link -->
        <div class="mt-8 text-center">
          <a routerLink="/" class="text-xs text-neutral-600 hover:text-neutral-400 transition-colors">
            {{ 'terms.backHome' | translate }}
          </a>
        </div>

      </div>
    </div>
  `
})
export class TermsPageComponent implements OnInit {
  private readonly seo = inject(SeoService);

  ngOnInit(): void {
    this.seo.set({
      title:        'Terms of Service',
      description:  'Read the Terms of Service for Dawar Kitchen. Understand the rules and conditions for using our website, placing orders, and receiving delivery.',
      keywords:     'terms of service, Dawar Kitchen terms, order terms, delivery terms, Cairo food delivery',
      canonicalUrl: 'https://www.dawarkitchen.com/terms',
      ogUrl:        'https://www.dawarkitchen.com/terms',
    });
  }
}
