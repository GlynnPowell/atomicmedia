## Atomic Tasks – Full-Stack Task Management Application

This repository contains a small full‑stack task management application implemented for a technical assessment.
This `README` is split into two parts:

- **Part 1** – how to **set up, run, and test** the application end‑to‑end.
- **Part 2** – the **development method**, architectural decisions, and **AI usage** notes that explain *why* it was built this way.

## Tech stack (overview)

- **Backend**: .NET 9 (ASP.NET Core) REST API
- **Frontend**: React with TypeScript (Vite)
- **Database**: SQLite (via Entity Framework Core)

## Project layout (quick reference)

```text
.
├─ README.md                      # Setup, run, test, and design notes (this file)
├─ AtomicTasks.sln                # .NET solution
├─ src/
│  ├─ server/
│  │  ├─ AtomicTasks.Api/         # ASP.NET Core API
│  │  ├─ AtomicTasks.Application/ # Use-cases, services, DTOs, interfaces
│  │  ├─ AtomicTasks.Domain/      # Domain entities and core rules
│  │  └─ AtomicTasks.Infrastructure/ # EF Core, SQLite, repositories
│  └─ client/                     # React + TypeScript app
└─ tests/
   └─ server/AtomicTasks.Tests/   # Backend unit/integration tests
```

## Running the application (local, no Docker)

From the repository root:

- **Backend (.NET 9 API with SQLite)**
  - Restore and build (first time):
    - `dotnet restore`
  - Run the API:
    - `dotnet run --project src/server/AtomicTasks.Api/AtomicTasks.Api.csproj`
  - The API listens on `http://localhost:5286` (per `launchSettings.json`), with task endpoints under `http://localhost:5286/api/tasks`.
  - SQLite database file `AtomicTasks.db` is created in the API project folder if it does not already exist.

- **Frontend (React + TypeScript)**
  - In a separate terminal:
    - `cd src/client`
    - `npm install` (first time only)
    - On PowerShell, point the UI at the API:
      - `$env:VITE_API_BASE_URL="http://localhost:5286/api"`
    - Start the dev server:
      - `npm run dev`
  - Open the browser at `http://localhost:5173/tasks` to use the app.

## Running tests

All tests should be run from a clean checkout after `dotnet restore` and `npm install` have completed successfully.

- **Backend tests** (xUnit, EF Core, Moq)
  - From the repository root:
    - `dotnet test AtomicTasks.sln`

- **Frontend unit tests** (Vitest + React Testing Library)
  - From `src/client`:
    - `npm test`

- **Frontend end-to-end tests** (Playwright)
  - Ensure the backend API and frontend dev server are running:
    - Terminal 1 (from repo root):
      - `dotnet run --project src/server/AtomicTasks.Api/AtomicTasks.Api.csproj`
    - Terminal 2 (from `src/client`):
      - `$env:VITE_API_BASE_URL="http://localhost:5286/api"`
      - `npm run dev`
  - Then, in Terminal 3 (from `src/client`), run:
    - `npm run test:e2e`

The E2E tests exercise the core flows required by the assessment: creating a task and toggling its completion status through the real UI and API.

## Notes for reviewers

- The implementation is aligned with the assessment specification for the `Task` model, API surface, and frontend behaviour.
- The sections below document how the assessment was approached (including TODOs, iterations, and AI involvement) and can be used as a narrative of the development process.

---

## Development method, architecture, and AI usage

This part of the `README` explains **how** the technical assessment was tackled:

- The architectural approach and how it maps to the brief.
- Key design decisions and any changes made along the way.
- How AI tools (Cursor and ChatGPT‑5) were used.
- The high‑level TODO / implementation plan that guided development.

### 1. Architectural approach

#### 1.1 Overall style – modular monolith

The backend is implemented as a **modular monolith**:

- A single deployable .NET 9 Web API.
- Clear internal separation into `Domain`, `Application`, `Infrastructure`, and `Api` layers.
- A separate React + TypeScript client in `src/client`.

This keeps local development and deployment simple (one backend, one frontend), while still making it easy to explain how the system could be split into microservices later (for example, by extracting `Tasks` into its own service with its own database).

#### 1.2 Backend layers

- **Domain layer (`AtomicTasks.Domain`)**
  - Contains the `Task` entity and core rules.
  - Final shape aligned strictly to the assessment spec, with optional bonus fields:
    - Required by spec:
      - `int Id`
      - `bool IsCompleted` (default `false`)
      - `string Title` (required, max length **100**)
      - `string? Description`
      - `DateTime? DueDate`
    - Bonus / extended model:
      - `DateTime CreatedAt`
      - `DateTime UpdatedAt`
      - `string? CreatedBy`
      - `string? AssignedTo`

- **Application layer (`AtomicTasks.Application`)**
  - Contains the use‑cases and orchestration logic.
  - Exposes interfaces and DTOs to keep the Web API and persistence concerns decoupled:
    - `ITaskService` – service interface used by the API layer.
    - `ITaskRepository` – abstraction over persistence.
    - DTOs and request models: `TaskDto`, `CreateTaskRequest`, `UpdateTaskRequest`.
  - Business logic includes:
    - Trimming and validating titles (non‑empty, ≤ 100 chars).
    - Mapping between domain entities and DTOs.
    - Passing filter/sort/pagination parameters down to the repository.

- **Infrastructure layer (`AtomicTasks.Infrastructure`)**
  - Provides an `AtomicTasksDbContext` configured for **SQLite**.
  - Implements `ITaskRepository` via `EfTaskRepository`, including:
    - Querying by completion state.
    - Optional due‑date range.
    - Optional search term across title/description.
    - Sorting (created date, due date, title) and simple pagination.
  - Uses `Database.EnsureCreated()` at startup to simplify local setup and seeds a small set of demo tasks for easier testing.

- **API layer (`AtomicTasks.Api`)**
  - ASP.NET Core minimal API hosting the HTTP endpoints under `/api/tasks`.
  - Depends on the `ITaskService` abstraction rather than talking directly to EF Core.
  - Configures DI, DbContext, CORS, and JSON options.
  - Implements endpoints:
    - `GET /api/tasks` with optional filters (`isCompleted`, `dueFrom`, `dueTo`, `createdBy`, `assignedTo`), sorting (`sortBy`, `sortDirection`), and pagination (`page`, `pageSize`).
    - `GET /api/tasks/{id}`
    - `POST /api/tasks`
    - `PUT /api/tasks/{id}`
    - `DELETE /api/tasks/{id}`
  - Returns appropriate HTTP status codes for validation failures and not‑found cases.

### 2. Frontend approach

#### 2.1 Stack and structure

- **React 18 + TypeScript**, scaffolded via Vite in `src/client`.
- A single main entry point at `src/client/src/main.ts`.
- Styling in `src/client/src/style.css`, kept lightweight but clean and modern.

#### 2.2 Routing and views

The brief called for distinct views and the ability to navigate between them. To keep dependencies minimal while satisfying the requirement, a simple **custom router** was implemented using the browser history API:

- Supported routes:
  - `/tasks` – main task list.
  - `/tasks/new` – add new task.
  - `/tasks/{id}` – edit existing task.
- The router is a small helper (`parseRoute`) that inspects `window.location.pathname` and drives a `Route` discriminated union in React state.

Views are rendered conditionally based on the current `route`:

- **List view**:
  - Table of tasks with columns: title, status, due date, created date, created by, assigned to, and actions.
  - Buttons for toggle completion, edit, and delete.
  - Filter controls for completion state, due date range, and the extended user metadata (CreatedBy/AssignedTo) via dropdowns populated from the API.
  - Sorting controls (by created date, due date, and title; ascending/descending).
  - Pagination controls (Previous / Next) backed by the API’s pagination support.
- **Add / edit view**:
  - Single form with:
    - Title (required, validated, max length 100).
    - Due date (optional).
    - Description (optional, textarea).
    - Created by (optional, free text).
    - Assigned to (optional, free text).
  - Shows inline validation errors (e.g. “Title is required”).
  - Reuses the same component for both “add” and “edit” flows, with state initialised from the selected task when editing.

#### 2.3 API integration

The frontend talks to the backend via the REST API using `fetch`:

- Base URL is configured via `VITE_API_BASE_URL` (with `/api` as a default fallback).
- All calls use JSON and align with the DTOs exposed by the backend.
- The list view builds up a query string from the current filters, sorting, and pagination state and calls `GET /tasks` accordingly.

### 3. Alignment with the assessment spec

The brief specified a concrete data model and behaviours. Key alignment points:

- **Data model**:
  - `Task` uses `int Id`, `bool IsCompleted`, `string Title` (max 100), optional `Description`, optional `DueDate`.
  - Both backend and frontend use the same shape; type mismatches from earlier iterations (e.g. GUID IDs, enums for status/priority) were removed and corrected.
  - Extended fields (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `AssignedTo`) are treated as bonus metadata.

- **API surface and filtering/sorting**:
  - CRUD endpoints are implemented as required.
  - `GET /api/tasks` supports server‑side filtering (completion, due date range, CreatedBy, AssignedTo), search, sorting, and pagination, which the UI surfaces via controls.

- **Service layer and tests**:
  - Explicit `TaskService` and `ITaskService` interface in the Application layer.
  - API endpoints depend on the service, not directly on repositories.
  - Unit tests with Moq validate key service behaviours.

- **Frontend UX**:
  - Separate views for listing, adding, and editing tasks.
  - Completion toggling and deletion supported with confirmations and error handling.
  - Clean, responsive layout with clear labels, state pills (Pending / Completed), and helpful error messages.

Any deviations identified during development (such as initial enum‑based status/priority fields or GUID IDs) were treated as bugs relative to the brief and corrected as a priority.

### 4. Testing strategy

#### 4.1 Backend tests

Tests live under `tests/server/AtomicTasks.Tests` and use **xUnit**:

- **Repository tests**:
  - Exercise `EfTaskRepository` with the in‑memory EF Core provider.
  - Verify that tasks can be added and queried back correctly.
- **Service tests**:
  - Use **Moq** to mock `ITaskRepository`.
  - Verify that `TaskService` correctly enforces title rules, default values, and maps between DTOs and entities.

These tests demonstrate the layering (Application vs Infrastructure) and validate behaviour without needing to spin up the full API.

#### 4.2 Frontend unit tests

Frontend unit tests use **Vitest** and **React Testing Library**:

- Located in `src/client/src/App.test.tsx`.
- Cover:
  - Rendering of the main “Atomic Tasks” header.
  - Validation behaviour when submitting a task without a title.
  - Rendering of tasks returned from a mocked API response (including Pending status).

The tests mock `fetch` to keep them fast and deterministic.

#### 4.3 Frontend end-to-end tests

End‑to‑end tests use **Playwright** and live under `src/client/tests/e2e`:

- Configured via `src/client/playwright.config.ts`.
- Key flows covered:
  - Creating a new task via the UI and verifying it appears in the list.
  - Toggling completion for a specific task (creating it first if necessary).

These tests talk to the real API and frontend dev server, providing confidence that the main user journeys work as expected.

### 5. Scaling, performance, and architecture considerations

Although the submitted solution is a single modular monolith, it was designed so that it could scale out relatively easily:

- **Scaling the API**:
  - The API is stateless; all state lives in the database, so horizontal scaling (multiple instances behind a load balancer) is straightforward.
  - In a cloud environment (e.g. Azure App Service), you would simply increase instance count or use autoscale rules based on CPU/latency.
- **Scaling the database**:
  - For the assessment, SQLite keeps local setup trivial.
  - In production, this would map neatly to a managed relational database (Azure SQL / PostgreSQL) with:
    - Proper indexing on `IsCompleted`, `DueDate`, `CreatedAt`, and `Title` (for search/sorting).
    - Connection pooling and potentially read replicas for heavy read workloads.
- **Likely bottlenecks and how to address them**:
  - For very large task lists, the main bottleneck would be the database scan and network transfer.
  - The implementation already uses **server-side pagination** and filtering to avoid returning unbounded result sets.
  - If queries started to dominate, you would:
    - Add or tune indexes for common filter/sort combinations.
    - Consider caching frequent query results at the API layer (memory cache / Redis).
    - Introduce background jobs for heavy operations instead of synchronous requests.
- **N+1 queries**:
  - With the current model there are no navigation collections, so there is no N+1 issue in the usual ORM sense.
  - If `Task` were later related to Users or Projects, the repository would explicitly use `Include`/`ThenInclude` or projection DTOs to avoid N+1 patterns.
- **Caching and indexing ideas**:
  - Indexes: `(IsCompleted, DueDate)`, `(CreatedAt)`, and perhaps a functional index for lowercase `Title` search.
  - Caching: short-lived (e.g. 30–60s) cached responses for common filter presets or dashboard views, invalidated on writes.
  - At the client level, a more advanced state management layer (see below) could cache recent API responses and avoid refetching on simple navigations.

### 6. Frontend state management options

For the assessment, React’s built-in state (`useState`/`useEffect`) is used, which is perfectly adequate for a single-page tasks screen. If the application grew, a more robust state story would be appropriate:

- **Redux Toolkit**:
  - Centralised store for tasks, filters, and UI state.
  - RTK Query could manage data fetching, caching, and invalidation for the `/tasks` endpoints.
- **React Context + reducer pattern**:
  - Lightweight alternative to Redux for a medium-sized app.
  - A `TasksProvider` could expose task state and actions (`loadTasks`, `createTask`, `updateTask`, `deleteTask`) to any component.
- **Service-based abstraction**:
  - A dedicated `taskApi` module encapsulating all HTTP calls and data mapping.
  - UI components depend on the service instead of `fetch` directly, which makes it easier to swap transport or add cross-cutting features (logging, retry, backoff).

These options were consciously not introduced to keep the codebase lean for the assessment, but the current structure would allow them to be layered in without rewriting the core features.

### 7. CI and quality gates (if this were extended)

No CI pipeline is included in the repo, but the structure is designed to be CI-friendly. A simple GitHub Actions pipeline might:

- Restore and build the .NET solution.
- Run `dotnet test` on `AtomicTasks.sln`.
- Set up Node + PNPM/NPM, run `npm install` in `src/client`, then `npm test`.
- Optionally run Playwright E2E tests against a test deployment or a composed environment.
- Enforce formatting and linting (e.g. `dotnet format`, ESLint/Prettier) as additional quality gates.

This would provide quick feedback on every push and help keep the main branch always releasable.

### 8. Use of AI tools (Cursor + ChatGPT‑5)

The assessment explicitly allowed (and asked about) the use of AI tools. This section documents how they were used.

- **Planning and breakdown**:
  - AI was used to read and summarise the assessment PDFs and derive a clear, structured TODO list.
  - The TODO list was refined iteratively as gaps were discovered (for example, the need for an explicit service layer and frontend E2E tests).

- **Architecture discussions**:
  - Several options were discussed, including microservices vs modular monolith.
  - The final choice of a modular monolith with clear boundaries was taken to balance simplicity and realism.

- **Code scaffolding and refactoring**:
  - Boilerplate code was sometimes generated with AI assistance (e.g. DTOs, mapping functions, initial EF Core setup).
  - Refactorings were suggested when the implementation drifted from the spec (e.g. removing enums in favour of `IsCompleted`, switching ID types to `int`, enforcing title length).

- **Debugging and error resolution**:
  - AI was used to diagnose build and runtime issues, including:
    - EF Core version mismatches with .NET 9.
    - Windows “Access is denied” errors when launching the API (resolved via `<UseAppHost>false>`).
    - JSON serialisation issues and TypeScript compile errors during model changes.

- **Documentation and polish**:
  - Portions of this `README.md` were drafted and then edited with AI help.
  - AI was treated as a collaborator for wording and structure, not as an authority on requirements.

All significant design decisions, code changes, and final verification were performed and signed off manually. AI output was always treated as a suggestion to be checked against the written brief and actual behaviour.

### 9. High-level implementation plan / TODO

This plan was used to drive the work and is preserved here to show how the assessment was approached.

1. **Clarify requirements**
   - Extract assessment requirements and constraints (features, tech constraints, AI usage rules, submission format).
2. **Choose stack & layout**
   - Decide on React + TypeScript frontend, .NET 9 API backend, SQLite database, and the project structure above.
3. **Design architecture**
   - Finalise the modular monolith architecture with clear `Domain`, `Application`, `Infrastructure`, and `Api` boundaries.
   - Explain how it could evolve into microservices if needed.
4. **Data model & API surface**
   - Define the `Task` domain model and REST API endpoints.
   - Later, refactor to ensure perfect alignment with the assessment spec (int IDs, `IsCompleted` boolean, title max 100, optional description/due date).
5. **Backend core implementation**
   - Implement task CRUD, business rules, and persistence with SQLite.
   - Add basic validation and error handling for invalid requests.
6. **Frontend core implementation**
   - Build the React + TypeScript UI:
     - Task list.
     - Create/edit forms.
     - Completion toggle and delete.
     - Loading/error states.
7. **Routing and enhanced list behaviour**
   - Introduce client‑side routing for separate list/add/edit views.
   - Implement list filters, sorting, and pagination wired to matching API query parameters.
8. **Testing**
   - Add backend unit and integration tests (mappings, repository, service with mocks).
   - Add frontend unit tests for key flows.
   - Add Playwright end‑to‑end tests for core user journeys.
9. **Azure notes and submission polish**
   - Outline a simple Azure deployment path (App Service + managed database).
   - Finalise the documentation so reviewers can quickly run, test, and understand the solution.


