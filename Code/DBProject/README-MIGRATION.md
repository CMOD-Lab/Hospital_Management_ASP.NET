# Migration Readiness Notes

This legacy module is an ASP.NET Web Forms application and cannot be upgraded to .NET 8 by changing a few source files in place.

## Why
- .NET 8 does not support Web Forms.
- `System.Web` is not available in .NET 8.
- `.aspx`, master pages, code-behind event handlers, and `Web.config` hosting models must be replaced.

## Required migration approach
1. Create a new ASP.NET Core .NET 8 solution.
2. Rebuild UI flows as Razor Pages or MVC.
3. Move data access logic from `DAL/myDAL.cs` into services and repositories.
4. Replace session-heavy workflows with route-driven handlers and typed models.
5. Move configuration from `Web.config` to `appsettings.json`.
6. Replace legacy packages with .NET 8 compatible packages.

## High-risk areas in this module
- `Clinic Management System.csproj`
- `Web.config`
- `packages.config`
- `DAL/myDAL.cs`
- All `.aspx` and `.Master` files under `Admin`, `Doctor`, and `Patient`

## Expected target architecture
- Domain
- Application
- Infrastructure
- Web

These notes accompany the migration analysis output generated for this task.
