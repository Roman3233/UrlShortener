# 🔗 UrlShortener — Full-Stack URL Shortening Application

  

A full-stack, URL Shortener built as part of the Technical Task. The application allows users to shorten any URL, navigate via generated short equivalents, manage links based on role permissions, and view/edit an explanation of the underlying shortening algorithm.

  

---

## 🚀 Technical Stack

  

- **Backend:** ASP.NET Core 9 (Web API & MVC Razor Pages)

- **Frontend:** Angular 21 (Latest version, Standalone Components, Signals, SCSS)

- **Database & ORM:** PostgreSQL + Entity Framework Core 9 (Code-First Approach)

- **Testing:** xUnit + Moq (48 unit tests covering core services, security, and handlers)

- **Authentication:** Cookie-based Authentication with Sliding Expiration & Role-Based Access Control (Admin / User)

- **Error Handling:** Centralized Global Exception Handling implementing .NET 9 `IExceptionHandler` 

  

---

  

## 🏛️ Solution Architecture (Clean / Onion Architecture)

  

The solution is divided into loosely coupled, highly testable layers with clear dependency boundaries:

  

```

UrlShortener/
├── UrlShortener.Core/          # Domain Layer: Entities, Interfaces, DTOs, Exceptions, Services (Zero external dependencies)
├── UrlShortener.Data/          # Infrastructure/Persistence: AppDbContext, Repositories, Migrations, DbInitializer
├── UrlShortener.Web/           # Presentation Layer: API Controllers, Razor Page (About), Middleware, Auth Configuration
├── UrlShortener.Tests/         # Test Suite: Unit tests for core services, encoders, security, and exception handling
└── ClientApp/                  # Frontend Layer: Angular 21 SPA (Standalone Components, Signals, Reactive Toasts)

```

  

### Layer Breakdown

  

1. **`UrlShortener.Core`**

   - **Entities:** [`User`](file:///d:/Projects/UrlShortener/UrlShortener.Core/Entities/User.cs), [`ShortUrl`](file:///d:/Projects/UrlShortener/UrlShortener.Core/Entities/ShortUrl.cs), [`AboutContent`](file:///d:/Projects/UrlShortener/UrlShortener.Core/Entities/AboutContent.cs)

   - **Interfaces:** `IShortUrlService`, `IShortUrlRepository`, `IShortCodeGenerator`, `IAuthService`, `IUserRepository`, `IPasswordHasher`

   - **Services:**

     - `Base62Encoder`: Encodes integer record IDs to compact alphanumeric strings and decodes them back.

     - `ShortUrlService`: Business logic for URL creation, duplication checks, and role/ownership delete authorization.

     - `AuthService`: Authentication and user registration logic.

     - `PasswordHasher`: Pure .NET cryptographic PBKDF2 implementation with unique salts and fixed-time verification.

   - **DTOs:** `LoginRequest`, `UserDto`, `CreateShortUrlRequest`, `ShortUrlDto`

   - **Exceptions:** `DuplicateUrlException`, `ShortUrlNotFoundException`, `ForbiddenDeleteException`, `UserAlreadyExistsException`

  

2. **`UrlShortener.Data`**

   - **`AppDbContext`:** EF Core Fluent API configuration enforcing uniqueness on `OriginalUrl`, `ShortCode`, and `Login`.

   - **Repositories:** `ShortUrlRepository` and `UserRepository` with async EF Core queries and eager loading.

   - **`DbInitializer`:** Automatically applies pending migrations (`MigrateAsync()`) and seeds initial Admin and User accounts on application startup.

  

3. **`UrlShortener.Web`**

   - **`AuthController`:** Endpoints for `/api/auth/login`, `/api/auth/logout`, and `/api/auth/me`.

   - **`ShortUrlsController`:** Endpoints for CRUD operations on shortened links (`/api/shorturls`).

   - **`RedirectController`:** Resolves short codes (`GET /{shortCode}`) and issues HTTP 302 redirects to the destination URL.

   - **`AboutController` + Razor View:** Standalone MVC Razor Page (`/About`) displaying the algorithm, with an admin-only form (`POST /About`) to edit the description.

   - **`GlobalExceptionHandler`:** Translates domain exceptions into standard HTTP status codes (`409 Conflict`, `404 Not Found`, `403 Forbidden`, `400 Bad Request`, `500 Internal Server Error`).

  

4. **`ClientApp` (Angular 21)**

   - **State Management:** Angular Signals (`signal`, `computed`) for reactive, reload-free UI updates.

   - **HTTP Interceptor:** `CredentialsInterceptor` sends `withCredentials: true` across all requests for smooth cross-origin cookie sharing.

   - **Guards:** `authGuard` prevents unauthenticated access to the Short URL Info view.

   - **Toasts:** Custom Signal-based Toast notification system (`ToastService` + `ToastComponent`) with auto-dismiss and manual close.

  

---

  

## 🧮 URL Shortening Algorithm (Base62 Encoding)

  

The application uses **Base62 Encoding** on the database entity's primary key (`Id`):

  

$$\text{ShortCode} = \text{Base62}(\text{Id})$$

  

### Alphabet

`0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ` (62 alphanumeric characters)

  

### Why Base62?

1. **Collision-Free:** Each database record has a guaranteed unique auto-incrementing ID; therefore, every generated code is mathematically unique without needing collision-resolution retry loops.

2. **Ultra-Compact:**

   - 1st record $\to$ `1` (1 character)

   - 100,000th record $\to$ `q0U` (3 characters)

   - 14,776,336th record $\to$ only 4 characters

3. **URL-Safe:** Contains only alphanumeric characters without special symbols like `+`, `/`, `=`, or `?`.

---


## ⚙️ Getting Started & Running Locally

  

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)

- [Node.js](https://nodejs.org/) (v18+ recommended)

- [PostgreSQL](https://www.postgresql.org/) (running locally on port `5432` or configured via connection string)

---
### 1. Database Setup

The connection string is configured in UrlShortener.Web/appsettings.json/appsettings.json 

```json

{

  "ConnectionStrings": {

    "DefaultConnection": "Host=localhost;Port=5432;Database=urlshortener;Username=postgres;Password=admin"

  }

}

```

> **Note:** You do **not** need to manually execute `dotnet ef database update`. On startup, `DbInitializer` automatically applies pending EF Core migrations and seeds initial accounts.

  

---

  

### 2. Run the Backend (ASP.NET Core)

  

```powershell

dotnet run --project .\UrlShortener.Web\UrlShortener.Web.csproj

```

The server will start on:

- API / MVC: `http://localhost:5062` (and `https://localhost:7051`)

- About Razor Page: `http://localhost:5062/About`

  

---

### 3. Run the Frontend (Angular SPA)


Open a new terminal in the `ClientApp` directory:


```powershell

cd ClientApp

npm install

npm start

```

The Angular application will launch at:

👉 **`http://localhost:4200`**

---
### 4. Pre-Seeded Test Credentials

  

| Role          | Username          | Password   | Permissions                                                                             |
| :------------ | :---------------- | :--------- | :-------------------------------------------------------------------------------------- |
| **Admin**     | `admin`           | `admin123` | Add URLs, view details, delete **all** URLs, edit the About page algorithm description. |
| **User**      | `user`            | `user123`  | Add URLs, view details, delete **only their own** URLs.                                 |
| **Anonymous** | *(Not logged in)* | —          | View the table of URLs (read-only), view the About page.                                |

---
## 📂 Project Structure Map


```
│
├── UrlShortener.sln
│
├── UrlShortener.Core/
│   ├── Entities/               # User, ShortUrl, AboutContent
│   ├── Interfaces/             # IShortUrlService, IShortUrlRepository, IAuthService, etc.
│   ├── Services/               # ShortUrlService, Base62Encoder, AuthService, PasswordHasher
│   ├── DTOs/                   # LoginRequest, UserDto, CreateShortUrlRequest, ShortUrlDto
│   └── Exceptions/             # DuplicateUrlException, ShortUrlNotFoundException, etc.
│
├── UrlShortener.Data/
│   ├── AppDbContext.cs         # Fluent API database configuration
│   ├── DbInitializer.cs        # Automatic migrations & seed data
│   ├── Repositories/           # ShortUrlRepository, UserRepository
│   └── Migrations/             # EF Core Code-First migrations
│
├── UrlShortener.Web/
│   ├── Controllers/            # AuthController, ShortUrlsController, RedirectController, AboutController
│   ├── Middleware/             # GlobalExceptionHandler (.NET 9 IExceptionHandler)
│   ├── Views/                  # About/Index.cshtml, Shared/_Layout.cshtml
│   ├── Program.cs              # DI, Cookie Auth, CORS, and Middleware pipeline
│   └── appsettings.json
│
├── UrlShortener.Tests/
│   ├── Base62EncoderTests.cs
│   ├── ShortUrlServiceTests.cs
│   ├── AuthServiceTests.cs
│   ├── PasswordHasherTests.cs
│   └── GlobalExceptionHandlerTests.cs
│
└── ClientApp/
    └── src/app/
        ├── components/
        │   ├── short-urls-table/   # Main table, Add section, Delete, Copy
        │   ├── short-url-info/     # Details page (Protected by AuthGuard)
        │   ├── login/              # Sign in page with demo accounts
        │   └── toast/              # Reactive floating notification container
        ├── services/               # AuthService, ShortUrlService, ToastService
        ├── guards/                 # authGuard
        ├── interceptors/           # CredentialsInterceptor
        └── models/                 # TypeScript interfaces

```
