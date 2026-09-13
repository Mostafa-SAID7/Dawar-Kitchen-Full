/**
 * validation.constants.ts
 * All form validation rules — min/max lengths, regex patterns.
 * Import once, use across every reactive form in the app.
 */

// ─── Name / text fields ───────────────────────────────────────────────────────
export const VALIDATION = {
  NAME: {
    MIN_LENGTH: 2,
    MAX_LENGTH: 100,
  },
  EMAIL: {
    PATTERN: /^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$/,
  },
  PHONE: {
    MIN_LENGTH: 7,
    MAX_LENGTH: 20,
  },
  PASSWORD: {
    MIN_LENGTH: 8,
    MAX_LENGTH: 128,
  },
  ADDRESS: {
    MIN_LENGTH: 5,
    MAX_LENGTH: 300,
  },
  NOTES: {
    MAX_LENGTH: 300,
  },
  MESSAGE: {
    MIN_LENGTH: 10,
    MAX_LENGTH: 1000,
  },
} as const;

// ─── Regex ────────────────────────────────────────────────────────────────────
export const REGEX = {
  EMAIL:   /^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$/,
  /** Digits, spaces, +, -, () */
  PHONE:   /^[0-9\s\+\-\(\)]{7,20}$/,
  NUMBERS: /^\d+$/,
} as const;
