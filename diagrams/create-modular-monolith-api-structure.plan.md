<!-- 6e337ced-7217-475c-b43b-4f6f60296ca3 267efd37-7f02-43fa-8c1a-e636af79ce5f -->
# Create Modular Monolith API Structure

## Overview

Create a .NET 8 Web API project structure following Clean Architecture principles with three modules: Orders, Tracking, and AiAnalyzer. Each module will be independently structured to allow future extraction as microservices.

## Folder Structure

```
api/
├── LogisticsTracker.Api/                      # Main API host (entry point)
│   ├── Program.cs
│   ├── LogisticsTracker.Api.csproj
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Properties/
│       └── launchSettings.json
│
├── Modules/
│   ├── Orders/
│   │   ├── Orders.Api/                        # API layer (controllers, endpoints)
│   │   │   ├── Controllers/
│   │   │   │   └── OrdersController.cs
│   │   │   ├── Orders.Api.csproj
│   │   │   └── OrdersApiExtensions.cs         # Module registration extension
│   │   │
│   │   ├── Orders.Application/                # Application layer (use cases, DTOs)
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   ├── DTOs/
│   │   │   ├── Interfaces/
│   │   │   ├── Orders.Application.csproj
│   │   │   └── DependencyInjection.cs
│   │   │
│   │   ├── Orders.Domain/                     # Domain layer (entities, value objects, events)
│   │   │   ├── Entities/
│   │   │   ├── ValueObjects/
│   │   │   ├── Events/
│   │   │   ├── Interfaces/
│   │   │   └── Orders.Domain.csproj
│   │   │
│   │   └── Orders.Infrastructure/             # Infrastructure layer (EF Core, repositories)
│   │       ├── Persistence/
│   │       │   ├── OrdersDbContext.cs
│   │       │   ├── Repositories/
│   │       │   └── Configurations/
│   │       ├── Orders.Infrastructure.csproj
│   │       └── DependencyInjection.cs
│   │
│   ├── Tracking/                              # Same structure as Orders
│   │   ├── Tracking.Api/
│   │   ├── Tracking.Application/
│   │   ├── Tracking.Domain/
│   │   └── Tracking.Infrastructure/
│   │
│   └── AiAnalyzer/                            # Same structure as Orders
│       ├── AiAnalyzer.Api/
│       ├── AiAnalyzer.Application/
│       ├── AiAnalyzer.Domain/
│       └── AiAnalyzer.Infrastructure/
│
└── Shared/                                    # Shared code (minimal, clearly separated)
    ├── Shared.Domain/                         # Shared domain concepts
    │   └── Shared.Domain.csproj
    ├── Shared.Infrastructure/                 # Shared infrastructure (logging, messaging abstractions)
    │   ├── Messaging/                         # Event bus abstractions for future microservices
    │   ├── Logging/
    │   └── Shared.Infrastructure.csproj
    └── Shared.Api/                            # Shared API concerns (middleware, filters)
        ├── Middleware/
        ├── Filters/
        └── Shared.Api.csproj
```

## Implementation Steps

1. **Create solution and main API project**

   - Create `LogisticsTracker.sln` solution file
   - Create `api/LogisticsTracker.Api` Web API project (.NET 8)
   - Configure minimal API or controllers pattern

2. **Create Orders module structure**

   - Create four projects: Orders.Api, Orders.Application, Orders.Domain, Orders.Infrastructure
   - Set up project references (Api → Application → Domain, Infrastructure → Application → Domain)
   - Create basic folder structure for each layer
   - Add `OrdersApiExtensions.cs` for module registration

3. **Create Tracking module structure**

   - Replicate Orders module structure for Tracking
   - Create Tracking.Api, Tracking.Application, Tracking.Domain, Tracking.Infrastructure projects

4. **Create AiAnalyzer module structure**

   - Replicate Orders module structure for AiAnalyzer
   - Create AiAnalyzer.Api, AiAnalyzer.Application, AiAnalyzer.Domain, AiAnalyzer.Infrastructure projects

5. **Create Shared projects**

   - Create Shared.Domain, Shared.Infrastructure, Shared.Api projects
   - Keep minimal - only truly shared concerns

6. **Configure module registration pattern**

   - Each module's Api project exposes an extension method (e.g., `AddOrdersModule`)
   - Main API's Program.cs registers all modules
   - Each module manages its own dependencies

7. **Set up database context per module**

   - Each Infrastructure project will have its own DbContext
   - Configure PostgreSQL with separate schemas (orders_schema, tracking_schema, aianalyzer_schema)
   - Use Entity Framework Core 8

8. **Add solution file references**

   - Add all projects to the solution
   - Configure proper project dependencies

## Key Design Principles

- **Module Independence**: Each module is self-contained with clear boundaries
- **Clean Architecture**: Domain → Application → Infrastructure/API dependency flow
- **Future Microservices Ready**: Modules communicate via interfaces/events, not direct dependencies
- **Database Per Module**: Each module has its own DbContext (can share database with schemas now, separate later)
- **Minimal Shared Code**: Shared projects only for truly common concerns

## Files to Create

- Solution file: `api/LogisticsTracker.sln`
- Main API: `api/LogisticsTracker.Api/` with Program.cs and project file
- 12 module projects (4 per module × 3 modules)
- 3 shared projects
- Module registration extension methods
- Basic DbContext classes per module
- Project reference configurations

### To-dos

- [ ] Create LogisticsTracker.sln solution file and main LogisticsTracker.Api project (.NET 8 Web API)
- [ ] Create Orders module with 4 projects (Api, Application, Domain, Infrastructure) and folder structure
- [ ] Create Tracking module with 4 projects (Api, Application, Domain, Infrastructure) and folder structure
- [ ] Create AiAnalyzer module with 4 projects (Api, Application, Domain, Infrastructure) and folder structure
- [ ] Create Shared projects (Domain, Infrastructure, Api) for common concerns
- [ ] Create module registration extension methods and configure Program.cs to register all modules
- [ ] Create DbContext classes for each module with PostgreSQL schema configuration
- [ ] Set up all project references and add projects to solution file