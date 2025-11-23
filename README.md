## Atomic Tasks – Full-Stack Task Management Application

This repository contains a small full‑stack task management application implemented for a technical assessment.
This `README` is focused on **how to set up, run, and test the application end‑to‑end**.  
For a detailed description of the development process, AI usage, and design reasoning, see `METHOD.txt`.

## Tech stack (overview)

- **Backend**: .NET 9 (ASP.NET Core) REST API
- **Frontend**: React with TypeScript (Vite)
- **Database**: SQLite (via Entity Framework Core)

## Project layout (quick reference)

```text
.
├─ README.md                      # Setup, run, and test guide (this file)
├─ METHOD.txt                     # Development method, TODOs, AI usage, reasoning
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
- `METHOD.txt` documents how the assessment was approached (including TODOs, iterations, and AI involvement) and can be used as a narrative of the development process.
- For deployment considerations and Azure‑focused notes, see the relevant section in `METHOD.txt`.

