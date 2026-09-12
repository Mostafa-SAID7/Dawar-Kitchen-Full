## [1.1.2] - 2026-09-12

### Changes
# Changelog

All notable changes follow [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and [Semantic Versioning](https://semver.org/).

---

## [Unreleased]

### Planned
- Advanced menu search and filtering
- Email notification system

---

## [1.1.1] - 2026-09-12

### Fixed
- Resolved duplicate `sitemap.xml` (merged canonical domain version into `src/assets/`)
- Removed tracked test artifact `cart-test-output.txt`; expanded `.gitignore`
- Renamed CI/CD workflows to clear descriptive names (removed numeric prefixes)
- Removed stale working documents from `docs/` that referenced non-existent apps
- Added missing `LICENSE` file

---

## [1.1.0] - 2026-09-12

### Added
- E2E test fixes for cart flow, checkout flow, and authentication
- Lighthouse CI workflow with automated performance audits
- Semantic release workflow for automatic version management
- Cart drawer `data-cy` attributes for better test selection

### Fixed
- YAML syntax errors in Lighthouse CI workflow
- Logout button selector text mismatch in E2E tests (`Logout` → `Sign Out`)
- Missing `data-cy='cart-item'` attribute in cart drawer component
- Node.js 20 deprecation warning (updated to Node.js 24)
- Lighthouse CI server startup for proper metric collection
- Cart service auto-open behavior causing E2E test failures

### Changed
- Updated GitHub Actions to use Node.js 24
- Improved test selectors for better reliability
- Enhanced CI/CD pipeline stability

---

## [1.0.0] - 2026-08-15

### Added
- Full-stack restaurant management platform
- Angular 18 frontend with Tailwind CSS
- ASP.NET Core 8 backend with PostgreSQL
- Clean Architecture (CQRS + MediatR + Domain-Driven Design)
- JWT authentication & Supabase integration
- Stripe payment processing
- Complete E2E test suite with Cypress
- CI/CD pipeline with GitHub Actions
- Production deployment to Vercel (frontend) and Azure App Service (backend)
- Bilingual support (English + Arabic)
- PWA manifest for installability

---

## Version Policy

- **MAJOR** — breaking API changes
- **MINOR** — backwards-compatible new features
- **PATCH** — bug fixes

Support: Current `1.x` receives full updates. Older majors receive security fixes for 6 months only.

---

[Repository](https://github.com/Mostafa-SAID7/Naar-Noor-Full) · [Issues](https://github.com/Mostafa-SAID7/Naar-Noor-Full/issues) · [Releases](https://github.com/Mostafa-SAID7/Naar-Noor-Full/releases)
