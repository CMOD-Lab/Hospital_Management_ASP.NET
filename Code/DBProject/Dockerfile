# =============================================================================
# Clinic Management System - Dockerfile
# .NET Framework 4.5.2 ASP.NET Web Application
# Target: Azure AKS (Windows Containers)
# Runtime Base Image: mcr.microsoft.com/dotnet/framework/runtime:4.8
# =============================================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS builder

WORKDIR /src

# Copy solution and project files first for NuGet package restore caching
COPY ["Code/DBProject/DBProject.sln", "Code/DBProject/"]
COPY ["Code/DBProject/Clinic Management System.csproj", "Code/DBProject/"]
COPY ["Code/DBProject/packages.config", "Code/DBProject/"]

# Restore NuGet packages (layer cached separately from source code)
WORKDIR /src/Code/DBProject
RUN nuget restore "Clinic Management System.csproj" -PackagesDirectory ../packages

# Copy all source code
WORKDIR /src
COPY . .

# Build and publish in Release configuration
WORKDIR /src/Code/DBProject
RUN msbuild "Clinic Management System.csproj" \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:PrecompileBeforePublish=true \
    /p:EnableUpdateable=false \
    /p:DebugType=none \
    /p:Optimize=true \
    /nologo \
    /verbosity:minimal

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8 AS runtime

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Install IIS and ASP.NET features
RUN Install-WindowsFeature -Name Web-Server, Web-Asp-Net45, Web-Net-Ext45, Web-ISAPI-Ext, Web-ISAPI-Filter, \
    Web-Default-Doc, Web-Static-Content, Web-Http-Errors, Web-Http-Logging, \
    Web-Request-Monitor, Web-Windows-Auth, Web-Filtering -IncludeManagementTools

WORKDIR C:\inetpub\wwwroot

# Remove default IIS content
RUN Remove-Item -Recurse -Force C:\inetpub\wwwroot\* -ErrorAction SilentlyContinue

# Copy published application from build stage
COPY --from=builder C:\publish .

# Configure IIS application pool and site
RUN Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name processModel.identityType -Value ApplicationPoolIdentity; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedRuntimeVersion -Value 'v4.0'; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedPipelineMode -Value 'Integrated'; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'

# Set environment variables for the application
ENV ASPNET_ENV=Production
ENV REDIS_CONNECTION_STRING=""
ENV SQL_CONNECTION_STRING=""
ENV APPINSIGHTS_INSTRUMENTATIONKEY=""

# Expose HTTP port
EXPOSE 80

# Start IIS service
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
