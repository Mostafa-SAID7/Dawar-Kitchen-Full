import { Injectable, inject } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { SeoConfig } from '../models';

export type { SeoConfig };

const BASE      = 'Dawar Kitchen';
const SITE_URL  = 'https://www.dawarkitchen.com';
const DEF_DESC  = 'Authentic Egyptian & Syrian cuisine from Dawar Kitchen. Social enterprise delivering home-style meals across Cairo.';
const DEF_IMG   = `${SITE_URL}/assets/hero/hero.webp`;
const DEF_KW    = 'Dawar Kitchen, Egyptian Syrian cuisine, Cairo delivery, social enterprise, home-style meals, authentic food, Cairo restaurant, Ezbet Khairallah';

@Injectable({ providedIn: 'root' })
export class SeoService {
  private readonly titleService = inject(Title);
  private readonly meta         = inject(Meta);

  set(config: SeoConfig): void {
    const fullTitle = config.title === BASE ? BASE : `${config.title} | ${BASE}`;
    const desc      = config.description ?? DEF_DESC;
    const image     = config.ogImage     ?? DEF_IMG;
    const type      = config.ogType      ?? 'website';
    const url       = config.ogUrl       ?? SITE_URL;
    const keywords  = config.keywords    ?? DEF_KW;

    this.titleService.setTitle(fullTitle);

    this.meta.updateTag({ name: 'description',        content: desc });
    this.meta.updateTag({ name: 'keywords',           content: keywords });
    this.meta.updateTag({ property: 'og:title',       content: fullTitle });
    this.meta.updateTag({ property: 'og:description', content: desc });
    this.meta.updateTag({ property: 'og:image',       content: image });
    this.meta.updateTag({ property: 'og:type',        content: type });
    this.meta.updateTag({ property: 'og:url',         content: url });
    this.meta.updateTag({ property: 'og:site_name',   content: BASE });
    this.meta.updateTag({ property: 'og:locale',      content: 'en_GB' });
    this.meta.updateTag({ name: 'twitter:card',        content: 'summary_large_image' });
    this.meta.updateTag({ name: 'twitter:title',       content: fullTitle });
    this.meta.updateTag({ name: 'twitter:description', content: desc });
    this.meta.updateTag({ name: 'twitter:image',       content: image });

    if (config.noIndex) {
      this.meta.updateTag({ name: 'robots', content: 'noindex, nofollow' });
    } else {
      this.meta.updateTag({ name: 'robots', content: 'index, follow' });
    }

    if (config.canonicalUrl) {
      let link = document.querySelector<HTMLLinkElement>('link[rel="canonical"]');
      if (!link) {
        link = document.createElement('link');
        link.rel = 'canonical';
        document.head.appendChild(link);
      }
      link.href = config.canonicalUrl;
    }
  }

  setHome(): void {
    this.set({
      title:       'Dawar Kitchen — Authentic Egyptian & Syrian Cuisine',
      description: 'Dawar Kitchen: Social enterprise delivering authentic Egyptian & Syrian home-style meals across Cairo. Celebrating food heritage and fair work.',
      keywords:    'Dawar Kitchen, Egyptian Syrian cuisine, Cairo delivery, social enterprise, authentic meals, Ezbet Khairallah, Egyptian food, Damascus cuisine',
      canonicalUrl: `${SITE_URL}/`,
      ogUrl:        `${SITE_URL}/`,
      ogType:       'restaurant',
    });
  }

  setCheckout(): void {
    this.set({
      title:    'Checkout',
      description: 'Complete your Dawar Kitchen order. Choose delivery across Cairo and confirm your authentic Egyptian & Syrian meal order.',
      noIndex:  true,
    });
  }

  setOrderConfirmed(): void {
    this.set({
      title:    'Order Confirmed',
      description: 'Your Dawar Kitchen order has been received. We will confirm shortly by phone. Thank you for supporting our social enterprise!',
      noIndex:  true,
    });
  }
}
