# Angular Architecture & SOLID Deep Refactoring — Dawar Kitchen

## ROLE

You are acting as a Senior Angular Architect and Production-Level Frontend Engineer.

Your responsibility is to deeply audit, redesign, refactor, reorganize, and clean the existing Angular frontend architecture of this repository.

This is NOT a documentation-only task.

You must DIRECTLY EXECUTE the required changes in the repository.

The final result must be:

* Clean
* Organized
* Feature-oriented
* SOLID-compliant
* Maintainable
* Scalable
* Testable
* Easy for another senior developer to understand
* Free from duplicate files and duplicate responsibilities
* Compatible with the existing backend/API
* Compatible with the existing Angular version
* Free from unnecessary over-engineering

---

# PROJECT CONTEXT

The frontend is an Angular application for a restaurant platform.

Current Angular stack includes:

* Angular 18
* Standalone Components
* TypeScript
* RxJS
* Angular Router
* HttpClient
* Angular Forms
* Angular Service Worker / PWA
* Tailwind CSS
* ngx-translate
* Jest
* Cypress
* Authentication
* Reservations
* Menu
* Checkout
* Orders
* Reviews
* Arabic / English localization
* RTL support

Existing application structure contains concepts such as:

```text
src/app/
├── components/
├── directives/
├── guards/
├── interceptors/
├── models/
├── pages/
├── sections/
├── services/
├── app.component.ts
├── app.config.ts
└── app.routes.ts
```

IMPORTANT:

Do NOT blindly preserve this structure.

First understand what currently exists and how every file is being used.

Then determine the best architecture.

---

# PRIMARY OBJECTIVE

Transform the current Angular architecture into a clean, scalable, feature-oriented architecture while preserving all existing behavior.

The architecture must clearly separate:

1. Application-level concerns
2. Feature-level concerns
3. Shared UI
4. Domain/data models
5. Infrastructure/API concerns
6. Cross-cutting concerns

The final structure must make it obvious:

> "Where should I put a new feature?"

and:

> "Who is responsible for this code?"

---

# ABSOLUTE RULE

DO NOT START MOVING FILES IMMEDIATELY.

First perform a COMPLETE AUDIT.

You must understand the current codebase before changing it.

---

# PHASE 1 — COMPLETE EXISTING ARCHITECTURE AUDIT

Inspect the entire frontend.

Analyze:

```text
src/
src/app/
routes
components
pages
sections
services
models
guards
interceptors
directives
pipes
utils
assets
styles
configuration
tests
```

Also inspect:

* package.json
* angular.json
* tsconfig files
* Jest configuration
* Cypress configuration
* environment configuration
* service worker configuration
* translation configuration
* Tailwind configuration
* routing configuration
* application bootstrap
* HTTP configuration
* authentication flow

Do not assume folders are correctly named.

---

# PHASE 2 — BUILD A FILE RESPONSIBILITY MAP

Before refactoring, classify EVERY relevant application file.

For each file determine:

```text
Current Path
File Name
Responsibility
Who Uses It
Dependencies
Feature
Layer
Reusable?
Business Logic?
UI Logic?
API Logic?
Cross-Cutting?
Candidate New Location
Action
```

Possible actions:

```text
KEEP
MOVE
MERGE
SPLIT
RENAME
DELETE
REFACTOR
```

DO NOT DELETE anything until you prove it is unused or obsolete.

---

# PHASE 3 — DETECT ARCHITECTURAL PROBLEMS

Search deeply for:

## Duplication

Find:

* duplicated components
* duplicated services
* duplicated models
* duplicated interfaces
* duplicated API calls
* duplicated validation
* duplicated constants
* duplicated utility functions
* duplicated authentication logic
* duplicated UI logic
* duplicated route logic

If two files have overlapping responsibility:

Determine whether they should be:

```text
merged
split by responsibility
moved into shared
moved into feature
deleted
```

Never keep two implementations of the same responsibility without a clear architectural reason.

---

# PHASE 4 — DETECT WRONG RESPONSIBILITIES

Find violations such as:

```text
Component → direct API call

Component → business logic

Component → localStorage implementation

Component → authentication implementation

Service → UI manipulation

Guard → business logic

Interceptor → feature-specific logic

Shared component → feature-specific behavior

Model → business logic

Utility → hidden application state

Page → huge implementation

God Service

God Component
```

Refactor these responsibilities to the correct layer.

---

# PHASE 5 — TARGET ARCHITECTURE

Use a feature-oriented architecture.

Prefer a structure similar to:

```text
src/
└── app/
    │
    ├── core/
    │   ├── auth/
    │   ├── http/
    │   ├── config/
    │   ├── guards/
    │   ├── interceptors/
    │   └── services/
    │
    ├── shared/
    │   ├── components/
    │   ├── directives/
    │   ├── pipes/
    │   ├── validators/
    │   ├── models/
    │   ├── utils/
    │   └── constants/
    │
    ├── features/
    │   ├── auth/
    │   │   ├── pages/
    │   │   ├── components/
    │   │   ├── services/
    │   │   ├── models/
    │   │   └── routes.ts
    │   │
    │   ├── menu/
    │   │   ├── pages/
    │   │   ├── components/
    │   │   ├── services/
    │   │   ├── models/
    │   │   └── routes.ts
    │   │
    │   ├── reservations/
    │   │   ├── pages/
    │   │   ├── components/
    │   │   ├── services/
    │   │   ├── models/
    │   │   └── routes.ts
    │   │
    │   ├── checkout/
    │   ├── orders/
    │   ├── reviews/
    │   ├── home/
    │   ├── contact/
    │   └── ...
    │
    ├── layout/
    │   ├── components/
    │   └── ...
    │
    ├── app.component.ts
    ├── app.config.ts
    └── app.routes.ts
```

IMPORTANT:

This is a TARGET DIRECTION, NOT a blind template.

Adapt it to the actual repository.

Do not create folders that do not provide real value.

Do not move files simply to make the tree look beautiful.

Architecture must follow responsibility.

---

# CORE VS SHARED VS FEATURE RULES

Apply these rules strictly.

## CORE

`core/` contains application-wide infrastructure.

Examples:

```text
authentication infrastructure
HTTP infrastructure
global interceptors
global guards
application configuration
global services
API infrastructure
```

A feature-specific service MUST NOT be placed in `core`.

---

## SHARED

`shared/` contains reusable code that does not belong to one specific feature.

Examples:

```text
Button
Modal
Loading
Pagination
Reusable form controls
pipes
directives
validators
generic models
utility functions
constants
```

A shared component MUST NOT know about:

```text
reservations
checkout
orders
authentication
menu
```

unless it is intentionally generic.

---

## FEATURES

Feature-specific code belongs inside its feature.

For example:

```text
features/reservations/
```

should own reservation-specific:

```text
pages
components
services
models
validators
state
API interactions
```

Do not put reservation-specific code into:

```text
shared/
core/
```

just because it is technically reusable.

---

# PAGES VS COMPONENTS

Apply a strict distinction.

## Page

A page represents a route/screen.

Responsibilities:

* route-level orchestration
* feature composition
* loading feature data
* coordinating child components
* route parameters
* page-level state

A page should NOT become a huge component.

---

## Component

A component represents reusable or meaningful UI.

Responsibilities:

* presentation
* user interaction
* emitting events
* displaying data
* small UI-level state

Avoid putting API orchestration and business rules inside reusable components.

---

# SECTIONS

Audit the existing `sections/` directory carefully.

For every section determine whether it is:

```text
feature-specific
shared UI
page composition
layout
```

Then move it accordingly.

Do NOT keep `sections/` just because it already exists.

If `sections/` creates architectural ambiguity, remove the concept and place files into:

```text
features/<feature>/components
```

or:

```text
shared/components
```

or:

```text
layout/components
```

based on responsibility.

---

# MODELS

Audit every model/interface/type.

Determine whether each belongs to:

```text
feature-specific model
shared UI model
API contract
domain model
request DTO
response DTO
```

Avoid a giant global:

```text
models/
```

folder containing unrelated feature models.

Feature-specific models should live with the feature.

---

# SERVICES

Audit every service.

For every service answer:

```text
What responsibility does it have?
Is it feature-specific?
Is it infrastructure?
Does it contain business logic?
Does it call HttpClient?
Does it manage state?
Is it duplicated?
Is it actually needed?
```

Use clear separation.

Example:

```text
Component
   ↓
Feature Service / Facade
   ↓
API Service
   ↓
HttpClient
   ↓
Interceptor
   ↓
Backend API
```

Do not blindly introduce repository patterns into Angular.

Do not create abstractions without a real benefit.

---

# SOLID IMPLEMENTATION

Apply SOLID practically.

Do NOT merely mention SOLID in documentation.

---

## S — Single Responsibility Principle

Each class/function/component should have one clear reason to change.

Find:

```text
God components
God services
mixed API + UI logic
mixed auth + storage + navigation
mixed validation + API + presentation
```

Split only when there is a meaningful responsibility boundary.

---

## O — Open/Closed Principle

Avoid modifying many unrelated components when introducing a new behavior.

Prefer:

```text
configuration
composition
reusable abstractions
strategy-like behavior
```

where appropriate.

Do NOT overuse abstract classes or interfaces.

---

## L — Liskov Substitution Principle

If inheritance exists:

Audit every base class.

Ensure derived implementations can safely replace the base abstraction.

Remove unnecessary inheritance.

Prefer composition where inheritance adds no real value.

---

## I — Interface Segregation Principle

Avoid giant interfaces.

Instead of:

```text
UserService
```

containing unrelated contracts, separate responsibilities when required.

Keep TypeScript interfaces focused.

---

## D — Dependency Inversion Principle

High-level feature logic should not be tightly coupled to low-level implementation details.

Use Angular Dependency Injection appropriately.

Avoid:

```text
new SomeService()
```

inside application classes.

Prefer DI.

Do not introduce interfaces everywhere just to claim DIP.

Only introduce abstractions where they improve:

```text
testability
replaceability
separation
architecture
```

---

# ANGULAR STANDALONE ARCHITECTURE

The project uses modern Angular standalone architecture.

Preserve and improve it.

Do NOT reintroduce:

```text
AppModule
```

unless absolutely required by an existing dependency.

Prefer:

```text
bootstrapApplication()
app.config.ts
standalone components
provideRouter()
provideHttpClient()
providers
```

Use modern Angular dependency injection patterns consistently.

Where appropriate, evaluate:

```text
inject()
constructor injection
```

Choose the approach based on readability and consistency.

Do not rewrite everything only for syntax preference.

---

# ROUTING

Audit routing deeply.

Ensure:

```text
app.routes.ts
```

contains application-level routing only.

Feature routes should be colocated with their feature where this improves maintainability.

Consider:

```text
loadComponent
loadChildren
feature routes
lazy loading
route-level providers
```

Use lazy loading for meaningful feature boundaries.

Do not lazy-load tiny components unnecessarily.

---

# AUTHENTICATION

Audit:

```text
auth service
auth guard
interceptor
token handling
storage
login
register
logout
route protection
```

Ensure responsibilities are separated.

For example:

```text
AuthService
    ↓
authentication state

AuthGuard
    ↓
route access

AuthInterceptor
    ↓
HTTP authentication

Backend
    ↓
real authorization/security
```

Never rely on Angular guards as the actual security boundary.

---

# HTTP / INTERCEPTORS

Audit all interceptors.

Separate concerns such as:

```text
authentication
error handling
loading
logging
```

Do not create one giant interceptor containing every concern unless there is a strong reason.

Ensure interceptors do not contain feature-specific business logic.

---

# STATE MANAGEMENT

Audit how state is currently handled.

Determine whether the project actually needs:

```text
Signals
RxJS
BehaviorSubject
service state
component state
NgRx
```

Do NOT introduce NgRx just for architectural appearance.

Use the simplest state mechanism that satisfies the feature.

Where Angular Signals provide meaningful local/application state improvements, consider them.

Do not convert every Observable into a Signal without reason.

---

# RXJS

Audit:

```text
subscriptions
unsubscribe patterns
async pipe
takeUntil
switchMap
combineLatest
forkJoin
shareReplay
BehaviorSubject
Subjects
```

Find:

* memory leaks
* nested subscriptions
* unnecessary subscriptions
* duplicated API calls
* incorrect operators
* race conditions
* stale data
* unnecessary manual subscriptions

Prefer declarative patterns where appropriate.

---

# COMPONENT COMMUNICATION

Audit:

```text
@Input
@Output
shared services
signals
route state
```

Avoid hidden coupling.

Use clear parent-child communication.

Do not use global services as event buses unless there is a strong reason.

---

# FORMS

Audit all forms.

Determine:

```text
Reactive Forms
Template-driven Forms
validation
custom validators
async validators
form state
error presentation
```

Prefer consistent Reactive Forms for complex business forms.

Move reusable validators into appropriate locations.

Do not duplicate validation rules.

---

# INTERNATIONALIZATION

The application supports Arabic and English.

Audit:

```text
translations
translation keys
RTL
direction switching
language state
hardcoded strings
formatting
```

Ensure feature-specific translations are organized logically.

Remove duplicated translation keys.

Do not break:

```text
Arabic
English
RTL
LTR
```

---

# UI / TAILWIND

Audit reusable styling.

Find:

* duplicated Tailwind classes
* repeated UI patterns
* page-specific styles incorrectly shared
* global styles incorrectly used
* component styles leaking responsibilities

Do not create unnecessary abstraction for every repeated CSS class.

---

# TEST ARCHITECTURE

Existing testing includes:

```text
Jest
Cypress
```

Preserve tests.

When moving files:

* update test paths
* update imports
* update mocks
* update providers
* preserve coverage

Do not delete tests simply because files moved.

If an architectural change exposes missing important tests, add focused tests.

---

# TYPESCRIPT QUALITY

Audit:

```text
any
unknown
type assertions
non-null assertions
optional chaining
interfaces
type aliases
enums
constants
```

Reduce unnecessary:

```text
any
```

Do not use `any` to hide architecture problems.

Use strong typing for:

```text
API responses
requests
forms
component inputs
component outputs
service methods
state
```

---

# IMPORT RULES

The final architecture must avoid chaotic imports.

Prefer:

```text
feature → shared
feature → core
shared → no feature
core → no feature
```

Avoid:

```text
shared → feature
core → feature
feature A → deep internal implementation of feature B
```

If cross-feature communication is required, use a clean application-level contract or appropriate shared abstraction.

Avoid circular dependencies.

---

# CIRCULAR DEPENDENCY AUDIT

Search for circular dependencies.

Examples:

```text
Feature A → Feature B → Feature A

Service A → Service B → Service A

Shared → Feature

Core → Feature
```

Resolve every meaningful circular dependency.

Do not ignore circular imports because the application happens to compile.

---

# NAMING CONVENTIONS

Normalize naming.

Use consistent Angular conventions for:

```text
components
services
guards
interceptors
directives
pipes
models
routes
files
folders
```

Do not rename files unnecessarily.

Rename when the existing name creates ambiguity or violates the architecture.

---

# DELETE OLD STRUCTURE

After migration:

1. Verify every old file.
2. Search all references.
3. Confirm replacement exists.
4. Update imports.
5. Update tests.
6. Update routes.
7. Run build.
8. Run tests.
9. Only then delete obsolete files/folders.

Final repository must NOT contain:

```text
old duplicate
new duplicate
```

for the same responsibility.

---

# NO DUPLICATE ARCHITECTURE

Do NOT create:

```text
services/
features/*/services/
core/services/
shared/services/
```

with unclear overlapping responsibilities.

Do NOT create:

```text
models/
features/*/models/
shared/models/
```

unless each has a clearly different responsibility.

The architecture must be intentional.

---

# API CONTRACT SAFETY

Do not change backend API contracts unless explicitly required.

Preserve:

```text
endpoints
HTTP methods
request payloads
response payloads
authentication behavior
query parameters
route parameters
```

If a frontend refactor requires changing an API contract, STOP and document the issue instead of silently changing behavior.

---

# FUNCTIONALITY PRESERVATION

The following must continue working:

```text
Home
Authentication
Registration
Login
Menu
Reservations
Checkout
Orders
Payment success
Payment cancellation
Reviews
Contact
About
Privacy
Terms
Not Found
Arabic
English
RTL
LTR
Authentication state
Protected routes
HTTP requests
PWA/service worker
```

Do not remove functionality.

---

# PERFORMANCE

During refactoring evaluate:

```text
lazy loading
bundle size
unnecessary imports
duplicate API calls
unnecessary subscriptions
change detection
track usage
image loading
route loading
service instantiation
```

Use performance improvements only where justified.

Do not perform premature micro-optimizations.

---

# ACCESSIBILITY

During component restructuring check:

```text
semantic HTML
button vs div
labels
ARIA where necessary
keyboard navigation
focus behavior
form errors
alt text
```

Do not turn accessibility into a separate giant abstraction.

Fix obvious problems encountered during refactoring.

---

# DOCUMENTATION

Create or update:

```text
docs/ARCHITECTURE.md
```

It must explain:

```text
Architecture overview
Folder structure
Core
Shared
Features
Layout
Routing
Dependency Injection
HTTP
Authentication
State management
SOLID decisions
Testing strategy
Import dependency rules
Where to place new code
```

Include a simple dependency direction diagram:

```text
App
 ↓
Features
 ↓
Core / Shared

Shared MUST NOT depend on Features
Core MUST NOT depend on Features
```

Also document important architectural decisions and why they were made.

---

# EXECUTION STRATEGY

Execute the refactor in controlled stages.

## Stage 1

Audit.

Do not modify architecture yet.

## Stage 2

Define target architecture based on the actual codebase.

## Stage 3

Move infrastructure:

```text
core
shared
layout
```

## Stage 4

Migrate features one by one:

```text
auth
menu
reservations
checkout
orders
reviews
home
other existing features
```

Only create a feature if the existing code actually represents that feature.

## Stage 5

Fix imports and dependencies.

## Stage 6

Remove obsolete structure.

## Stage 7

Apply SOLID improvements.

## Stage 8

Improve routing/lazy loading where justified.

## Stage 9

Fix tests.

## Stage 10

Run full verification.

---

# IMPORTANT: INCREMENTAL VERIFICATION

After each major feature migration:

Run:

```bash
npm test
```

and:

```bash
npm run build
```

If available and appropriate:

```bash
npm run e2e
```

Fix errors immediately before continuing.

Do NOT accumulate hundreds of errors and fix them at the end.

---

# DO NOT OVER-ENGINEER

This is extremely important.

Do NOT introduce:

```text
Repository pattern
Unit of Work
CQRS
NgRx
facade for everything
interface for every service
abstract class for every component
factory for every service
generic base service
generic base component
five-layer architecture
```

unless the existing application has a real requirement for it.

The goal is:

```text
Simple
Clear
SOLID
Feature-oriented
Maintainable
Production-ready
```

NOT:

```text
Complex
Academic
Over-abstracted
```

---

# FINAL ARCHITECTURE QUALITY CHECK

Before finishing, verify:

## Structure

* [ ] Features are clearly isolated
* [ ] Core contains only application-wide infrastructure
* [ ] Shared contains only truly reusable code
* [ ] Layout is isolated
* [ ] No unnecessary folders
* [ ] No duplicate structures
* [ ] No obsolete folders

## SOLID

* [ ] Components have focused responsibilities
* [ ] Services have focused responsibilities
* [ ] No God components
* [ ] No God services
* [ ] Dependencies are sensible
* [ ] Abstractions exist only where useful

## Angular

* [ ] Standalone architecture preserved
* [ ] DI is clean
* [ ] Routes are organized
* [ ] Lazy loading used where useful
* [ ] HTTP architecture is clean
* [ ] Guards are focused
* [ ] Interceptors are focused

## TypeScript

* [ ] No unnecessary `any`
* [ ] Strong API typing
* [ ] Clean interfaces/types
* [ ] Consistent naming

## RxJS / State

* [ ] No obvious memory leaks
* [ ] No nested subscriptions where avoidable
* [ ] No unnecessary state management
* [ ] Signals/RxJS used appropriately

## Testing

* [ ] Existing tests preserved
* [ ] Tests moved with files
* [ ] Important architectural paths covered
* [ ] Jest passes
* [ ] Build passes
* [ ] E2E remains functional

## Functionality

* [ ] Authentication works
* [ ] Routes work
* [ ] Menu works
* [ ] Reservations work
* [ ] Checkout works
* [ ] Orders work
* [ ] Payments work
* [ ] Reviews work
* [ ] i18n works
* [ ] RTL works
* [ ] PWA remains functional

---

# FINAL CLEANUP

Search the repository for:

```text
TODO
FIXME
old folder names
duplicate services
duplicate components
unused imports
unused files
unused dependencies
dead code
any
console.log
temporary code
debug code
```

Do not blindly remove TODO/FIXME items if they represent real future work.

Remove only obsolete/debug artifacts.

---

# FINAL REPORT

When everything is complete, provide a concise but detailed report containing:

## 1. Before

Show the important old structure.

## 2. After

Show the final structure.

## 3. Major Changes

List:

```text
Moved
Renamed
Merged
Split
Deleted
Created
```

## 4. SOLID Improvements

Explain concrete examples from the actual codebase.

## 5. Critical Problems Found

List the most important architectural problems that were fixed.

## 6. Remaining Issues

If anything could not safely be changed, explain:

```text
Problem
Why it remains
Risk
Recommended next step
```

## 7. Verification

Report exact results of:

```text
npm test
npm run build
npm run e2e
```

Do not claim a command passed unless you actually executed it.

---

# FINAL SUCCESS CRITERIA

The task is complete ONLY when:

1. Existing functionality is preserved.
2. Architecture is clearly feature-oriented.
3. Core/shared/feature responsibilities are clear.
4. SOLID has been applied to real code.
5. Duplicate responsibilities are removed.
6. Old architecture is cleaned up.
7. Imports are clean.
8. Circular dependencies are resolved.
9. Tests are preserved/fixed.
10. Build succeeds.
11. No unnecessary architecture has been introduced.
12. The resulting codebase is something a Senior Angular Developer could confidently explain in a technical interview.

DO NOT STOP AFTER ANALYSIS.

EXECUTE THE CHANGES.

DO NOT ONLY GIVE RECOMMENDATIONS.

Inspect → Plan → Refactor → Verify → Clean → Document.
