import { Injectable, inject } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { DOCUMENT } from '@angular/common';
import { SeoConfig } from '../models';
import {
  SEO_PAGES,
  SEO_BASE_NAME,
  SITE_URL,
  APP_DESCRIPTION,
  APP_KEYWORDS,
  APP_LOCALE,
  SITE_OG_IMAGE
} from '../constants';

export type { SeoConfig };

@Injectable({ providedIn: 'root' })
export class SeoService {
  private readonly titleService = inject(Title);
  private readonly meta         = inject(Meta);
  private readonly document     = inject(DOCUMENT);

  // ─── Core ────────────────────────────────────────────────────────────────────

  set(config: SeoConfig): void {
    const fullTitle = config.title === SEO_BASE_NAME
      ? SEO_BASE_NAME
      : `${config.title} | ${SEO_BASE_NAME}`;
    const desc     = config.description ?? APP_DESCRIPTION;
    const image    = config.ogImage     ?? SITE_OG_IMAGE;
    const type     = config.ogType      ?? 'website';
    const url      = config.ogUrl       ?? SITE_URL;
    const keywords = config.keywords    ?? APP_KEYWORDS;

    // ─── Title ───────────────────────────────────────────────────────────────
    this.titleService.setTitle(fullTitle);

    // ─── Standard meta ───────────────────────────────────────────────────────
    this.meta.updateTag({ name: 'description',        content: desc });
    this.meta.updateTag({ name: 'keywords',           content: keywords });

    // ─── Robots ──────────────────────────────────────────────────────────────
    this.meta.updateTag({
      name: 'robots',
      content: config.noIndex ? 'noindex, nofollow' : 'index, follow'
    });

    // ─── Open Graph ──────────────────────────────────────────────────────────
    this.meta.updateTag({ property: 'og:title',       content: fullTitle });
    this.meta.updateTag({ property: 'og:description', content: desc });
    this.meta.updateTag({ property: 'og:image',       content: image });
    this.meta.updateTag({ property: 'og:type',        content: type });
    this.meta.updateTag({ property: 'og:url',         content: url });
    this.meta.updateTag({ property: 'og:site_name',   content: SEO_BASE_NAME });
    this.meta.updateTag({ property: 'og:locale',      content: APP_LOCALE });

    // ─── Twitter Card ────────────────────────────────────────────────────────
    this.meta.updateTag({ name: 'twitter:card',        content: 'summary_large_image' });
    this.meta.updateTag({ name: 'twitter:title',       content: fullTitle });
    this.meta.updateTag({ name: 'twitter:description', content: desc });
    this.meta.updateTag({ name: 'twitter:image',       content: image });

    // ─── Canonical (Angular DOCUMENT token — SSR-safe, no raw DOM) ───────────
    if (config.canonicalUrl) {
      let link = this.document.querySelector<HTMLLinkElement>('link[rel="canonical"]');
      if (!link) {
        link = this.document.createElement('link');
        link.rel = 'canonical';
        this.document.head.appendChild(link);
      }
      link.href = config.canonicalUrl;
    }
  }

  // ─── Page helpers (one method per route — zero inline strings in components) ─

  setHome():            void { this.set(SEO_PAGES['HOME']); }
  setMenu():            void { this.set(SEO_PAGES['MENU']); }
  setReservations():    void { this.set(SEO_PAGES['RESERVATIONS']); }
  setAbout():           void { this.set(SEO_PAGES['ABOUT']); }
  setContact():         void { this.set(SEO_PAGES['CONTACT']); }
  setCheckout():        void { this.set(SEO_PAGES['CHECKOUT']); }
  setOrderConfirmed():  void { this.set(SEO_PAGES['ORDER_CONFIRMED']); }
  setPaymentSuccess():  void { this.set(SEO_PAGES['PAYMENT_SUCCESS']); }
  setPaymentCancelled(): void { this.set(SEO_PAGES['PAYMENT_CANCELLED']); }
  setPrivacy():         void { this.set(SEO_PAGES['PRIVACY']); }
  setTerms():           void { this.set(SEO_PAGES['TERMS']); }
  setLogin():           void { this.set(SEO_PAGES['LOGIN']); }
  setRegister():        void { this.set(SEO_PAGES['REGISTER']); }
}
