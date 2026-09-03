# CareTrack - Clinic Management System

## Overview
CareTrack is a modern .NET 8 Clinic Management System migrated from ASP.NET Web Forms 4.5.2. It provides a comprehensive platform for managing clinic operations including patient registration, doctor management, appointment scheduling, billing, and more.

## Architecture
This application follows **Clean Architecture** principles with four distinct layers:

```
CareTrack/
├── src/
│   ├── CareTrack.Domain/          # Domain entities, interfaces, enums, exceptions
│   ├── CareTrack.Application/     # Business logic services, DTOs, validators
│   ├── CareTrack.Infrastructure/  # EF Core DbContext, repositories, data access
│   └── CareTrack.Web/             # ASP.NET Core Razor Pages UI
├── tests/
│   ├── CareTrack.UnitTests/       # xUnit unit tests with Moq
│   └── CareTrack.IntegrationTests/# Integration tests
└── docs/                          # Documentation
```

## Technology Stack
- **Framework**: .NET 8
- **UI**: ASP.NET Core Razor Pages
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server
- **Logging**: Serilog
- **Testing**: xUnit, Moq, FluentAssertions
- **Frontend**: Bootstrap 5, Font Awesome 6

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (Express or higher)

### Configuration
1. Update the connection string in `src/CareTrack.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=DBProject;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

2. Run the database schema scripts from `Database Files/` directory.

### Running the Application
```bash
cd src/CareTrack.Web
dotnet run
```

### Running Tests
```bash
cd tests/CareTrack.UnitTests
dotnet test
```

## User Roles
- **Admin** (Type 3): Manage doctors, staff, view dashboard
- **Doctor** (Type 2): Manage appointments, prescriptions, billing
- **Patient** (Type 1): Book appointments, view history, feedback

## Migration Notes
See `docs/MIGRATION_NOTES.md` for details on the Web Forms to .NET 8 migration.
