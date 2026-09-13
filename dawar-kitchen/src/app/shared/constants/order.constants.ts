/**
 * order.constants.ts
 * Order type values, cart limits, and reservation constraints.
 */

// ─── Order types ──────────────────────────────────────────────────────────────
export const ORDER_TYPE = {
  COLLECTION: 'collection',
  DELIVERY:   'delivery',
  DINE_IN:    'dine-in',
} as const;

export type OrderType = typeof ORDER_TYPE[keyof typeof ORDER_TYPE];

// ─── Cart limits ──────────────────────────────────────────────────────────────
export const CART_MAX_QTY_PER_ITEM = 20;
export const CART_MIN_QTY           = 1;

// ─── Reservation ──────────────────────────────────────────────────────────────
export const RESERVATION = {
  MIN_PARTY_SIZE:  1,
  MAX_PARTY_SIZE: 20,
  MIN_NAME_LENGTH: 2,
  MIN_PHONE_LENGTH: 7,
  NOTES_MAX_LENGTH: 300,
} as const;

// ─── Tables available for dine-in ─────────────────────────────────────────────
export const DINE_IN_TABLES: readonly string[] = [
  'Table 1 (Window)',
  'Table 2 (Cosy)',
  'Table 3 (Garden)',
  'Table 4 (Family)',
  'Table 5 (Bar)',
  'Table 6 (Private)',
];

// ─── Available time slots (reservation & pickup) ──────────────────────────────
export const TIME_SLOTS: readonly string[] = [
  '11:00', '11:30',
  '12:00', '12:30',
  '13:00', '13:30',
  '14:00', '14:30',
  '15:00', '15:30',
  '16:00', '16:30',
  '17:00', '17:30',
  '18:00', '18:30',
  '19:00', '19:30',
  '20:00', '20:30',
  '21:00', '21:30',
];

// ─── Guest count options ──────────────────────────────────────────────────────
export const GUEST_OPTIONS: readonly string[] = [
  '1 guest', '2 guests', '3 guests', '4 guests',
  '5 guests', '6 guests', '7 guests', '8 guests',
  '9 guests', '10+ guests',
];
