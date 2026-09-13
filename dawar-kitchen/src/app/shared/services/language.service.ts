import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { SUPPORTED_LANGUAGES, DEFAULT_LANGUAGE, RTL_LANGUAGE, STORAGE_KEYS } from '../constants';
import { StorageUtil, DomUtil } from '../utils';

/**
 * Language service for managing EN/AR bilingual support
 * - Manages language switching
 * - Persists language preference to localStorage
 * - Handles RTL support for Arabic
 */
@Injectable({ providedIn: 'root' })
export class LanguageService {
  private currentLanguage$ = new BehaviorSubject<string>(
    this.getInitialLanguage()
  );

  constructor(private translateService: TranslateService) {
    this.initializeTranslation();
  }

  /**
   * Get initial language from localStorage or use default
   */
  private getInitialLanguage(): string {
    const stored = StorageUtil.get(STORAGE_KEYS.LANGUAGE);
    if (stored && (SUPPORTED_LANGUAGES as readonly string[]).includes(stored)) {
      return stored;
    }
    return DEFAULT_LANGUAGE;
  }

  /**
   * Initialize translation service
   */
  private initializeTranslation(): void {
    this.translateService.setDefaultLang(DEFAULT_LANGUAGE);
    this.translateService.addLangs([...SUPPORTED_LANGUAGES]);
    this.setLanguage(this.currentLanguage$.value);
  }

  /**
   * Set active language
   * - Updates TranslateService
   * - Persists to localStorage
   * - Sets document lang and RTL attributes
   */
  setLanguage(lang: string): void {
    if (!(SUPPORTED_LANGUAGES as readonly string[]).includes(lang)) {
      return;
    }

    this.applyDocumentLanguage(lang);
    StorageUtil.set(STORAGE_KEYS.LANGUAGE, lang);

    // Force a fresh fetch every time. ngx-translate caches translations in memory
    // and treats an empty {} as "loaded" (never retrying), so a load that failed
    // during early bootstrap would leave the language permanently broken.
    // reloadLang() clears the cache and re-issues the HTTP requests.
    this.translateService.reloadLang(lang);

    this.translateService.use(lang).subscribe({
      next: () => this.currentLanguage$.next(lang),
      error: () => this.handleLanguageFallback(lang)
    });
  }

  /**
   * Fall back to the default language when the requested language fails to load.
   */
  private handleLanguageFallback(lang: string): void {
    if (lang === DEFAULT_LANGUAGE) {
      this.currentLanguage$.next(DEFAULT_LANGUAGE);
      return;
    }

    this.applyDocumentLanguage(DEFAULT_LANGUAGE);
    StorageUtil.set(STORAGE_KEYS.LANGUAGE, DEFAULT_LANGUAGE);
    this.translateService.reloadLang(DEFAULT_LANGUAGE);
    this.translateService.use(DEFAULT_LANGUAGE).subscribe({
      next: () => this.currentLanguage$.next(DEFAULT_LANGUAGE),
      error: () => this.currentLanguage$.next(DEFAULT_LANGUAGE)
    });
  }

  private applyDocumentLanguage(lang: string): void {
    const doc = DomUtil.getDocument();
    if (!doc) return;

    const isRtl = lang === RTL_LANGUAGE;
    doc.documentElement.lang = lang;
    doc.documentElement.dir = isRtl ? 'rtl' : 'ltr';
    doc.documentElement.classList.toggle('rtl', isRtl);
    doc.documentElement.classList.toggle('ltr', !isRtl);
    doc.body.classList.toggle('rtl', isRtl);
    doc.body.classList.toggle('ltr', !isRtl);
  }

  /**
   * Get current language as Observable
   */
  getCurrentLanguage(): Observable<string> {
    return this.currentLanguage$.asObservable();
  }

  /**
   * Get current language value synchronously
   */
  getCurrentLanguageValue(): string {
    return this.currentLanguage$.value;
  }

  /**
   * Get supported languages
   */
  getSupportedLanguages(): string[] {
    return [...SUPPORTED_LANGUAGES];
  }

  /**
   * Toggle between EN and AR
   */
  toggleLanguage(): void {
    const newLang =
      this.currentLanguage$.value === 'en' ? 'ar' : 'en';
    this.setLanguage(newLang);
  }

  /**
   * Get the "other" language (opposite of current)
   */
  getOtherLanguage(): string {
    return this.currentLanguage$.value === 'en' ? 'ar' : 'en';
  }
}
