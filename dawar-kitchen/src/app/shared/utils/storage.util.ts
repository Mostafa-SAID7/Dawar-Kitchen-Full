export const StorageUtil = {
  /**
   * Safely gets a string value from localStorage.
   */
  get(key: string): string | null {
    if (typeof localStorage === 'undefined') return null;
    try {
      return localStorage.getItem(key);
    } catch {
      return null;
    }
  },

  /**
   * Safely sets a string value in localStorage.
   */
  set(key: string, value: string): void {
    if (typeof localStorage === 'undefined') return;
    try {
      localStorage.setItem(key, value);
    } catch (e) {
      console.warn('Error saving to localStorage', e);
    }
  },

  /**
   * Safely removes an item from localStorage.
   */
  remove(key: string): void {
    if (typeof localStorage === 'undefined') return;
    try {
      localStorage.removeItem(key);
    } catch {
      // Ignore
    }
  },

  /**
   * Safely gets and parses a JSON object from localStorage.
   */
  getObject<T>(key: string): T | null {
    const item = this.get(key);
    if (!item) return null;
    try {
      return JSON.parse(item) as T;
    } catch {
      return null;
    }
  },

  /**
   * Safely stringifies and sets an object in localStorage.
   */
  setObject(key: string, value: unknown): void {
    this.set(key, JSON.stringify(value));
  }
};
