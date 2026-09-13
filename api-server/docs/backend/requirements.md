Functional requirements (exact contracts the Angular app expects):
Auth: login / register / logout / reset-password / me
Menu (with category filter + dietary flags)
Chefs
Reservations (create + list/update/delete)
Orders (collection / delivery / dine-in)
Stripe checkout session + webhook
Contact form
Health check

Non-functional requirements:
Clean Architecture + CQRS
Security (JWT, CORS, rate limits, Stripe signature, secrets via env)
Performance, health, Docker, seeding, bilingual readiness
Testing & observability

enhnacments:

Priority 0Contract alignment with frontend (auth, menu, chefs, reservations, orders, payments, contact) so mocks can be removed
Priority 1 – New featuresAdvanced menu search/filter, email notifications, order status + realtime, reviews, staff/admin, bilingual EN/AR
Priority 2 Security, performance, caching, tests, observabilityImplementation orderClear step-by-step sequenceSuccess criteriaChecklist to know when the enhancement is done