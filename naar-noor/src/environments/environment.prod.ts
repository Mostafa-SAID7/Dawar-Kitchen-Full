export const environment = {
  production: true,
  apiUrl: (window as any)['env']?.apiUrl || 'https://dawar-kitchen-api.runasp.net',
  supabaseUrl: (window as any)['env']?.supabaseUrl || 'https://your-project.supabase.co',
  supabaseAnonKey: (window as any)['env']?.supabaseAnonKey || ''
};
