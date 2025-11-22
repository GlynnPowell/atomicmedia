## Atomic Tasks – Full-Stack Task Management Application

This repository contains a small full‑stack task management application implemented for a technical assessment.
The focus is on clean architecture, straightforward local setup, and demonstrating awareness of microservices and Azure without over‑engineering the solution.

## Tech stack

- **Backend**: .NET 9 (ASP.NET Core) REST API
- **Frontend**: React with TypeScript
- **Database**: SQLite (via Entity Framework Core)
- **Cloud target**: Azure (App Service or Container Apps + managed database), described at a high level

## Project structure

The solution is organised as a **modular monolith**: a single deployable backend with clear internal boundaries, plus a separate React client.

```text
.
├─ README.md
├─ AtomicTasks.sln                 # .NET solution
├─ src/
│  ├─ server/
│  │  ├─ AtomicTasks.Api/          # ASP.NET Core API (endpoints, startup, DI)
│  │  ├─ AtomicTasks.Application/  # Use-cases, services, DTOs, interfaces
│  │  ├─ AtomicTasks.Domain/       # Domain entities and core rules
│  │  └─ AtomicTasks.Infrastructure/ # EF Core, SQLite, repositories, migrations
│  └─ client/
│     └─ (React + TypeScript app)  # UI, routing, API calls, local state
└─ tests/
   └─ server/AtomicTasks.Tests/    # Backend unit/integration tests
```

This structure keeps responsibilities separated and makes it easy to explain how the backend could later be split into independent microservices (for example, by extracting `Tasks` into its own service).

## Running the application (local, no Docker)

From the repository root:

- **Backend (.NET 9 API with SQLite)**
  - Restore and build (first time):
    - `dotnet restore`
  - Run the API:
    - `dotnet run --project src/server/AtomicTasks.Api/AtomicTasks.Api.csproj`
  - The API will listen on `http://localhost:5286` (per `launchSettings.json`), with task endpoints under `http://localhost:5286/api/tasks`.

- **Frontend (React + TypeScript)**
  - In a separate terminal:
    - `cd src/client`
    - `npm install` (first time only)
    - On PowerShell:
      - `$env:VITE_API_BASE_URL="http://localhost:5286/api"`
    - Then start the dev server:
      - `npm run dev`
  - Open the browser at `http://localhost:5173` to use the app.

- **Tests**
  - Run backend tests from the repo root:
    - `dotnet test AtomicTasks.sln`

## Architecture overview

- **Domain layer (`AtomicTasks.Domain`)**: core business concepts, such as the `Task` entity, status enums, and domain rules (e.g. allowed status transitions).
- **Application layer (`AtomicTasks.Application`)**: use-cases and services (e.g. create/update/delete task, change status), expressed in terms of interfaces and DTOs.
- **Infrastructure layer (`AtomicTasks.Infrastructure`)**: EF Core DbContext configured for SQLite, repository implementations, and migrations.
- **API layer (`AtomicTasks.Api`)**: ASP.NET Core endpoints, dependency injection configuration, validation, and HTTP concerns.
- **Frontend (`src/client`)**: React + TypeScript SPA that consumes the API, providing task CRUD, filtering, and a clean, responsive UI with proper loading and error states.

This “modular monolith” layout is intentionally simple to run as a single service while still reflecting microservice-friendly boundaries.

### Known limitations and future improvements

- **Authentication/authorisation**: The app is intentionally unauthenticated; in a real system you would integrate with an identity provider and scope tasks per user.
- **Validation and error handling**: Only basic validation is implemented (primarily around titles). With more time, richer domain validation and a consistent problem-details contract would be added.
- **Testing**: The current test suite covers mapping logic and repository integration; additional API-level tests and front-end tests would be the next focus.
- **Persistence**: SQLite is sufficient for this assessment; in production you would use a managed database (e.g. Azure SQL) and apply migrations as part of deployment.

## Deployment to Azure (outline)

Azure deployment is **not required** to run or review this assessment, but a simple path would be:

- **Compute**: Deploy the backend to **Azure App Service** as a .NET 9 application.
- **Data**: Use **Azure SQL Database** as the managed database, with the EF Core migrations applied at deploy time or via a one-off migration step.
- **Configuration**:
  - Store the connection string in App Service configuration as `ConnectionStrings:DefaultConnection`.
  - For production secrets, prefer Azure Key Vault referenced from App Service settings.
- **Frontend**: Either
  - Serve the built React app from Azure Static Web Apps or Azure Storage Static Website, pointing `VITE_API_BASE_URL` at the App Service URL, or
  - Host the frontend from the same App Service (e.g. as static files) and keep the API under `/api`.
- **IaC (optional)**: With more time, a small Bicep or Terraform template (under `infra/`) could declaratively create the App Service, Azure SQL instance, and supporting resources.

The goal here is to show a clear path to Azure without over-investing in cloud infrastructure for the purposes of this coding exercise.

## Use of AI tools (Cursor + ChatGPT-5)

This project was developed with the assistance of **Cursor** and **ChatGPT‑5** as coding and architecture companions.
They were used to:

- Help break down the assessment into clear steps and a TODO plan.
- Discuss architectural options (modular monolith vs microservices) and arrive at the current structure.
- Generate or refine boilerplate code, configuration snippets, and documentation, which were then reviewed and adapted manually.

All key design decisions, final code, and trade-offs were made and checked by the author; AI outputs were treated as suggestions, not ground truth.

### Implementation plan / TODO

The following TODO list captures the high-level plan used to tackle the assessment:

1. **Clarify requirements** – Extract assessment requirements and constraints (features, tech constraints, AI usage rules, submission format). *(Completed)*
2. **Choose stack & layout** – Decide on React + TypeScript frontend, .NET 9 API backend, SQLite database, and the project structure above. *(Completed)*
3. **Design architecture** – Finalise the modular monolith architecture with clear `Domain`, `Application`, `Infrastructure`, and `Api` boundaries, and explain how it could evolve into microservices. *(Completed)*
4. **Data model & API surface** – Define the `Task` domain model (entities, DTOs, enums) and REST API endpoints, including validation rules. *(Completed)*
5. **Backend core implementation** – Implement task CRUD, business rules, persistence with SQLite, error handling, and configuration for local development. *(Completed)*
6. **Frontend core implementation** – Build the React + TypeScript UI (task list, create/edit/delete, status changes, filtering, and UX details like loading and error states). *(Completed)*
7. **Testing** – Add essential tests (unit tests for domain and application logic; a small number of integration-style tests using the EF Core repository). *(Completed)*
8. **Azure notes** – Document an outline for deploying to Azure, including configuration via environment variables and Key Vault. *(Completed)*
9. **Docs & polish** – Finalise this README with concrete run instructions, record trade-offs and future improvements, and do a final UX and code cleanup pass. *(In progress)*

