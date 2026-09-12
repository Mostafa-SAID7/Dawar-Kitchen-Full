/** Severity level of a toast notification */
export type ToastType = 'success' | 'error' | 'warning' | 'info';

/** A single toast notification entry */
export interface Toast {
  id: number;
  type: ToastType;
  title: string;
  message: string;
}
