# Clinic Management System - .NET 8 Migration

## Overview
This is the migrated version of the Clinic Management System, converted from ASP.NET Web Forms 4.5.2 to .NET 8 using clean architecture principles.

## Architecture
The solution follows **Clean Architecture** with four layers:

```
ClinicManagement.sln
├── src/
│   ├── ClinicManagement.Domain          # Entities, Interfaces, Enums, Exceptions
│   ├── ClinicManagement.Application     # Services, DTOs, Business Logic
│   ├── ClinicManagement.Infrastructure  # EF Core, Repositories, Data Access
│   └── ClinicManagement.Web             # Razor Pages, ViewModels, UI
└── tests/
    └── ClinicManagement.UnitTests       # xUnit unit tests
```

## Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

## Setup

### 1. Configure Database Connection
Update `src/ClinicManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=DBProject;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

### 2. Run Database Scripts
Execute the SQL scripts in `Database Files/` folder in order:
1. `Schema.sql` - Creates tables
2. `Admin.sql` - Admin data
3. `Doctor.sql` - Doctor data
4. `Patient.sql` - Patient data
5. `Insertions.sql` - Sample data

### 3. Build and Run
```bash
cd "Hospital Mgmt"
dotnet restore ClinicManagement.sln
dotnet build ClinicManagement.sln
dotnet run --project src/ClinicManagement.Web
```

## User Roles
- **Admin** (Type=3): Manage doctors, staff, view dashboard
- **Doctor** (Type=2): Manage appointments, prescriptions, bills
- **Patient** (Type=1): Book appointments, view history, feedback

## Key Features
- Login/Sign-up for patients
- Admin dashboard with statistics
- Doctor registration and management
- Staff management
- Appointment booking and management
- Prescription management
- Bill management
- Treatment history
- Patient notifications and feedback

## Migration Notes
See `docs/MIGRATION_NOTES.md` for detailed migration information.
