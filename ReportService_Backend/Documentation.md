# Report Service Backend Documentation

## Table of Contents
1. [Architecture Overview](#1-architecture-overview)
2. [Implementation Details](#2-implementation-details)
3. [Code Structure](#3-code-structure)
4. [Database Schema](#4-database-schema)
5. [API Endpoints](#5-api-endpoints)
6. [Service Layer](#6-service-layer)
7. [Configuration](#7-configuration)
8. [Deployment Guide](#8-deployment-guide)

## 1. Architecture Overview

### 1.1 Project Structure
```
ReportService_Backend/
├── ReportService.API/         # API Controllers and endpoints
├── ReportService.Business/    # Business logic and services
├── ReportService.Domain/      # Entities and DTOs
├── ReportService.Data/        # Data access and repositories
└── ReportService.Infrastructure/ # Cross-cutting concerns
```

### 1.2 Technology Stack
- .NET Core 6.0
- PostgreSQL Database
- Entity Framework Core
- OpenAI Integration
- AutoMapper
- Swagger/OpenAPI

## 2. Implementation Details

### 2.1 Domain Entities

#### Report Entity
```csharp
public class Report
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Query { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string CreatedBy { get; set; }
    public Guid ChatSessionId { get; set; }

    // Navigation properties
    public virtual ChatSession ChatSession { get; set; }
    public virtual ChartConfiguration ChartConfig { get; set; }
}
```

#### ChartConfiguration Entity
```csharp
public class ChartConfiguration
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public string Type { get; set; }
    public string XAxisField { get; set; }
    public string YAxisField { get; set; }
    public string SeriesField { get; set; }
    public string SizeField { get; set; }
    public string ColorField { get; set; }
    public string OptionsJson { get; set; }
    public string FiltersJson { get; set; }

    // Navigation property
    public virtual Report Report { get; set; }
}
```

#### ChatSession Entity
```csharp
public class ChatSession
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual ICollection<ChatMessage> Messages { get; set; }
    public virtual ICollection<Report> Reports { get; set; }
}
```

#### ChatMessage Entity
```csharp
public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid ChatSessionId { get; set; }
    public string Message { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public string GeneratedSql { get; set; }

    // Navigation property
    public virtual ChatSession ChatSession { get; set; }
}
```

### 2.2 DTOs

#### ReportDto
```csharp
public class ReportDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Query { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string CreatedBy { get; set; }
    public Guid ChatSessionId { get; set; }
    public ChartConfigurationDto ChartConfig { get; set; }
}

public class CreateReportRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Query { get; set; }
    public Guid ChatSessionId { get; set; }
    public ChartConfigurationDto ChartConfig { get; set; }
}

public class UpdateReportRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Query { get; set; }
    public ChartConfigurationDto ChartConfig { get; set; }
}
```

## 3. Code Structure

### 3.1 Service Interfaces

#### IReportService
```csharp
public interface IReportService
{
    Task<IEnumerable<ReportDto>> GetAllReportsAsync();
    Task<ReportDto> GetReportByIdAsync(int id);
    Task<ReportDto> CreateReportAsync(CreateReportDto createReportDto);
    Task<ReportDto> UpdateReportAsync(int id, UpdateReportDto updateReportDto);
    Task<bool> DeleteReportAsync(int id);
}
```

#### IQueryManagementService
```csharp
public interface IQueryManagementService
{
    Task<QueryResultDto> ExecuteQueryAsync(string query);
    Task<QueryValidationDto> ValidateQueryAsync(string query);
    Task<QueryMetadataDto> GetQueryMetadataAsync(string query);
    Task<string> TranslateNaturalLanguageToSqlAsync(string naturalLanguageQuery);
    Task<ReportDto> SaveQueryAsync(SaveQueryRequestDto request);
    Task<ReportDto> UpdateQueryAsync(Guid queryId, UpdateQueryRequestDto request);
    Task<List<ReportDto>> GetQueriesByChatSessionAsync(Guid chatSessionId);
    Task<ChartConfigurationDto> SuggestChartConfigurationAsync(string query, QueryMetadataDto metadata);
    Task<object> ExecuteQueryWithFiltersAsync(string query, Dictionary<string, object> filters);
    Task<string> GetDatabaseSchemaAsync();
}
```

### 3.2 Service Implementations

#### ReportService Implementation
```csharp
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IMapper _mapper;

    public ReportService(IReportRepository reportRepository, IMapper mapper)
    {
        _reportRepository = reportRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReportDto>> GetAllReportsAsync()
    {
        var reports = await _reportRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReportDto>>(reports);
    }

    public async Task<ReportDto> GetReportByIdAsync(int id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        return _mapper.Map<ReportDto>(report);
    }

    public async Task<ReportDto> CreateReportAsync(CreateReportDto createReportDto)
    {
        var report = _mapper.Map<Report>(createReportDto);
        report.CreatedDate = DateTime.UtcNow;
        report.CreatedBy = "System"; // This should be replaced with actual user info
        report.Status = "Active";

        var createdReport = await _reportRepository.CreateAsync(report);
        return _mapper.Map<ReportDto>(createdReport);
    }

    public async Task<ReportDto> UpdateReportAsync(int id, UpdateReportDto updateReportDto)
    {
        var existingReport = await _reportRepository.GetByIdAsync(id);
        if (existingReport == null)
            return null;

        _mapper.Map(updateReportDto, existingReport);
        var updatedReport = await _reportRepository.UpdateAsync(existingReport);
        return _mapper.Map<ReportDto>(updatedReport);
    }

    public async Task<bool> DeleteReportAsync(int id)
    {
        return await _reportRepository.DeleteAsync(id);
    }
}
```

## 4. Database Schema

### 4.1 Entity Framework Configuration
```csharp
public class ReportDbContext : DbContext
{
    public DbSet<Report> Reports { get; set; }
    public DbSet<ChartConfiguration> ChartConfigurations { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Report Configuration
        modelBuilder.Entity<Report>()
            .HasOne(r => r.ChartConfig)
            .WithOne(c => c.Report)
            .HasForeignKey<ChartConfiguration>(c => c.ReportId);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.ChatSession)
            .WithMany(s => s.Reports)
            .HasForeignKey(r => r.ChatSessionId);

        // ChatMessage Configuration
        modelBuilder.Entity<ChatMessage>()
            .HasOne(m => m.ChatSession)
            .WithMany(s => s.Messages)
            .HasForeignKey(m => m.ChatSessionId);
    }
}
```

## 5. API Endpoints

### 5.1 Reports Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportDto>>> GetAll()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(reports);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReportDto>> GetById(int id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null)
            return NotFound();
        return Ok(report);
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> Create([FromBody] CreateReportDto createReportDto)
    {
        var report = await _reportService.CreateReportAsync(createReportDto);
        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReportDto>> Update(int id, [FromBody] UpdateReportDto updateReportDto)
    {
        var report = await _reportService.UpdateReportAsync(id, updateReportDto);
        if (report == null)
            return NotFound();
        return Ok(report);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _reportService.DeleteReportAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
```

## 6. Service Layer

### 6.1 OpenAI Integration
```csharp
public class OpenAIService : ISqlGenerationService
{
    private readonly IOpenAIService _openAI;
    private readonly string _model = Models.Gpt_4;

    public OpenAIService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentNullException(nameof(apiKey), "OpenAI API key is not configured");

        _openAI = new OpenAIService(new OpenAiOptions { ApiKey = apiKey });
    }

    public async Task<string> GenerateSqlQueryAsync(string userPrompt, string databaseSchema)
    {
        var completionResult = await _openAI.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem($"You are a SQL expert. Generate SQL queries based on the following schema:\n{databaseSchema}"),
                ChatMessage.FromUser(userPrompt)
            },
            Model = _model,
            Temperature = 0.3f
        });

        return completionResult.Choices[0].Message.Content;
    }
}
```

## 7. Configuration

### 7.1 Application Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=reportservice;Username=your_username;Password=your_password"
  },
  "OpenAI": {
    "ApiKey": "your_api_key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "Origins": [
      "http://localhost:4201",
      "http://localhost:4202"
    ]
  }
}
```

### 7.2 Service Registration
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ReportDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Repositories
        services.AddScoped<IReportRepository, ReportRepository>();

        // Services
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IQueryManagementService, QueryManagementService>();
        services.AddScoped<ISqlGenerationService, OpenAIService>();

        return services;
    }
}
```

## 8. Deployment Guide

### 8.1 Prerequisites
- .NET 6.0 SDK
- PostgreSQL Database
- OpenAI API Key
- SSL Certificate (for production)

### 8.2 Deployment Steps
1. Update configuration in appsettings.json
2. Run database migrations:
   ```bash
   dotnet ef database update
   ```
3. Build the application:
   ```bash
   dotnet build
   dotnet publish -c Release
   ```
4. Deploy to your hosting environment
5. Configure environment variables
6. Set up SSL certificate
7. Configure reverse proxy (if needed)

### 8.3 Environment Variables
- `ConnectionStrings__DefaultConnection`
- `OpenAI__ApiKey`
- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`

### 8.4 Health Checks
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ... other middleware

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/health");
        endpoints.MapControllers();
    });
}
```

---

## Additional Resources

### API Documentation
- Swagger UI: `/swagger`
- API Documentation: `/api-docs`

### Monitoring
- Health Check: `/health`
- Metrics: `/metrics`

### Support
For technical support or questions, contact:
- Email: support@yourcompany.com
- Documentation: https://docs.yourcompany.com 