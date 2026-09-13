# Agent Prompt: System Design Document — Dawar Kitchen 

## Role
You are a senior software architect. Your job is to produce a **complete, accurate System Design document** for the production restaurant platform in this repository:

**Repository:** https://github.com/Mostafa-SAID7/Dawar-Kitchen-Full  
**Product name:** Dawar Kitchen  
**Live demo:** https://dawar-kitchen.vercel.app  

You must **base every claim on the actual codebase and docs** under:
- `dawar-kitchen/` (Angular 18 frontend)
- `api-server/` (ASP.NET Core 8 backend)
- `docs/` (ARCHITECTURE, API, DATABASE, DEPLOYMENT, FRONTEND, BACKEND, STRUCTURE, TESTING, etc.)
- `.github/workflows/` (CI/CD)
- `docker-compose*.yml`, `vercel.json`, root `README.md`

Do **not** invent services, tables, or patterns that do not exist. If something is optional (e.g. Supabase/Stripe keys empty), say so explicitly.

---

## Mission
Write a professional **System Design** document (Markdown) that a tech lead, new engineer, or interviewer could use to understand:
1. What the system does
2. How it is structured end-to-end
3. How data, auth, payments, and i18n flow
4. How it is deployed, secured, tested, and scaled
5. Trade-offs, risks, and realistic next improvements

Output a single polished Markdown file suitable for `docs/SYSTEM_DESIGN.md` (or equivalent).

---

## Required document structure

Produce the document with these sections (use clear headings, diagrams in Mermaid or ASCII where useful):

### 1. Executive Summary
- Product purpose (bilingual restaurant platform: menu, reservations, orders, contact; Egyptian/Syrian cuisine social enterprise context)
- High-level stack table (Frontend / Backend / DB / Auth / Payments / Deploy / CI)
- Key non-functional goals (bilingual EN/AR + RTL, production-ready, Clean Architecture, security hardening)

### 2. Goals & Non-Goals
**Goals** (from real features):
- Browse menu, reserve tables, place orders, pay via Stripe
- EN/AR i18n with RTL support
- JWT + optional Supabase Auth
- Clean Architecture + CQRS on backend
- SPA with lazy routes, PWA service worker, SEO helpers

**Non-goals / out of scope** (state clearly if not in repo):
- Full admin back-office UI (unless present)
- Multi-tenant SaaS for many restaurants
- Native mobile as primary path (docs may mention mobile — only include if code exists)

### 3. System Context (C4 Level 1)
Diagram: Users/Browsers → Frontend (Vercel) → Backend API (Azure/Docker) → PostgreSQL (Supabase) + Stripe + Supabase Auth/Realtime/Storage.

Actors:
- Guest customers
- Authenticated customers
- Stripe (webhooks)
- Optional Supabase services

### 4. High-Level Architecture (C4 Level 2)
Describe the real split:
Browser (Angular 18 SPA)
HTTPS REST (+ optional Supabase Realtime)
▼
ASP.NET Core 8 API
API → Application (CQRS/MediatR) → Domain → Infrastructure (EF Core)
▼
PostgreSQL (Supabase) | Stripe | Supabase Auth


Frontend deploy: Vercel  
Backend deploy: Docker / Azure App Service (per docs)  
Local: docker-compose / Makefile

### 5. Backend Design (Clean Architecture + CQRS)

Document layers as in the repo:

| Layer | Project | Responsibility |
|-------|---------|----------------|
| API | `NaarNoor.API` | Controllers, middleware, DTOs, Swagger, health |
| Application | `NaarNoor.Application` | Commands/Queries, handlers, FluentValidation, caching |
| Domain | `NaarNoor.Domain` | Entities, value objects, enums |
| Infrastructure | `NaarNoor.Infrastructure` | EF Core, repositories, external services, migrations, seeding |

**Request flow:**
`Controller → Validation → MediatR Handler → EF Core / external service → HTTP response`

**Domain capabilities to document from code/docs:**
- Menu items (CRUD/query, filters, availability)
- Reservations (create/update/delete/list, auth where required)
- Orders + Stripe checkout session + webhook
- Chefs
- Contact inquiries
- Auth (login/register, JWT)
- Health checks

**Cross-cutting:**
- Exception handling middleware
- Auth / security middleware
- CORS
- Caching (Application `Caching`)
- Accept-Language / localization hooks if present
- Auto-migrate + seed on startup (if still true in code)

### 6. Frontend Design (Angular 18)

Document real structure (features/layout/shared after latest refactors):

- Standalone components
- App shell: header, footer, cart drawer, toast, splash, cookie consent, animated background
- Feature areas: home, menu, reservations, checkout/orders, auth, contact, static pages
- Routing: eager home + `loadComponent` lazy routes; guards (`authGuard`, `guestGuard`, `checkoutGuard`)
- `@defer` for below-the-fold home sections
- Services: API, auth, cart, language, theme, SEO, toast, realtime (as present)
- i18n: `@ngx-translate`, multi-file or merged assets under `src/assets/i18n/{en,ar}/`, `LanguageService` (dir/rtl, persistence)
- State: cart in localStorage; signals/OnPush where used
- Interceptors: auth, error, language (`Accept-Language`)
- PWA: `ngsw-config.json` (app shell, assets, i18n freshness, API cache strategies)
- Styling: Tailwind + global `styles.css` (theme + RTL)

### 7. Data Model
From `docs/DATABASE.md` and EF entities — document main aggregates:

- MenuItems
- Reservations
- Orders (and related payment/session data if modeled)
- Chefs
- Reviews (if present)
- ContactInquiries
- Users/auth-related storage (JWT vs Supabase — clarify actual source of truth)

Include:
- Primary keys, important indexes (e.g. category, availability)
- Status enums (reservation status, order status)
- ER diagram (Mermaid `erDiagram` preferred)
- Note any doc drift (e.g. SQL Server wording vs PostgreSQL/Supabase in practice)

### 8. API Design
Summarize from `docs/API.md` and controllers:

- Public vs JWT-protected endpoints
- Representative resources: `/api/menu`, `/api/reservations`, `/api/orders`, `/api/payments/*`, `/api/auth/*`, `/api/contact`, `/api/chefs`, `/health`
- Pagination/filter query params for menu
- Validation rules for reservations/orders
- Stripe webhook as public endpoint with signature verification (describe expected flow)
- Error handling style (problem details / consistent envelope if used)

### 9. AuthN / AuthZ
- Register/login → JWT
- Optional Supabase Auth integration (when keys configured)
- Frontend: token storage, `authInterceptor`, route guards
- Backend: JWT validation, authorized controllers
- Guest vs authenticated flows (browse vs checkout/reservations as implemented)

### 10. Payments Flow (Stripe)
Sequence diagram:
1. Client creates order / requests checkout session  
2. API creates Stripe Checkout Session  
3. Redirect to Stripe  
4. Webhook → API updates order/payment state  
5. Success/cancel frontend routes  

Call out idempotency, webhook security, and “Stripe optional if keys empty” behavior if true.

### 11. Internationalization & RTL
- EN/AR translation assets and loader strategy
- Language persistence + document `dir` / `lang`
- RTL layout strategy (logical CSS / dir attribute)
- API `Accept-Language` interceptor for localized backend responses where supported

### 12. Cross-Cutting Concerns

**Security**
- CSP and security headers (Vercel / nginx)
- Secrets via env (never commit); reference SECRETS patterns
- OWASP dependency check, secret scanning workflows
- CORS policy differences dev vs prod

**Performance**
- Angular production optimization, lazy routes, defer
- Image strategy / service worker caching
- API caching behaviours
- Known heavy assets or preload choices (honest about trade-offs)

**Reliability**
- Health endpoint
- Frontend mock/fallback data when API down (if still present)
- Docker compose for local parity

**Observability**
- What exists today (logs, health, CI badges) vs gaps (APM, structured logging) — be honest

### 13. Deployment Architecture
- Frontend: Vercel (`dawar-kitchen`, output `dist/.../browser`)
- Backend: Docker image, Azure App Service (per docs)
- Database: PostgreSQL via Supabase (env: `PGHOST`, `PGUSER`, etc.)
- docker-compose production/dev topology
- Environment configuration matrix (dev/stage/prod)

### 14. CI/CD
Document the real GitHub Actions workflows:
- secret-scan
- unit-tests
- integration-tests
- coverage-analysis
- build-artifacts
- deploy
- sast-sca
- lighthouse-ci
- release
(+ keep-alive if present)

Describe pipeline stages: PR checks → main deploy → tag release.

### 15. Testing Strategy
- Backend: xUnit (Application, Domain, Infrastructure, API integration)
- Frontend: Jest unit tests, Cypress E2E
- Coverage goals from docs
- Property-based tests if present

### 16. Scalability & Capacity (realistic)
Discuss current design limits and levers:
- Stateless API containers horizontal scale
- DB as primary bottleneck; indexes and read caching
- Stripe/Supabase external limits
- SPA static hosting scale (Vercel)
- What is **not** yet in place (message bus, multi-region, CQRS read models at scale, etc.)

### 17. Key Sequences (Mermaid)
Include at least:
1. Page load + i18n + menu fetch  
2. Login → JWT → authenticated reservation  
3. Add to cart → checkout → Stripe → webhook → confirmation  
4. Language switch EN ↔ AR (UI + dir)

### 18. Risks, Tech Debt & Recommendations
Ground in real repo observations, e.g.:
- Doc drift (SQL Server vs PostgreSQL wording)
- Large static images / performance debt
- i18n multi-file request cost vs merge
- Optional external services complexity
- Aggressive preloading or animation costs
Prioritize recommendations (P0/P1/P2).

### 19. Appendix
- Link map to existing docs (`ARCHITECTURE.md`, `API.md`, `DATABASE.md`, …)
- Glossary (CQRS, MediatR, Clean Architecture, RTL, JWT, SW)
- Version/stack pin summary (Angular 18, .NET 8, EF Core 8, Tailwind 3.4, etc.)

---

## Analysis rules (mandatory)

1. **Read first** (in order):
   - `README.md`
   - `docs/ARCHITECTURE.md`
   - `docs/STRUCTURE.md`
   - `docs/API.md`
   - `docs/DATABASE.md`
   - `docs/BACKEND.md` / `docs/FRONTEND.md`
   - `docs/DEPLOYMENT.md`
   - `api-server/src/**` project layout (controllers, Application feature folders, Domain entities)
   - `dawar-kitchen/src/app/core/config/app.routes.ts`, `app.config.ts`
   - `dawar-kitchen/vercel.json`, `src/assets/ngsw-config.json`
   - `.github/workflows/*`

2. Prefer **code over outdated comments** when docs conflict; note the conflict in “Doc drift”.

3. Use **Mermaid** for context, container, ER, and sequence diagrams.

4. Keep tone **precise and engineering-grade** — no marketing fluff.

5. Every major subsystem should answer: *purpose, components, data flow, failure modes*.

6. Length target: **thorough but usable** (roughly a solid design-review doc, not a full rewrite of all existing docs). Cross-link instead of pasting entire API tables when a summary + pointer is enough.

---

## Deliverable

1. Full Markdown System Design document with the sections above.
2. Short “Sources” footer listing primary files you relied on.
3. Optional: a one-page “Architecture at a glance” blurb at the top for executives.

Start by reading the repository structure and `docs/ARCHITECTURE.md`, then expand with API, data, frontend, deploy, and CI facts from the live tree.