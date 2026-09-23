# Migration Notes

## What was migrated

- Legacy ASP.NET Web Forms project converted to SDK-style .NET 8 web project.
- `System.Web` dependencies replaced with ASP.NET Core Razor Pages and middleware.
- ADO.NET-style DAL replaced with clean architecture layers and EF Core repositories.
- Legacy package management replaced with `PackageReference`.
- Application startup moved to `Program.cs`.
- Configuration moved to `appsettings.json`.

## Breaking changes

- Web Forms postback lifecycle and server controls are removed.
- Session-based navigation patterns are replaced by route-driven Razor Pages.
- In-memory EF Core is used as the default persistence provider for build portability.
