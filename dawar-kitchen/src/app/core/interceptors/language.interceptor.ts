import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LanguageService } from '../../shared/services/language.service';
import { environment } from '../../../environments/environment';

/**
 * Attaches the Accept-Language header to API requests
 * so the backend returns localized data when possible.
 */
export const languageInterceptor: HttpInterceptorFn = (req, next) => {
  const languageService = inject(LanguageService);
  
  // Only attach to our own API to prevent leaking headers to third parties
  const isOwnApi = req.url.startsWith(environment.apiUrl) || req.url.startsWith('/api');
  
  if (isOwnApi) {
    const langReq = req.clone({
      setHeaders: { 'Accept-Language': languageService.getCurrentLanguageValue() }
    });
    return next(langReq);
  }

  return next(req);
};
