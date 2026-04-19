# All The Beans

A full-stack coffee bean ordering application built with a .NET API backend and React frontend, secured with Microsoft Entra ID (Azure AD) authentication.

---

## Running the Application

### What you need installed
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (version 20 or above)
- Test credentials provided separately by the developer

### Step 1 - Start the API
1. Open a terminal and navigate to the API project folder (the folder containing AllTheBeans.sln)
2. Run:
   ```
   dotnet run --project AllTheBeans.Api
   ```
3. Wait until you see Now listening on: https://localhost:7069 - the API is ready

### Step 2 - Start the Frontend
1. Open a second terminal and navigate to the frontend folder (the folder containing package.json)
2. Run:
   ```
   npm run dev
   ```
3. Wait until you see Local: http://localhost:5173/ - the frontend is ready

### Step 3 - Use the Application
1. Open your browser and go to http://localhost:5173/
2. Click Sign in with Microsoft
3. Enter the credentials provided separately
4. You can now browse beans, view details, and place orders

### To stop the application
Press Ctrl+C in each terminal window.

---

## Architecture and Patterns

### CQRS - Command Query Responsibility Segregation

The API separates read operations (Queries) from write operations (Commands). For example:

- SearchBeansQuery fetches a list of beans (read)
- CreateBeanCommand creates a new bean (write)

This separation makes the codebase easier to reason about. Reads and writes have different concerns - reads need to be fast and flexible, writes need to be validated and transactional.

### MediatR

MediatR is a library that acts as an in-process message bus. Instead of controllers calling services directly, they send a message (a Query or Command) to MediatR, which routes it to the correct handler.

This means controllers are thin - they receive an HTTP request, create a message, and return a response. All business logic lives in the handler. This makes the code easier to test and maintain, and means adding new features does not require changing existing code.

### FluentValidation

All commands are validated before they reach the handler. FluentValidation defines rules in a dedicated class - for example, ensuring a bean name is not empty, or a price is greater than zero. If validation fails, a structured error is returned immediately without hitting the database.

This is wired into the MediatR pipeline via ValidationBehaviour, meaning validation runs automatically for every command without any extra code in the handlers.

### Repository Pattern and Unit of Work

The API does not use Entity Framework directly in handlers. Instead, it uses repository interfaces (IBeanRepository, IOrderRepository, etc.) that abstract the database operations.

This means if the database technology ever changes, only the repository implementations change - not the business logic. The Unit of Work (IUnitOfWork) ensures that multiple repository operations within a single request are committed together as one transaction, so the database is never left in a partial state.

### Middleware

Two custom middleware components run on every request before it reaches a controller:

ExceptionHandlingMiddleware catches any unhandled exceptions and returns a consistent, structured error response. Without this, .NET would return a raw 500 error. This middleware ensures the API always returns a predictable JSON error format.

SecurityHeadersMiddleware adds HTTP security headers to every response. These headers instruct the browser to apply protections such as preventing the page from being embedded in an iframe (clickjacking protection) and restricting what resources the page can load (Content Security Policy). These are standard security best practices for any web application.

### Authentication - Microsoft Entra ID (Azure AD)

The application uses Microsoft Entra ID as its identity provider. This means the application never stores or handles passwords itself. Authentication is delegated entirely to Microsoft.

The flow works as follows:

1. The user clicks Sign in with Microsoft in the React app
2. The app redirects the user to Microsoft's login page
3. After successful login, Microsoft issues a signed JWT (JSON Web Token) - a token that proves who the user is
4. The React app includes this token in every API request as a Bearer token in the Authorization header
5. The .NET API validates the token on every request - checking it was issued by the correct Microsoft tenant, for the correct application, and that it has not expired
6. If the token is valid, the request proceeds. If not, the API returns 401 Unauthorized

The React app uses the MSAL (Microsoft Authentication Library) to handle the login redirect and token management. Tokens are cached in session storage so the user does not have to log in on every page.

### Authorization

The API uses two levels of authorization:

- Read endpoints (GET /api/beans, GET /api/bean-of-the-day) are public - any user can browse the catalogue
- Write endpoints (POST, PUT, DELETE on /api/beans and POST /api/orders) require the Writer policy - only authenticated users can modify data or place orders

This is enforced with [Authorize] and [Authorize(Policy = "Writer")] attributes on the controller actions.

### CORS - Cross-Origin Resource Sharing

Browsers block requests from one domain to another by default. Since the React app runs on http://localhost:5173 and the API runs on https://localhost:7069, CORS must be explicitly configured on the API to allow this. The API is configured to only allow requests from http://localhost:5173, meaning no other website can call the API from a browser.

---

## Project Structure

AllTheBeans/
  AllTheBeans.Api/            .NET Web API
    Controllers/              HTTP endpoints (thin - just routing)
    Application/
      Beans/
        Commands/             Write operations (Create, Update, Delete)
        Queries/              Read operations (Search, GetById)
        Dtos/                 Data shapes returned to the client
      BeanOfTheDay/           Bean of the day feature
      Behaviours/             MediatR pipeline (validation)
      Common/                 Shared interfaces (IDateTimeProvider etc.)
    Infrastructure/
      Persistence/            Entity Framework, repositories, seeding
      Services/               Infrastructure service implementations
    Middleware/               Exception handling, security headers

  AllTheBeans.Tests/          Unit and integration tests
    Unit/                     Business logic and validation tests
    Integration/              Full API endpoint tests

  client/                     React + Vite frontend
    src/
      auth/                   MSAL configuration
      api/                    API client (attaches Bearer token)
      components/             Reusable UI components
      pages/                  Page-level components

---

## Testing

### Running Tests

From the solution root, run:

```
dotnet test
```

This runs all 15 tests (unit and integration) and reports results in the terminal.

Alternatively, in Visual Studio: Test menu, Run All Tests (or Ctrl+R, A).

### Unit Tests

Unit tests verify individual pieces of business logic in isolation, with no database or HTTP involved. Dependencies are replaced with mocks using Moq.

BeanOfTheDayServiceTests - tests the core Bean of the Day selection logic:
- Returns the existing selection if today's bean has already been picked
- Excludes the previous day's bean when making a new selection
- Returns null gracefully when the catalogue is empty

CreateBeanValidatorTests - tests the FluentValidation rules for creating a bean:
- Accepts valid input
- Rejects empty names
- Rejects negative costs

PlaceOrderValidatorTests - tests the validation rules for placing an order:
- Accepts valid orders
- Rejects empty emails, zero quantities, and missing bean IDs

### Integration Tests

Integration tests start the full API in memory using WebApplicationFactory and make real HTTP requests against it. The database is swapped for an in-memory SQLite instance so no external infrastructure is needed.

Authentication is replaced with a fake handler that always succeeds, allowing tests to call authenticated endpoints without needing real Entra ID tokens.

BeansApiTests - tests the API endpoints end-to-end:
- GET /api/beans returns an empty list when no beans exist
- POST /api/beans followed by GET /api/beans/{id} completes a full create-and-retrieve round trip
- GET /api/beans/{id} returns 404 for a nonexistent bean
- GET /api/bean-of-the-day returns a bean after seeding the catalogue
- POST /api/beans with invalid data returns 400 with validation errors

### Test Architecture

Tests use a CustomWebApplicationFactory that:
- Replaces SQL Server with SQLite in-memory (relational, so EF.Functions.Like still works)
- Replaces Entra ID authentication with a fake handler that auto-authenticates as a test user
- Skips the database seed that runs in production startup
- Creates a fresh database schema for each test run