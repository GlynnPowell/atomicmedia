## Atomic Tasks – Full-Stack Task Management Application

This repository contains a small full‑stack task management application implemented for a technical assessment.
The focus is on clean architecture, straightforward local setup, and demonstrating awareness of microservices, Docker, and Azure without over‑engineering the solution.

## Tech stack

- **Backend**: .NET 9 (ASP.NET Core) REST API
- **Frontend**: React with TypeScript
- **Database**: SQLite (via Entity Framework Core)
- **Containerisation**: Docker + docker-compose (optional, for one-command local setup)
- **Cloud target**: Azure (App Service or Container Apps + managed database), described at a high level

## Project structure (planned)

The solution is organised as a **modular monolith**: a single deployable backend with clear internal boundaries, plus a separate React client.

```text
.
├─ README.md
├─ AtomicTasks.sln                 # .NET solution (planned)
├─ docker-compose.yml              # Orchestrates API, DB, and frontend (planned)
├─ infra/                          # Optional Azure / IaC snippets (planned)
├─ src/
│  ├─ server/
│  │  ├─ AtomicTasks.Api/          # ASP.NET Core API (endpoints, startup, DI)
│  │  ├─ AtomicTasks.Application/  # Use-cases, services, DTOs, interfaces
│  │  ├─ AtomicTasks.Domain/       # Domain entities, value objects, enums, rules
│  │  └─ AtomicTasks.Infrastructure/ # EF Core, SQLite, repositories, migrations
│  └─ client/
│     └─ (React + TypeScript app)  # UI, API client, hooks, components
└─ tests/
   ├─ server/AtomicTasks.Tests/    # Domain / application / API tests
   └─ client/                      # Optional front-end tests
```

This structure keeps responsibilities separated and makes it easy to explain how the backend could later be split into independent microservices (for example, by extracting `Tasks` into its own service).

## Running the application (planned)

**Note**: The implementation is being built step by step. The commands below describe the intended developer experience and will be updated as the code is added.

- **Direct (no Docker)**:
  - **Backend**: restore and run the .NET 9 API (e.g. `dotnet restore` then `dotnet run` in `AtomicTasks.Api`).
  - **Frontend**: install dependencies and start the dev server (e.g. `npm install` then `npm run dev` in `src/client`).
- **With Docker**:
  - Use `docker-compose.yml` at the repo root to start the API, SQLite DB, and frontend with a single `docker compose up` command.

Exact commands, ports, and environment variables will be documented once the implementation is complete.

## Architecture overview

- **Domain layer (`AtomicTasks.Domain`)**: core business concepts, such as the `Task` entity, status enums, and domain rules (e.g. allowed status transitions).
- **Application layer (`AtomicTasks.Application`)**: use-cases and services (e.g. create/update/delete task, change status), expressed in terms of interfaces and DTOs.
- **Infrastructure layer (`AtomicTasks.Infrastructure`)**: EF Core DbContext configured for SQLite, repository implementations, and migrations.
- **API layer (`AtomicTasks.Api`)**: ASP.NET Core endpoints, dependency injection configuration, validation, and HTTP concerns.
- **Frontend (`src/client`)**: React + TypeScript SPA that consumes the API, providing task CRUD, filtering, and a clean, responsive UI with proper loading and error states.

This “modular monolith” layout is intentionally simple to run as a single service while still reflecting microservice-friendly boundaries.

## Deployment to Azure (outline)

While Azure deployment is not required to run the app locally, the intended approach is:

- **Compute**: Deploy the backend as a containerised app to Azure App Service or Azure Container Apps.
- **Data**: Use Azure SQL or Azure Database for PostgreSQL as the managed database analogue to local SQLite.
- **Configuration**: Store connection strings and secrets in environment variables, ideally backed by Azure Key Vault.
- **IaC (optional)**: A small Bicep or Terraform template (under `infra/`) can declare the App Service/Container App, database, and supporting resources.

The repository will include a short “how this would be deployed” note rather than a full production-grade Azure setup, to keep the assessment focused on code and architecture.

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
3. **Design architecture** – Finalise the modular monolith architecture with clear `Domain`, `Application`, `Infrastructure`, and `Api` boundaries, and explain how it could evolve into microservices. *(In progress)*
4. **Data model & API surface** – Define the `Task` domain model (entities, DTOs, enums) and REST API endpoints, including validation rules.
5. **Backend core implementation** – Implement task CRUD, business rules, persistence with SQLite, error handling, and configuration for local development.
6. **Frontend core implementation** – Build the React + TypeScript UI (task list, create/edit/delete, status changes, filtering, and UX details like loading and error states).
7. **Testing** – Add essential tests (unit tests for domain and application logic; a small number of integration/API tests).
8. **Dockerisation** – Add Dockerfiles and a `docker-compose.yml` to run API, DB, and frontend with one command.
9. **Azure notes** – Document an outline for deploying to Azure, including configuration via environment variables and Key Vault.
10. **Docs & polish** – Finalise this README with concrete run instructions, record trade-offs and future improvements, and do a final UX and code cleanup pass.

This README will be updated as each step is completed.


