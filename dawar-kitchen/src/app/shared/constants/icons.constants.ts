/**
 * icons.constants.ts
 * Iconify icon names used across the app.
 * Keeping them here prevents typos and makes icon refactoring trivial.
 */
export const ICONS = {
  // Navigation
  MENU:       'solar:hamburger-menu-linear',
  CLOSE:      'solar:close-circle-linear',
  ARROW_LEFT: 'solar:arrow-left-linear',
  ARROW_RIGHT:'solar:arrow-right-linear',
  EXTERNAL:   'solar:arrow-right-up-linear',

  // Cart / Checkout
  CART:       'solar:cart-large-2-bold',
  CART_EMPTY: 'solar:cart-large-2-linear',
  TRASH:      'solar:trash-bin-trash-linear',
  PLUS:       'solar:add-square-linear',
  MINUS:      'solar:minus-square-linear',
  TAG:        'solar:tag-linear',
  LOCK:       'solar:lock-bold',
  CHECK:      'solar:check-circle-bold',
  CHECK_LINE: 'solar:check-circle-linear',

  // Order types
  PICKUP:   'solar:bag-5-linear',
  DELIVERY: 'solar:delivery-linear',
  DINE_IN:  'solar:tea-cup-linear',
  TABLE:    'solar:sofa-linear',

  // Status / feedback
  SPINNER:  'solar:spinner-line-duotone',
  DANGER:   'solar:danger-circle-linear',
  INFO:     'solar:info-circle-linear',
  STAR:     'solar:star-linear',

  // Misc
  FLAME:    'solar:flame-linear',
  LEAF:     'solar:leaf-linear',
  CUP_HOT: 'solar:cup-hot-linear',
  FILTER:   'solar:filter-linear',
  SORT:     'solar:sort-from-top-linear',
  SEARCH:   'solar:magnifer-linear',
  PHONE:    'solar:phone-linear',
  EMAIL:    'solar:letter-linear',
  MAP_PIN:  'solar:map-point-linear',
  DOCUMENT: 'solar:document-linear',
} as const;

export type IconKey = keyof typeof ICONS;
