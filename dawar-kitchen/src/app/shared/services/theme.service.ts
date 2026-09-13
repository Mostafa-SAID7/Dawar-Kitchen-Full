import { Injectable, signal, computed, effect } from '@angular/core';
import { STORAGE_KEYS, THEME } from '../constants';
import { StorageUtil, DomUtil } from '../utils';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly _isDark = signal<boolean>(this.loadTheme());

  readonly isDark = computed(() => this._isDark());

  constructor() {
    // Apply theme immediately and on every change
    effect(() => {
      const dark = this._isDark();
      const doc = DomUtil.getDocument();
      if (doc) {
        const html = doc.documentElement;
        if (dark) {
          html.classList.remove('theme-light');
          html.classList.add('theme-dark');
        } else {
          html.classList.remove('theme-dark');
          html.classList.add('theme-light');
        }
      }
      StorageUtil.set(STORAGE_KEYS.THEME, dark ? THEME.DARK : THEME.LIGHT);
    });
  }

  toggle(): void {
    this._isDark.update(v => !v);
  }

  private loadTheme(): boolean {
    const stored = StorageUtil.get(STORAGE_KEYS.THEME);
    if (stored) return stored === THEME.DARK;
    return true; // default: dark
  }
}
