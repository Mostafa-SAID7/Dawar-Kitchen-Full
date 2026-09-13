import { Component, OnInit, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SeoService } from '@shared/services';
import { AboutComponent } from '../../../home/components/about-section/about.component';
import { ChefsComponent } from '../../../chefs/components/chefs-showcase/chefs.component';
import { RevealDirective } from '@shared/directives/scroll-reveal.directive';

@Component({
  selector: 'app-about-page',
  standalone: true,
  imports: [CommonModule, RouterModule, AboutComponent, ChefsComponent, TranslateModule, RevealDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  template: `
    <!-- Page hero -->
    <div class="pt-32 pb-10 px-6 bg-[#0a0a0a]">
      <div class="max-w-3xl mx-auto text-center space-y-4">
        <span class="text-[#C65A1E] text-xs font-medium tracking-[0.2em] uppercase">{{ 'about.ourStory' | translate }}</span>
        <h1 class="font-['Forum'] text-4xl sm:text-5xl text-white tracking-tight">{{ 'about.aboutTitle' | translate }}</h1>
        <p class="text-neutral-400 text-sm sm:text-base leading-relaxed font-light max-w-xl mx-auto">
          {{ 'app.description' | translate }}
        </p>
      </div>
    </div>

    <!-- Full about section (story + features) -->
    <app-about [standalone]="true"></app-about>

    <!-- Values strip -->
    <section class="py-16 px-6 bg-[#0d0d0d]">
      <div class="max-w-5xl mx-auto">
        <div reveal="true" [revealDelay]="0" [revealFrom]="'bottom'" class="text-center mb-12">
          <span class="text-[#C65A1E] text-xs font-medium tracking-[0.2em] uppercase mb-3 block">{{ 'about.missionTitle' | translate }}</span>
          <h2 class="font-['Forum'] text-3xl sm:text-4xl text-white tracking-tight">{{ 'about.whyWeExist' | translate }}</h2>
        </div>
        <div class="max-w-3xl mx-auto space-y-6 mb-12">
          <p class="text-neutral-300 text-sm leading-relaxed font-light">
            Dawar Kitchen is located in Ezbet Khairallah, one of Cairo's largest informal settlements. Through catering and food production, we provide dignified employment and vocational training for migrant, refugee, and Egyptian women. We pride ourselves on fair working conditions and a participatory approach that ensures worker engagement in core decision-making and business planning.
          </p>
          <p class="text-neutral-300 text-sm leading-relaxed font-light">
            We celebrate food heritage and quality cuisine. We choose the finest and freshest ingredients, using locally sourced produce wherever possible. Our customers include embassies, NGOs, private businesses, and the general public across Cairo.
          </p>
          <p class="text-neutral-300 text-sm leading-relaxed font-light">
            Founded as part of Dawar for Arts and Development, the kitchen began deliveries in April 2018. It empowers women with skills in commercial food production, small business management, and creates bridge-building and cross-cultural exchange through the universal language of food.
          </p>
        </div>
        <div reveal="true" [revealDelay]="80" [revealFrom]="'bottom'" class="grid grid-cols-1 sm:grid-cols-3 gap-6">
          <div *ngFor="let v of values" class="p-6 rounded-2xl bg-[#111] border border-white/5 space-y-3">
            <div class="w-10 h-10 rounded-lg bg-[#C65A1E]/10 border border-[#C65A1E]/20 flex items-center justify-center text-[#C65A1E]">
              <iconify-icon [attr.icon]="v.icon" width="22"></iconify-icon>
            </div>
            <h3 class="font-['Forum'] text-lg text-white">{{ v.title }}</h3>
            <p class="text-xs text-neutral-400 leading-relaxed font-light">{{ v.description }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Chefs -->
    <app-chefs></app-chefs>

    <!-- CTA -->
    <section class="py-16 px-6 bg-[#0d0d0d] text-center">
      <div class="max-w-xl mx-auto space-y-5">
        <h2 class="font-['Forum'] text-3xl text-white">{{ 'about.readyToSupport' | translate }}</h2>
        <p class="text-sm text-neutral-400 font-light leading-relaxed">{{ 'about.supportMessage' | translate }}</p>
        <div class="flex flex-col sm:flex-row gap-3 justify-center">
          <a routerLink="/reservations"
             class="px-8 py-3.5 text-sm font-medium text-white bg-[#C65A1E] rounded-xl hover:bg-[#a84915] hover:shadow-[0_0_28px_rgba(198,90,30,0.4)] transition-all duration-300">
            {{ 'about.orderNow' | translate }}
          </a>
          <a routerLink="/menu"
             class="px-8 py-3.5 text-sm font-medium text-white border border-white/20 rounded-xl hover:bg-white/5 transition-all duration-300">
            {{ 'about.exploreMenu' | translate }}
          </a>
        </div>
      </div>
    </section>
  `
})
export class AboutPageComponent implements OnInit {
  private readonly seo       = inject(SeoService);
  private readonly translate = inject(TranslateService);

  get values() {
    return [
      {
        icon: 'solar:fire-bold',
        title: this.translate.instant('about.recipesTitle'),
        description: this.translate.instant('about.recipesDesc')
      },
      {
        icon: 'solar:leaf-bold',
        title: this.translate.instant('about.ingredientsTitle'),
        description: this.translate.instant('about.ingredientsDesc')
      },
      {
        icon: 'solar:heart-bold',
        title: this.translate.instant('about.fairWorkTitle'),
        description: this.translate.instant('about.fairWorkDesc')
      }
    ];
  }

  ngOnInit(): void {
    this.seo.set({
      title:        'About | Dawar Kitchen',
      description:  'Learn about Dawar Kitchen — a social enterprise celebrating authentic Egyptian & Syrian cuisine while empowering Syrian and Egyptian women. Based in Ezbet Khairallah, Cairo.',
      keywords:     'about Dawar Kitchen, Egyptian Syrian restaurant, social enterprise Cairo, women chefs, authentic cuisine, fair trade food Cairo',
      canonicalUrl: 'https://www.dawarkitchen.com/about',
      ogUrl:        'https://www.dawarkitchen.com/about',
    });
  }
}
