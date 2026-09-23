# Architecture

The solution follows clean architecture principles.

## Layers

### Domain
Contains entities and repository/service interfaces.

### Application
Contains DTOs, AutoMapper profiles, and application services.

### Infrastructure
Contains EF Core DbContext, entity configurations, repository implementations, and dependency injection registrations.

### Web
Contains the Razor Pages UI, static assets, `Program.cs`, and ASP.NET Core configuration.
