# Clinic Management System - .NET 8 Migration

## Overview
This is the migrated version of the Clinic Management System, converted from ASP.NET Web Forms (.NET 4.5.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

- **ClinicManagement.Domain** - Domain entities, interfaces, enums, exceptions
- **ClinicManagement.Application** - Business logic, DTOs, AutoMapper profiles
- **ClinicManagement.Infrastructure** - Data access (ADO.NET stored procedures), service implementations
- **ClinicManagement.Web** - ASP.NET Core Razor Pages UI

## Prerequisites
- .NET 8 SDK
- SQL Server (SQLEXPRESS or full)
- The original database schema (see Database Files folder)

## Setup
1. Update the connection string in `src/ClinicManagement.Web/appsettings.json`
2. Run the database scripts from the `Database Files` folder
3. Run `dotnet restore` from the `Code` directory
4. Run `dotnet build` to verify compilation
5. Run `dotnet run --project src/ClinicManagement.Web` to start the application

## Migration Notes
- Web Forms pages (.aspx) → Razor Pages (.cshtml)
- Code-behind files (.aspx.cs) → Page Models (.cshtml.cs)
- Master pages (.master) → Layout pages (_Layout.cshtml)
- Global.asax → Program.cs
- Web.config → appsettings.json
- System.Web → ASP.NET Core equivalents
- Session state → ASP.NET Core Session middleware
- ADO.NET stored procedures preserved (no EF Core migration needed for existing DB)

## User Roles
- **Patient (type=1)**: Register, view doctors, book appointments, view history
- **Doctor (type=2)**: Manage appointments, update prescriptions, generate bills
- **Admin (type=3)**: Manage doctors, staff, view dashboard

## Build
```bash
cd "Code"
dotnet restore
dotnet build
```
