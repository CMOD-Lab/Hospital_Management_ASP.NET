# Clinic Management System

This project is a .NET 8 migration of a legacy ASP.NET Web Forms clinic management module.

## Architecture
- Domain: entities and interfaces
- Application: DTOs, mappings, services
- Infrastructure: EF Core persistence, repositories, Identity
- Web: Razor Pages UI

## Run
1. Open the solution in the `Code` folder.
2. Run `dotnet restore ../"Clinic Management System.sln"`.
3. Run `dotnet build ../"Clinic Management System.sln"`.
4. Run `dotnet run --project "Clinic Management System.csproj"`.

## Build Troubleshooting
- If you run `dotnet build` from this folder without arguments and encounter `MSB1003`, specify the project or solution file explicitly.
- Valid entry points are `"Clinic Management System.csproj"` from this directory or `../"Clinic Management System.sln"` from this directory.

## Notes
- Legacy Web Forms files are scheduled for removal after successful migration verification.
- Configuration is stored in `appsettings.json`.
- Logging uses Serilog and ASP.NET Core logging abstractions.
