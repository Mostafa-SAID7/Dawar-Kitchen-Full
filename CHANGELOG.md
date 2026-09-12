# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-09-12

### Added
- E2E test fixes for cart flow, checkout flow, and authentication
- Lighthouse CI workflow with automated performance audits
- Semantic release workflow for automatic version management
- Cart drawer data-cy attributes for better test selection

### Fixed
- YAML syntax errors in Lighthouse CI workflow
- Logout button selector text mismatch in E2E tests ('Logout' → 'Sign Out')
- Missing data-cy='cart-item' attribute in cart drawer component
- Node.js 20 deprecation warning (updated to Node.js 24)
- Lighthouse CI server startup for proper metric collection
- Cart service auto-open behavior causing E2E test failures

### Changed
- Updated GitHub Actions to use Node.js 24
- Improved test selectors for better reliability
- Enhanced CI/CD pipeline stability

## [1.0.0] - 2026-08-15

### Initial Release
- Full-stack restaurant management platform
- Angular 18 frontend with Tailwind CSS
- ASP.NET Core 8 backend with PostgreSQL
- Complete E2E test suite with Cypress
- CI/CD pipeline with GitHub Actions
- Production deployment to Vercel (frontend) and Azure App Service (backend)
