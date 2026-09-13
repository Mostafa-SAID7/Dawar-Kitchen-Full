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

    this.translateService.use(lang);
    StorageUtil.set(STORAGE_KEYS.LANGUAGE, lang);
    this.currentLanguage$.next(lang);
    
    const doc = DomUtil.getDocument();
    if (doc) {
      doc.documentElement.lang = lang;
      if (lang === RTL_LANGUAGE) {
        doc.documentElement.dir = 'rtl';
        doc.body.classList.add('rtl');
      } else {
        doc.documentElement.dir = 'ltr';
        doc.body.classList.remove('rtl');
      }
    }
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
