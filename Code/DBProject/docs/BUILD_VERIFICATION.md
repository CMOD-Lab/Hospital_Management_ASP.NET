# Build Verification

Build verification is pending execution in the transformation environment.

## Intended commands
- `dotnet restore`
- `dotnet build`

## Expected outcome
- All projects target `net8.0`.
- No `System.Web` or legacy Web Forms package dependencies remain in project references.
