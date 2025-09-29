# eShop C# Development Instructions

## Project Overview
This is a **microservices e-commerce application** built with **.NET 9 and .NET Aspire**. When working with C# files in this project, follow these architectural patterns and conventions.

## Core C# Patterns & Conventions

### 1. Global Usings Pattern
Every C# project has a `GlobalUsings.cs` file that contains all common using statements. This eliminates the need for repetitive using statements in individual files.

**Best Practice:**
- Add new common usings to `GlobalUsings.cs` instead of individual files
- Keep project-specific usings in `GlobalUsings.cs`
- Only add file-specific usings when they're unique to that file

**Example GlobalUsings.cs pattern:**
```csharp
global using Microsoft.EntityFrameworkCore;
global using eShop.ServiceDefaults;
global using eShop.EventBus.Abstractions;
global using eShop.EventBus.Events;
```

### 2. Service Registration Pattern (Extensions.cs)
Every service project has an `Extensions.cs` file containing an `AddApplicationServices` extension method for `IHostApplicationBuilder`.

**Required Structure:**
```csharp
public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // Database context registration
        builder.AddNpgsqlDbContext<YourContext>("connectionStringName");

        // Service registration
        builder.Services.AddTransient<IYourService, YourService>();

        // Event bus registration
        builder.AddRabbitMqEventBus("eventbus")
               .AddSubscription<YourEvent, YourEventHandler>();

        // Authentication (if needed)
        builder.AddDefaultAuthentication();
    }
}
```

### 3. Program.cs Minimal Pattern
All API services follow a minimal Program.cs pattern:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Always call these in order:
builder.AddServiceDefaults(); // or AddBasicServiceDefaults() for gRPC
builder.AddApplicationServices();
builder.Services.AddProblemDetails(); // For REST APIs

// API versioning (for REST APIs)
var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.MapDefaultEndpoints();

// Map your APIs
app.MapYourApi();

// For REST APIs only
app.UseDefaultOpenApi();
app.Run();
```

### 4. Minimal API Organization
API endpoints are organized in dedicated static classes with extension methods:

```csharp
public static class YourApi
{
    public static RouteGroupBuilder MapYourApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/your-resource");

        api.MapGet("/", GetAllAsync);
        api.MapGet("/{id}", GetByIdAsync);
        api.MapPost("/", CreateAsync);

        return api;
    }

    // Endpoint handlers as private static methods
    private static async Task<Results<Ok<YourDto[]>, NotFound>> GetAllAsync(
        [AsParameters] YourService service)
    {
        // Implementation
    }
}
```

### 5. Event-Driven Communication Pattern

**Integration Events:**
```csharp
// Event definition
public record YourIntegrationEvent(int Id, string Data) : IntegrationEvent;

// Event handler
public class YourIntegrationEventHandler : IIntegrationEventHandler<YourIntegrationEvent>
{
    public async Task Handle(YourIntegrationEvent @event)
    {
        // Handle the event
    }
}

// Publishing events with transaction safety
await integrationEventService.SaveEventAndDbContextChangesAsync(@event);
await integrationEventService.PublishThroughEventBusAsync(@event);
```

### 6. Entity Framework Patterns

**Context Registration:**
```csharp
// In Extensions.cs
builder.AddNpgsqlDbContext<YourContext>("connectionStringName",
    configureDbContextOptions: dbContextOptionsBuilder =>
    {
        dbContextOptionsBuilder.UseNpgsql(builder =>
        {
            builder.UseVector(); // If using pgvector
        });
    });

// Add migrations
builder.Services.AddMigration<YourContext, YourContextSeed>();
```

**Entity Configuration:**
```csharp
public class YourEntityTypeConfiguration : IEntityTypeConfiguration<YourEntity>
{
    public void Configure(EntityTypeBuilder<YourEntity> builder)
    {
        builder.ToTable("YourEntities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
    }
}
```

### 7. Domain-Driven Design Patterns (Ordering Service)

**Entities:**
```csharp
public class YourEntity : Entity, IAggregateRoot
{
    public YourEntity(/* parameters */)
    {
        // Domain logic in constructor
    }

    public void YourBusinessMethod()
    {
        // Domain logic
        AddDomainEvent(new YourDomainEvent(/* parameters */));
    }
}
```

**Value Objects:**
```csharp
public class YourValueObject : ValueObject
{
    public string Value { get; private init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### 8. CQRS with MediatR (Ordering Service)

**Commands:**
```csharp
public record YourCommand(int Id, string Data) : IRequest<bool>;

public class YourCommandHandler : IRequestHandler<YourCommand, bool>
{
    public async Task<bool> Handle(YourCommand request, CancellationToken cancellationToken)
    {
        // Command handling logic
        return true;
    }
}
```

**Queries:**
```csharp
public record YourQuery(int Id) : IRequest<YourDto>;

public class YourQueryHandler : IRequestHandler<YourQuery, YourDto>
{
    public async Task<YourDto> Handle(YourQuery request, CancellationToken cancellationToken)
    {
        // Query handling logic
        return new YourDto();
    }
}
```

### 9. Testing Patterns

**Unit Tests:**
```csharp
[TestClass]
public class YourServiceTests
{
    [TestMethod]
    public async Task YourMethod_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var service = new YourService();

        // Act
        var result = await service.YourMethodAsync();

        // Assert
        Assert.IsNotNull(result);
    }
}
```

**Functional Tests:**
```csharp
public sealed class YourApiTests : IClassFixture<YourApiFixture>
{
    public YourApiTests(YourApiFixture fixture)
    {
        // Setup
    }

    [Fact]
    public async Task GetYourEndpoint_ReturnsOk()
    {
        // Test implementation
    }
}
```

### 10. Authentication & Authorization

**Service Registration:**
```csharp
// In Extensions.cs
builder.AddDefaultAuthentication();

// For identity service
builder.Services.AddTransient<IIdentityService, IdentityService>();
```

**Usage in APIs:**
```csharp
api.MapGet("/protected", ProtectedEndpoint)
   .RequireAuthorization();
```

### 11. gRPC Services Pattern

**Service Implementation:**
```csharp
[Authorize]
public class YourGrpcService : YourService.YourServiceBase
{
    public override async Task<YourResponse> YourMethod(
        YourRequest request,
        ServerCallContext context)
    {
        // Implementation
        return new YourResponse();
    }
}
```

**Registration:**
```csharp
// In Program.cs
app.MapGrpcService<YourGrpcService>();
```

### 12. AI Integration Patterns

**Service Registration:**
```csharp
// In Extensions.cs
if (builder.Configuration["OllamaEnabled"] is string ollamaEnabled && bool.Parse(ollamaEnabled))
{
    builder.AddOllamaApiClient("embedding")
        .AddEmbeddingGenerator();
}
else if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("textEmbeddingModel")))
{
    builder.AddOpenAIClientFromConfiguration("textEmbeddingModel")
        .AddEmbeddingGenerator();
}
```

## Coding Standards

### File Organization
- `Program.cs`: Keep minimal, delegate to Extensions.cs
- `Extensions.cs`: All service registration
- `GlobalUsings.cs`: All common using statements
- `Apis/`: Minimal API endpoint definitions
- `Services/`: Business logic services
- `Infrastructure/`: EF configurations, repositories
- `IntegrationEvents/`: Event definitions and handlers
- `Model/`: Domain entities and DTOs

### Naming Conventions
- **Interfaces**: Prefix with `I` (e.g., `ICatalogService`)
- **Integration Events**: Suffix with `IntegrationEvent`
- **Event Handlers**: Suffix with `IntegrationEventHandler`
- **APIs**: Suffix with `Api` (e.g., `CatalogApi`)
- **DTOs**: Suffix with `Dto` (e.g., `CatalogItemDto`)

### Error Handling
- Use `Results<T>` for minimal APIs
- Implement global exception handling in ServiceDefaults
- Use `ProblemDetails` for structured error responses

### Dependency Injection
- Register services in `Extensions.cs`
- Use appropriate lifetimes:
  - `AddTransient`: Stateless services
  - `AddScoped`: Per-request services (DbContext, etc.)
  - `AddSingleton`: Stateless, expensive-to-create services

### Configuration
- Use strongly-typed configuration with Options pattern
- Bind configuration in `Extensions.cs`:
  ```csharp
  builder.Services.AddOptions<YourOptions>()
      .BindConfiguration(nameof(YourOptions));
  ```

### OpenTelemetry Integration
- All services inherit OpenTelemetry configuration from ServiceDefaults
- Manual instrumentation when needed:
  ```csharp
  using var activity = ActivitySource.StartActivity("OperationName");
  ```

## AI-Specific Guidelines
- Use embeddings for semantic search in Catalog service
- Implement AI features behind feature flags
- Follow the established pattern for OpenAI/Ollama configuration
- Store embeddings in PostgreSQL with pgvector extension

## Database Guidelines
- Use PostgreSQL as primary database
- Implement migrations with seeding
- Use EF Core with proper entity configurations
- Apply migrations automatically on startup in development

Follow these patterns consistently across all C# files to maintain architectural coherence and leverage the established microservices patterns in this eShop application.