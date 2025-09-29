# eShop Architecture & Development Guide

## Core Architecture

This is a **microservices e-commerce application** built with **.NET 9 and .NET Aspire**. The AppHost (`src/eShop.AppHost/Program.cs`) orchestrates all services and defines their dependencies.

### Service Topology
- **Catalog.API**: Product catalog with AI-powered semantic search (pgvector)
- **Basket.API**: Shopping basket using Redis, exposes gRPC services
- **Ordering.API**: Order management with domain-driven design patterns
- **Identity.API**: Authentication using Duende IdentityServer
- **WebApp**: Blazor Server frontend consuming all APIs
- **Mobile.Bff.Shopping**: YARP reverse proxy for mobile clients
- **PaymentProcessor/OrderProcessor**: Background services handling events
- **Webhooks.API**: Event-driven webhook notifications

### Key Infrastructure
- **EventBus**: RabbitMQ-based async messaging with custom integration events
- **PostgreSQL**: Primary database with pgvector for AI embeddings
- **Redis**: Basket storage and caching
- **OpenTelemetry**: Distributed tracing across all services

## Essential Patterns

### Service Registration
All services inherit from `eShop.ServiceDefaults` which provides:
- OpenTelemetry configuration
- Health checks at `/health` and `/alive`
- Service discovery via Aspire
- Resilient HTTP clients with Polly

```csharp
builder.AddServiceDefaults(); // Standard pattern for all services
```

### Event-Driven Communication
Services communicate via **Integration Events** through RabbitMQ:

```csharp
// Register event handlers in Extensions.cs
eventBus.AddSubscription<ProductPriceChangedIntegrationEvent, ProductPriceChangedHandler>();

// Publish events with transaction safety
await integrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);
await integrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
```

### Minimal API Organization
APIs use dedicated static classes for endpoint mapping:
- `CatalogApi.MapCatalogApi()` in Catalog.API
- Versioned endpoints (v1, v2) using Asp.Versioning
- Follow pattern: `api/[service]/[resource]`

### AI Integration
- **Catalog.API**: Uses OpenAI/Ollama for product embeddings and semantic search
- **WebApp**: AI-powered chat features
- Toggle with `useOpenAI`/`useOllama` flags in AppHost Program.cs

## Development Workflows

### Running the Application
```bash
# Primary method - starts entire microservices stack
dotnet run --project src/eShop.AppHost/eShop.AppHost.csproj

# Alternative - use VS Code tasks
# Ctrl+Shift+P -> "Tasks: Run Task" -> select "build" or "watch"
```

### Testing Strategy
- **Unit Tests**: `tests/*UnitTests/` for business logic
- **Functional Tests**: `tests/*FunctionalTests/` using WebApplicationFactory with real containers
- **E2E Tests**: Playwright tests in `e2e/` directory
- **Requires Docker** for functional tests (TestContainers pattern)

### Database Patterns
- EF Core with PostgreSQL across all services
- Migrations auto-applied on startup via `MigrateDbContextExtensions`
- Shared types in `src/Shared/` folder for cross-cutting concerns

## Project-Specific Conventions

### File Organization
- **Extensions.cs**: Service registration and dependency injection per project
- **GlobalUsings.cs**: Common using statements (especially in Identity.API)
- **eShop.Web.slnf**: Solution filter excluding mobile apps and unused projects

### Authentication Flow
- All APIs authenticate via JWT from Identity.API
- Use `builder.AddDefaultAuthentication()` from ServiceDefaults
- Identity URLs configured in AppHost and passed via environment variables

### Configuration Management
- `Directory.Build.props`: Global MSBuild properties
- `Directory.Packages.props`: Centralized NuGet package versions
- AppHost manages connection strings and service discovery

### AI/OpenAI Usage
When working with AI features:
- Check `useOpenAI` flag in AppHost Program.cs
- Embeddings stored in PostgreSQL using pgvector extension
- Configure via `AddOpenAI()` or `AddOllama()` extension methods

## Integration Points

### Event Bus Implementation
Custom event handling with guaranteed delivery:
- Events logged to database before publishing (transactional outbox pattern)
- Retry policies and dead letter handling
- OpenTelemetry tracing across event boundaries

### Cross-Service Communication
- Synchronous: Direct HTTP calls via service discovery
- Asynchronous: Integration events via EventBus
- gRPC: Basket service exposes protobuf contracts

### External Dependencies
- **Docker**: Required for local development (RabbitMQ, PostgreSQL, Redis containers)
- **Azure**: Optional deployment target with azd CLI support
- **.NET Aspire**: Development orchestration and service discovery

Always check `src/eShop.AppHost/Program.cs` to understand service dependencies and startup sequence.