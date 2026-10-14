# Nexora — Digital Services Platform

A full-stack platform built with an **ASP.NET Core 8 Web API** backend (Clean / Onion Architecture) and an **Angular 11** single-page front-end. It supports product browsing with filtering, sorting and pagination, JWT-based authentication, a Redis-backed shopping basket, order management, and Stripe payments.

```
Cartly - ASP.NET Core API/
├── backend-dotnet/     # Backend  (ASP.NET Core 8 Web API)
└── frontend-angular/  # Frontend (Angular 11 SPA)
```

---

## Tech Stack

| Layer        | Technology                                                                 |
|--------------|---------------------------------------------------------------------------|
| **Backend**  | ASP.NET Core 8, EF Core 8 (SQL Server), ASP.NET Core Identity, JWT Bearer, AutoMapper, Redis (StackExchange.Redis), Stripe.NET, Swagger |
| **Frontend** | Angular 11, RxJS, Bootstrap 4 / Bootswatch, ngx-bootstrap, ngx-toastr, ngx-spinner, Stripe.js |
| **Database** | SQL Server (two databases: store + identity), Redis (basket cache & response cache) |

---

## Backend Architecture

The API follows **Clean Architecture**, split into independent projects so that dependencies always point inward toward the domain:

```
E-Commerce_APIs-master/
├── Core/
│   ├── Domain/                 # Entities, contracts (interfaces), domain exceptions
│   ├── Services.Abstractions/  # Service interfaces (IProductService, IAuthService, ...)
│   └── Services/               # Business logic, AutoMapper profiles, specifications
├── Infrastructure/
│   ├── Persistence/            # EF Core DbContexts, repositories, UnitOfWork, Redis, seeding
│   └── Presentation/           # API Controllers + custom attributes (e.g. [Cache])
├── Shared/                     # DTOs + error models shared across layers
└── E-Commerce/                 # Startup host (Program.cs, DI wiring, middleware, settings)
```

**Request flow:** `Controller (Presentation)` → `IServiceManager` → `Service (Core)` → `Repository / UnitOfWork (Persistence)` → `DbContext / Redis`. Controllers never touch EF Core directly — they go through the service layer, and entities are mapped to **DTOs** before leaving the API.

### Key backend features
- **Two `DbContext`s** — `StoreDbContext` (products, orders, baskets) and `IdentityStoreDbContext` (users/roles), each with its own connection string.
- **JWT authentication** via ASP.NET Core Identity. Tokens are signed with a symmetric key and validated for issuer, audience and lifetime ([Extensions.cs](E-Commerce_APIs-master/E-Commerce/ExtensionsMethod/Extensions.cs)).
- **Redis** is used both as the shopping-basket store and as a response cache (via the custom `[Cache]` attribute on product endpoints).
- **Generic Repository + Specification pattern + Unit of Work** for clean, testable data access.
- **Global error-handling middleware** produces consistent error payloads; model-validation errors are reshaped into a uniform `ValidationErrorResponse`.
- **Data seeding** runs on startup to populate products and identity (roles/users).
- **Stripe** integration for payment intents and a webhook endpoint to confirm payment status.

### Main API endpoints

| Method | Route                          | Auth | Description                          |
|--------|--------------------------------|------|--------------------------------------|
| GET    | `/api/products`                | —    | Paginated products (filter/sort/search) |
| GET    | `/api/products/{id}`           | —    | Single product                       |
| GET    | `/api/products/brands`         | —    | All brands                           |
| GET    | `/api/products/types`          | —    | All product types                    |
| POST   | `/api/auth/login`              | —    | Login, returns JWT                   |
| POST   | `/api/auth/register`           | —    | Register, returns JWT                |
| GET    | `/api/auth`                    | ✔    | Current user                         |
| GET/PUT| `/api/auth/address`            | ✔    | Get / update user address            |
| GET    | `/api/baskets?id=`             | —    | Get basket (Redis)                   |
| POST   | `/api/baskets`                 | —    | Create / update basket               |
| DELETE | `/api/baskets?id=`             | —    | Delete basket                        |
| POST   | `/api/orders`                  | ✔    | Create order                         |
| GET    | `/api/orders`                  | ✔    | Current user's orders                |
| GET    | `/api/orders/deliveryMethods`  | —    | Delivery methods                     |
| POST   | `/api/payments/{basketId}`     | —    | Create/update Stripe payment intent  |
| POST   | `/api/payments/webhook`        | —    | Stripe webhook                       |

Interactive docs are available via **Swagger** at `https://localhost:7065/swagger` in development.

---

## Frontend Architecture

The Angular app is a feature-module SPA:

```
E-Commerce_Front-end-main/src/app/
├── account/      # login, register, AccountService (auth + JWT storage)
├── shop/         # product list, details, filtering (ShopService)
├── basket/       # basket + Stripe payment intent (BasketService)
├── checkout/     # address → delivery → review → payment → success
├── orders/       # order history & detail
├── home/         # landing page
├── shared/       # models (interfaces) + reusable components
└── core/
    ├── interceptors/   # jwt, error, loading
    ├── guards/         # route protection
    └── services/       # busy/loading service
```

Each feature has a service that wraps `HttpClient` calls to the corresponding API controller — e.g. `ShopService` → `/api/products`, `AccountService` → `/api/auth`, `BasketService` → `/api/baskets` & `/api/payments`.

---

## How the Frontend & Backend Are Connected

### 1. Base URL via environment files
The Angular app never hard-codes the API host. It reads `environment.apiUrl`, and every service does `baseUrl = environment.apiUrl`:

```ts
// src/environments/environment.ts (development)
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7065/api/'   // ← points to the ASP.NET Core API
};
```

The backend listens on `https://localhost:7065` (see [launchSettings.json](E-Commerce_APIs-master/E-Commerce/Properties/launchSettings.json)), which is exactly the host the frontend targets. To deploy elsewhere, change only `apiUrl` — no code changes needed.

### 2. CORS allows the browser to call the API
Browsers block cross-origin requests by default. The API enables a permissive CORS policy so the Angular dev server (`http://localhost:4200`) can reach it:

```csharp
// Extensions.cs
Options.AddPolicy("AllowAll", builder =>
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
...
app.UseCors("AllowAll");
```

### 3. JWT auth flows through an HTTP interceptor
On login/register, the API returns a JWT which the frontend stores in `localStorage`:

```ts
// account.service.ts
login(values) {
  return this.http.post(this.baseUrl + 'Auth/login', values).pipe(
    map((user: IUser) => localStorage.setItem('token', user.token))
  );
}
```

A **JWT interceptor** then attaches that token to every outgoing request, so protected endpoints (`[Authorize]`) like orders and user address work transparently:

```ts
// core/interceptors/jwt.interceptor.ts
const token = localStorage.getItem('token');
if (token) {
  req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}
```

On the API side, `app.UseAuthentication()` / `app.UseAuthorization()` validate that bearer token against the configured issuer, audience, lifetime and signing key.

### 4. Consistent error & loading handling
- An **error interceptor** catches API error responses (400/401/404/500) and surfaces them via toasts / the server-error page, matching the API's uniform error model (`ErrorDetails` / `ValidationErrorResponse`).
- A **loading interceptor** + `BusyService` show a spinner while requests are in flight.

### 5. Data contracts (DTOs ↔ models)
The API returns **DTOs** from the `Shared` project (e.g. `ProductResponse`, `PaginationResponse<T>`). The frontend mirrors these as TypeScript interfaces in `shared/models` (e.g. `IProduct`, `IPagination`), so responses deserialize cleanly into typed objects.

### Connection at a glance

```
┌──────────────────────────┐         HTTPS / JSON          ┌───────────────────────────┐
│  Angular SPA (:4200)      │  ───────────────────────────▶ │  ASP.NET Core API (:7065) │
│                           │   Authorization: Bearer <JWT> │                           │
│  Services (HttpClient)    │ ◀─────────────────────────── │  Controllers → Services    │
│  + JWT/Error/Loading      │       DTOs (JSON)             │  → Repos → SQL Server      │
│    interceptors           │                               │  + Redis + Stripe          │
└──────────────────────────┘                               └───────────────────────────┘
        environment.apiUrl ──────────────────────────────────────▶ CORS "AllowAll"
```

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) + Angular CLI 11 (`npm i -g @angular/cli@11`)
- SQL Server (local instance — connection strings use `Server=.`)
- Redis (running on `localhost`)

### 1. Run the backend
```bash
cd "E-Commerce_APIs-master"
dotnet restore
# (optional) set your Stripe keys in E-Commerce/appsettings.json
dotnet run --project E-Commerce
```
- The app **auto-applies migrations / seeds data** on startup (no manual `dotnet ef database update` required).
- API: `https://localhost:7065`  •  Swagger: `https://localhost:7065/swagger`

> Configuration lives in [appsettings.json](E-Commerce_APIs-master/E-Commerce/appsettings.json): SQL connection strings (`DefaultConnection`, `IdentityConnection`), `RedisConnection`, `JwtOptions`, and `Stripe` keys.

### 2. Run the frontend
```bash
cd "E-Commerce_Front-end-main"
npm install
npm start          # ng serve -o
```
- App opens at `http://localhost:4200` and talks to the API at `https://localhost:7065/api/`.

> **Tip:** because the API uses a self-signed dev certificate, visit `https://localhost:7065/swagger` once in the browser and accept the certificate so the SPA's HTTPS calls succeed.

---

## Build for Production
```bash
# Backend
dotnet publish "E-Commerce_APIs-master/E-Commerce" -c Release

# Frontend (uses environment.prod.ts → apiUrl: 'api/')
cd "E-Commerce_Front-end-main" && ng build --prod
```
For production, set `environment.prod.ts`'s `apiUrl` to your deployed API URL (or serve the Angular build behind the same origin as the API and keep the relative `api/`).
