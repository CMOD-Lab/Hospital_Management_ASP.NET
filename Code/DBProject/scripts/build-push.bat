@echo off
setlocal enabledelayedexpansion

REM =============================================================================
REM build-push.bat - Build and Push Docker Image (Windows)
REM Clinic Management System (.NET Framework 4.5.2 ASP.NET Web Application)
REM Supports: Azure Container Registry (ACR) and Docker Hub
REM =============================================================================

set "PROJECT_NAME=clinic-management-system"
set "DOCKERFILE_PATH=Code\DBProject\Dockerfile"
set "BUILD_CONTEXT=."

echo ==============================================
echo  Clinic Management System - Build and Push
echo ==============================================
echo.

REM Sanitize image name using PowerShell
for /f "delims=" %%i in ('powershell -NoProfile -Command "$n = 'clinic-management-system'; $n = $n.ToLower() -replace '[^a-z0-9]+','-'; $n = $n.Trim('-'); Write-Output $n"') do set "IMAGE_NAME=%%i"

REM Prompt for image tag
set /p "IMAGE_TAG_INPUT=Enter image tag [latest]: "
if "!IMAGE_TAG_INPUT!"=="" set "IMAGE_TAG_INPUT=latest"
for /f "delims=" %%i in ('powershell -NoProfile -Command "$t = '!IMAGE_TAG_INPUT!'; $t = $t.ToLower() -replace '[^a-z0-9._-]+','-'; $t = $t.Trim('-'); if ($t -eq '') { $t = 'latest' }; Write-Output $t"') do set "IMAGE_TAG=%%i"

echo.
echo Select container registry:
echo   1. Azure Container Registry (ACR)
echo   2. Docker Hub
set /p "REGISTRY_CHOICE=Enter choice [1 or 2]: "
echo.

if "!REGISTRY_CHOICE!"=="1" goto :acr_login
if "!REGISTRY_CHOICE!"=="2" goto :dockerhub_login
echo ERROR: Invalid choice. Please enter 1 or 2.
exit /b 1

:acr_login
set /p "ACR_NAME=Enter ACR name (e.g. myregistry): "
if "!ACR_NAME!"=="" (
    echo ERROR: ACR name cannot be empty.
    exit /b 1
)
set "ACR_LOGIN_SERVER=!ACR_NAME!.azurecr.io"
set "FULL_IMAGE_NAME=!ACR_LOGIN_SERVER!/!IMAGE_NAME!:!IMAGE_TAG!"

echo Logging in to Azure Container Registry: !ACR_LOGIN_SERVER!
az acr login --name !ACR_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ACR login failed.
    exit /b 1
)
echo ACR login successful.
goto :build

:dockerhub_login
set /p "DOCKER_USERNAME=Enter Docker Hub username: "
if "!DOCKER_USERNAME!"=="" (
    echo ERROR: Docker Hub username cannot be empty.
    exit /b 1
)
set /p "DOCKER_PASSWORD=Enter Docker Hub password/token: "
if "!DOCKER_PASSWORD!"=="" (
    echo ERROR: Docker Hub password cannot be empty.
    exit /b 1
)
set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!"

echo Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed.
    exit /b 1
)
echo Docker Hub login successful.
goto :build

:build
echo.
echo Building Docker image...
echo   Image  : !FULL_IMAGE_NAME!
echo   File   : !DOCKERFILE_PATH!
echo   Context: !BUILD_CONTEXT!
echo.

docker build -f "!DOCKERFILE_PATH!" -t "!FULL_IMAGE_NAME!" "!BUILD_CONTEXT!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)

echo.
echo Build successful. Pushing image to registry...
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo  Image pushed successfully!
echo  !FULL_IMAGE_NAME!
echo ==============================================
echo.
echo Next step: Run scripts\deploy-image.bat to deploy to Azure AKS.

endlocal
