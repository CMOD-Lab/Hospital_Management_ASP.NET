# Clinic Management System - .NET 8 Migration

## Overview
This is a Clinic Management System migrated from ASP.NET Web Forms (.NET 4.5.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
ClinicManagement/
├── src/
│   ├── ClinicManagement.Domain/          # Domain entities, interfaces
│   ├── ClinicManagement.Application/     # Business logic, services, DTOs
│   ├── ClinicManagement.Infrastructure/  # EF Core, repositories
│   └── ClinicManagement.Web/             # Razor Pages UI
├── tests/
│   └── ClinicManagement.UnitTests/       # Unit tests
└── docs/                                 # Documentation
```

## Features
- **Patient Portal**: Registration, appointment booking, bill history, treatment history, notifications, feedback
- **Doctor Portal**: Profile, pending appointments, history update, billing, patient history
- **Admin Panel**: Dashboard, doctor registration, staff management, clinic management

## Setup

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)

### Configuration
Update the connection string in `src/ClinicManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=DBProject;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

### Build
```bash
dotnet restore
dotnet build
```

### Run
```bash
dotnet run --project src/ClinicManagement.Web
```

### Tests
```bash
dotnet test tests/ClinicManagement.UnitTests
```

## Migration Notes
- Migrated from ASP.NET Web Forms to Razor Pages
- Replaced ADO.NET with Entity Framework Core 8.0.0
- Replaced System.Web with ASP.NET Core equivalents
- Replaced Web.config with appsettings.json
- Replaced Global.asax with Program.cs
- Added Serilog for structured logging
- Implemented clean architecture with proper separation of concerns
- Added dependency injection throughout
- Implemented async/await patterns
