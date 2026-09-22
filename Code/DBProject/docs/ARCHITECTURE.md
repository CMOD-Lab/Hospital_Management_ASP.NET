# Architecture

## Layers
- `src/ClinicManagementSystem.Domain`: entities, repository interfaces, service interfaces.
- `src/ClinicManagementSystem.Application`: DTOs, mappings, orchestration services.
- `src/ClinicManagementSystem.Infrastructure`: EF Core context, repositories, Identity user, seed data.
- `Code/DBProject`: ASP.NET Core Razor Pages host.

## Design choices
- Dependency inversion is enforced through interfaces.
- Logging is handled with `ILogger<T>`.
- CRUD pages are implemented for doctors, patients, staff members, and appointments.
- View models are manually mapped in Razor Page handlers.
