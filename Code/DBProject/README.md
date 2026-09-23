# Clinic Management System

This solution contains a .NET 8 migration of the legacy ASP.NET Web Forms clinic management application.

## Architecture

- `src/ClinicManagementSystem.Domain`: entities and contracts
- `src/ClinicManagementSystem.Application`: DTOs, mappings, services
- `src/ClinicManagementSystem.Infrastructure`: EF Core data access and repository implementations
- `DBProject`: Razor Pages web application
- `tests`: unit and integration tests

## Running

1. Install .NET 8 SDK.
2. From `Code`, run `dotnet restore`.
3. Run `dotnet build`.
4. Run `dotnet run --project DBProject`.

## Migration Notes

The old Web Forms pages, master pages, packages.config, and Web.config are replaced with ASP.NET Core Razor Pages, appsettings.json, Program.cs, EF Core repositories, and ASP.NET Core Identity.
