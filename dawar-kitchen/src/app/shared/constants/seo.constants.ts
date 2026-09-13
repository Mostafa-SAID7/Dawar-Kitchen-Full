/**
 * seo.constants.ts
 * SEO metadata constants for every page — single source of truth.
 * Used by SeoService page-specific helpers.
 */
import { APP_NAME, APP_DESCRIPTION, APP_KEYWORDS, SITE_URL, SITE_OG_IMAGE } from './app.constants';
import { SeoConfig } from '../models/seo.model';

export const SEO_BASE_NAME = APP_NAME;

export const SEO_DEFAULTS: SeoConfig = {
  title:        APP_NAME,
  description:  APP_DESCRIPTION,
  keywords:     APP_KEYWORDS,
  canonicalUrl: `${SITE_URL}/`,
  ogUrl:        `${SITE_URL}/`,
  ogImage:      SITE_OG_IMAGE,
  ogType:       'website',
  noIndex:      false,
};

export const SEO_PAGES: Record<string, SeoConfig> = {
  HOME: {
    title:        `${APP_NAME} — Authentic Egyptian & Syrian Cuisine`,
    description:  'Dawar Kitchen: Social enterprise delivering authentic Egyptian & Syrian home-style meals across Cairo. Celebrating food heritage and fair work.',
    keywords:     'Dawar Kitchen, Egyptian Syrian cuisine, Cairo delivery, social enterprise, authentic meals, Ezbet Khairallah, Egyptian food, Damascus cuisine',
    canonicalUrl: `${SITE_URL}/`,
    ogUrl:        `${SITE_URL}/`,
    ogType:       'restaurant',
  },
  MENU: {
    title:        'Menu',
    description:  'Browse the full Dawar Kitchen menu — authentic Egyptian & Syrian dishes, Koshari, Kabsa, Kibbeh, Om Ali, and more. Filter by category, dietary preference, or price.',
    keywords:     'Dawar Kitchen menu, Egyptian Syrian cuisine, Cairo delivery menu, Koshari, Kabsa, Kofta, vegetarian, vegan options',
    canonicalUrl: `${SITE_URL}/menu`,
    ogUrl:        `${SITE_URL}/menu`,
  },
  RESERVATIONS: {
    title:        'Reservations',
    description:  'Book a table at Dawar Kitchen. Reserve your spot for an authentic Egyptian & Syrian dining experience in Cairo.',
    keywords:     'Dawar Kitchen reservations, Cairo restaurant booking, Egyptian dining, table reservation Cairo',
    canonicalUrl: `${SITE_URL}/reservations`,
    ogUrl:        `${SITE_URL}/reservations`,
  },
  ABOUT: {
    title:        'About Us',
    description:  'Learn about Dawar Kitchen — a Cairo-based social enterprise empowering Syrian and Egyptian women through authentic food production and dignified employment.',
    keywords:     'Dawar Kitchen about, Cairo social enterprise, Syrian Egyptian women empowerment, authentic cuisine mission',
    canonicalUrl: `${SITE_URL}/about`,
    ogUrl:        `${SITE_URL}/about`,
  },
  CONTACT: {
    title:        'Contact Us',
    description:  'Get in touch with Dawar Kitchen. We\'d love to hear from you — for orders, events, or partnerships.',
    keywords:     'Dawar Kitchen contact, Cairo restaurant contact, Egyptian food delivery enquiry',
    canonicalUrl: `${SITE_URL}/contact`,
    ogUrl:        `${SITE_URL}/contact`,
  },
  CHECKOUT: {
    title:        'Checkout',
    description:  'Complete your Dawar Kitchen order. Choose delivery across Cairo and confirm your authentic Egyptian & Syrian meal order.',
    noIndex:      true,
  },
  ORDER_CONFIRMED: {
    title:        'Order Confirmed',
    description:  'Your Dawar Kitchen order has been received. We will confirm shortly by phone. Thank you for supporting our social enterprise!',
    noIndex:      true,
  },
  PAYMENT_SUCCESS: {
    title:        'Payment Successful',
    description:  'Thank you! Your Dawar Kitchen payment was processed successfully.',
    noIndex:      true,
  },
  PAYMENT_CANCELLED: {
    title:        'Payment Cancelled',
    description:  'Your Dawar Kitchen payment was cancelled. Your cart is still saved — try again anytime.',
    noIndex:      true,
  },
  PRIVACY: {
    title:        'Privacy Policy',
    description:  'Read the Dawar Kitchen privacy policy to understand how we handle your personal data.',
    canonicalUrl: `${SITE_URL}/privacy`,
    ogUrl:        `${SITE_URL}/privacy`,
  },
  TERMS: {
    title:        'Terms & Conditions',
    description:  'Dawar Kitchen terms and conditions — governing the use of our website and ordering service.',
    canonicalUrl: `${SITE_URL}/terms`,
    ogUrl:        `${SITE_URL}/terms`,
  },
  LOGIN: {
    title:        'Sign In',
    description:  'Sign in to your Dawar Kitchen account to manage your orders and reservations.',
    canonicalUrl: `${SITE_URL}/login`,
    ogUrl:        `${SITE_URL}/login`,
    noIndex:      true,
  },
  REGISTER: {
    title:        'Create Account',
    description:  'Create a Dawar Kitchen account to place orders and book reservations faster.',
    canonicalUrl: `${SITE_URL}/register`,
    ogUrl:        `${SITE_URL}/register`,
    noIndex:      true,
  },
};
