@echo off
setlocal enabledelayedexpansion

REM =============================================================================
REM deploy-image.bat - Deploy Clinic Management System to Azure AKS (Windows)
REM .NET Framework 4.5.2 ASP.NET Web Application
REM Target: Azure Kubernetes Service (AKS) with Windows Node Pool
REM =============================================================================

set "APP_NAME=clinic-management-system"
set "NAMESPACE=clinic-management-system"
set "SCRIPT_DIR=%~dp0"
set "MANIFESTS_DIR=%SCRIPT_DIR%..\kubernetes"

echo ==============================================
echo  Clinic Management System - Deploy to AKS
echo ==============================================
echo.

REM ---- Collect Azure / AKS details ----
set /p "RESOURCE_GROUP=Enter Azure Resource Group name: "
if "!RESOURCE_GROUP!"=="" (
    echo ERROR: Resource group cannot be empty.
    exit /b 1
)

set /p "CLUSTER_NAME=Enter AKS Cluster name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: AKS cluster name cannot be empty.
    exit /b 1
)

set /p "IMAGE_URI=Enter full Docker image URI (e.g. myregistry.azurecr.io/clinic-management-system:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Image URI cannot be empty.
    exit /b 1
)

echo.
echo ---- Application Environment Variables ----
echo Press Enter to skip any optional variable.
echo.

set /p "SQL_CONNECTION_STRING=Enter SQL Server connection string (SQL_CONNECTION_STRING): "
set /p "REDIS_CONNECTION_STRING=Enter Redis connection string (REDIS_CONNECTION_STRING): "
set /p "APPINSIGHTS_KEY=Enter Application Insights key (APPINSIGHTS_INSTRUMENTATIONKEY) [optional]: "

echo.
echo Configuring kubectl for AKS cluster: !CLUSTER_NAME!
az aks get-credentials --resource-group "!RESOURCE_GROUP!" --name "!CLUSTER_NAME!" --overwrite-existing
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to get AKS credentials.
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to AKS cluster.
    exit /b 1
)

echo.
echo Preparing Kubernetes manifests...

REM Create temp directory for modified manifests
set "TEMP_DIR=%TEMP%\aks-deploy-%RANDOM%"
mkdir "!TEMP_DIR!"
xcopy /E /I /Q "!MANIFESTS_DIR!" "!TEMP_DIR!" >nul

REM Replace placeholders using PowerShell
powershell -NoProfile -Command ^
  "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{IMAGE_URI}}','!IMAGE_URI!' | Set-Content '!TEMP_DIR!\deployment.yaml'"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to update deployment.yaml & exit /b 1 )

powershell -NoProfile -Command ^
  "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{NAMESPACE}}','!NAMESPACE!' | Set-Content '!TEMP_DIR!\deployment.yaml'"
powershell -NoProfile -Command ^
  "(Get-Content '!TEMP_DIR!\service.yaml') -replace '{{NAMESPACE}}','!NAMESPACE!' | Set-Content '!TEMP_DIR!\service.yaml'"
powershell -NoProfile -Command ^
  "(Get-Content '!TEMP_DIR!\ingress.yaml') -replace '{{NAMESPACE}}','!NAMESPACE!' | Set-Content '!TEMP_DIR!\ingress.yaml'"

powershell -NoProfile -Command ^
  "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{SQL_CONNECTION_STRING}}','!SQL_CONNECTION_STRING!' | Set-Content '!TEMP_DIR!\deployment.yaml'"
powershell -NoProfile -Command ^
  "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{REDIS_CONNECTION_STRING}}','!REDIS_CONNECTION_STRING!' | Set-Content '!TEMP_DIR!\deployment.yaml'"
powershell -NoProfile -Command ^
  "(Get-Content '!TEMP_DIR!\deployment.yaml') -replace '{{APPINSIGHTS_INSTRUMENTATIONKEY}}','!APPINSIGHTS_KEY!' | Set-Content '!TEMP_DIR!\deployment.yaml'"

echo.
echo Applying Kubernetes manifests...

echo   [1/4] Applying namespace...
kubectl apply -f "!TEMP_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace.yaml & exit /b 1 )

echo   [2/4] Applying deployment...
kubectl apply -f "!TEMP_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment.yaml & exit /b 1 )

echo   [3/4] Applying service...
kubectl apply -f "!TEMP_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service.yaml & exit /b 1 )

echo   [4/4] Applying ingress...
kubectl apply -f "!TEMP_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress.yaml & exit /b 1 )

echo.
echo Waiting for deployment rollout...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo WARNING: Rollout did not complete within timeout. Check pod status.
)

echo.
echo Verifying deployed resources...
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo ==============================================
echo  Deployment Complete!
echo ==============================================
echo  Health Check: http://^<INGRESS_IP^>/Health.aspx
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
echo.

REM Cleanup temp files
rmdir /S /Q "!TEMP_DIR!" 2>nul

endlocal
