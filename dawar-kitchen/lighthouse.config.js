module.exports = {
  ci: {
    collect: {
      url: ['http://localhost:5000/'],
      numberOfRuns: 1,
      settings: {
        chromePath: '/usr/bin/chromium-browser',
        onlyCategories: ['performance', 'accessibility', 'best-practices', 'seo', 'pwa'],
      },
    },
    upload: {
      target: 'temporary-public-storage',
    },
    assert: {
      preset: 'lighthouse:recommended',
      assertions: {
        // Performance
        'categories:performance': ['warn', { minScore: 0.80 }],
        'largest-contentful-paint': ['warn', { maxNumericValue: 2500 }],
        'cumulative-layout-shift': ['warn', { maxNumericValue: 0.1 }],
        
        // Accessibility (WCAG 2.1 AA)
        'categories:accessibility': ['warn', { minScore: 0.80 }],
        'color-contrast': ['warn', { minScore: 0.90 }],
        'aria-hidden-body': 'warn',
        'aria-required-attr': 'warn',
        
        // Best Practices
        'categories:best-practices': ['warn', { minScore: 0.80 }],
        
        // SEO
        'categories:seo': ['warn', { minScore: 0.90 }],
        'meta-description': 'warn',
        'viewport': 'warn',
        
        // PWA (informational, not blocking)
        'installable-manifest': 'off',
        'splash-screen': 'off',
        'themed-omnibox': 'off',
        'maskable-icon': 'off',
        'service-worker': 'off',
        'csp-xss': 'off',
        'errors-in-console': 'warn',
      },
    },
  },
};
