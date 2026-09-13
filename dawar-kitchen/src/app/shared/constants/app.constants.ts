/**
 * app.constants.ts
 * Core application-level constants — brand, URLs, storage keys, API paths.
 */

// ─── Brand ────────────────────────────────────────────────────────────────────
export const APP_NAME        = 'Dawar Kitchen';
export const APP_TAGLINE     = 'Authentic Egyptian & Syrian Cuisine';
export const APP_DESCRIPTION = 'Dawar Kitchen is a social enterprise delivering authentic Egyptian & Syrian home-style meals across Cairo. Celebrating food heritage and fair work.';
export const APP_KEYWORDS    = 'Dawar Kitchen, Egyptian Syrian cuisine, Cairo delivery, social enterprise, home-style meals, authentic food, Cairo restaurant, Ezbet Khairallah';
export const APP_LOCALE      = 'en_GB';
export const APP_SINCE_YEAR  = 2018;

// ─── Public URL ───────────────────────────────────────────────────────────────
export const SITE_URL        = 'https://www.dawarkitchen.com';
export const SITE_OG_IMAGE   = `${SITE_URL}/assets/hero/hero.webp`;

// ─── localStorage keys ────────────────────────────────────────────────────────
export const STORAGE_KEYS = {
  SESSION:   'nn_session',
  CART:      'nn_cart',
  THEME:     'nn_theme',
  LANGUAGE:  'language',
} as const;

// ─── Theme ────────────────────────────────────────────────────────────────────
export const THEME = {
  DARK:  'dark',
  LIGHT: 'light',
} as const;

// ─── Supported languages ──────────────────────────────────────────────────────
export const SUPPORTED_LANGUAGES  = ['en', 'ar'] as const;
export const DEFAULT_LANGUAGE     = 'en';
export const RTL_LANGUAGE         = 'ar';

// ─── API retry ────────────────────────────────────────────────────────────────
export const API_RETRY_COUNT      = 2;
export const API_RETRY_DELAY_MS   = 2000;

// ─── Toast durations (ms) ─────────────────────────────────────────────────────
export const TOAST_DURATION = {
  SUCCESS: 7000,
  ERROR:   5000,
  WARNING: 5000,
  INFO:    4000,
} as const;
