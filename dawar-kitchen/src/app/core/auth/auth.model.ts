/** Authenticated session stored after login */
export interface AuthSession {
  accessToken: string;
  userId: string;
  email: string;
}

/** Response from POST /api/auth/login */
export interface LoginResponse {
  access_token: string;
}

/** Response from GET /api/auth/me */
export interface MeResponse {
  userId: string;
  email: string;
}
