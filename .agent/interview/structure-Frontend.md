have now:

Dawar-Kitchen-Full
└── dawar-kitchen
    └── src
        ├── app
        │   ├── components
        │   ├── directives
        │   ├── guards
        │   ├── interceptors
        │   ├── models
        │   ├── pages
        │   ├── sections
        │   ├── services
        │   ├── app.component.ts
        │   ├── app.config.ts
        │   └── app.routes.ts

tradoffs:
1-legacy
    Application Root
       ↓
AppComponent
       ↓
Router Outlet
       ↓
Pages

enhancements: