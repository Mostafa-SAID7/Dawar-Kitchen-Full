export const DomUtil = {
  /**
   * Checks if the code is running in a browser environment.
   * Useful for SSR (Server-Side Rendering) guards.
   */
  isBrowser(): boolean {
    return typeof window !== 'undefined' && typeof document !== 'undefined';
  },

  /**
   * Safely gets the document object if in the browser.
   */
  getDocument(): Document | null {
    return this.isBrowser() ? document : null;
  },

  /**
   * Safely gets the window object if in the browser.
   */
  getWindow(): Window | null {
    return this.isBrowser() ? window : null;
  }
};
