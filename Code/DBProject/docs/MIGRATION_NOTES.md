# Migration Notes

## What was migrated
- Legacy Web Forms project file converted to SDK-style .NET 8 web project.
- `System.Web`-based UI replaced by Razor Pages.
- ADO.NET DAL responsibilities mapped to clean architecture services and repositories.
- Configuration moved from `Web.config` to `appsettings.json`.
- Legacy package management via `packages.config` replaced with modern PackageReference.

## Breaking changes
- Web Forms pages, master pages, and code-behind event lifecycle are no longer used.
- State management now relies on Razor Pages handlers and application services.
- Identity replaces legacy forms-auth style patterns.

## Future improvements
- Replace in-memory EF Core setup with SQL Server configuration for production.
- Add repository coverage for billing, notifications, feedback, and treatment workflows.
