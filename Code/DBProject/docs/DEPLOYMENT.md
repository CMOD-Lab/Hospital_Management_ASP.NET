# Clinic Management System — Deployment Guide

## Overview

This guide covers building, containerizing, and deploying the **Clinic Management System** — a .NET Framework 4.5.2 ASP.NET Web Forms application — to **Azure Kubernetes Service (AKS)** using Windows containers.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Structure](#project-structure)
3. [Technology Stack](#technology-stack)
4. [Local Development with Docker Compose](#local-development-with-docker-compose)
5. [Build and Push Docker Image](#build-and-push-docker-image)
6. [Azure AKS Deployment](#azure-aks-deployment)
7. [Kubernetes Manifest Reference](#kubernetes-manifest-reference)
8. [Configuration and Environment Variables](#configuration-and-environment-variables)
9. [Health Check Endpoint](#health-check-endpoint)
10. [Scaling and Management](#scaling-and-management)
11. [Troubleshooting](#troubleshooting)
12. [Security Considerations](#security-considerations)
13. [Rollback Procedure](#rollback-procedure)

---

## Prerequisites

### Local Development
- Docker Desktop (Windows containers mode enabled)
- .NET Framework 4.5.2 SDK / Visual Studio 2019+
- Azure CLI (`az`) — [Install guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- kubectl — [Install guide](https://kubernetes.io/docs/tasks/tools/)

### Azure Requirements
- Active Azure subscription
- Azure Container Registry (ACR) or Docker Hub account
- AKS cluster with a **Windows node pool** (required for .NET Framework Windows containers)
- Azure Application Gateway Ingress Controller (AGIC) installed on the AKS cluster

### Windows Node Pool Requirement
> ⚠️ **CRITICAL**: This application uses .NET Framework 4.5.2 Windows containers. Your AKS cluster **must** have a Windows node pool. Linux-only clusters will not work.

Create a Windows node pool if not already present:
```bash
az aks nodepool add \
  --resource-group <RESOURCE_GROUP> \
  --cluster-name <CLUSTER_NAME> \
  --os-type Windows \
  --name winnp \
  --node-count 2 \
  --node-vm-size Standard_D4s_v3
```

---

## Project Structure

```
Code/DBProject/
├── Dockerfile                    # Multi-stage Windows container build
├── docker-compose.yml            # Local development compose file
├── .dockerignore                 # Docker build context exclusions
├── Web.config                    # ASP.NET configuration
├── packages.config               # NuGet package references
├── Clinic Management System.csproj
├── Health.aspx / Health.aspx.cs  # Health check endpoint
├── Admin/                        # Admin module pages
├── Doctor/                       # Doctor module pages
├── Patient/                      # Patient module pages
├── DAL/                          # Data Access Layer
├── Helpers/                      # RedisSessionHelper, etc.
├── kubernetes/
│   ├── namespace.yaml
│   ├── deployment.yaml
│   ├── service.yaml
│   └── ingress.yaml
├── scripts/
│   ├── build-push.sh             # Linux/macOS build & push
│   ├── build-push.bat            # Windows build & push
│   ├── deploy-image.sh           # Linux/macOS AKS deploy
│   └── deploy-image.bat          # Windows AKS deploy
└── docs/
    └── DEPLOYMENT.md             # This file
```

---

## Technology Stack

| Component         | Technology                                      |
|-------------------|-------------------------------------------------|
| Framework         | .NET Framework 4.5.2                            |
| Application Type  | ASP.NET Web Forms                               |
| Build Tool        | MSBuild                                         |
| Container OS      | Windows Server Core                             |
| Web Server        | IIS (Internet Information Services)             |
| Session Store     | Azure Cache for Redis (StackExchange.Redis)     |
| Database          | SQL Server (via System.Data.SqlClient)          |
| Monitoring        | Application Insights                            |
| Builder Image     | `mcr.microsoft.com/dotnet/framework/sdk:4.8`    |
| Runtime Image     | `mcr.microsoft.com/dotnet/framework/runtime:4.8`|
| Application Port  | 80 (HTTP)                                       |
| Health Endpoint   | `GET /Health.aspx`                              |

---

## Local Development with Docker Compose

### Step 1: Configure Environment Variables

Create a `.env` file in the `Code/DBProject/` directory:

```env
SQL_CONNECTION_STRING=Data Source=<SQL_HOST>;Initial Catalog=DBProject;User Id=<USER>;Password=<PASSWORD>
REDIS_CONNECTION_STRING=<REDIS_HOST>:6379,abortConnect=false
APPINSIGHTS_INSTRUMENTATIONKEY=<YOUR_KEY>
```

### Step 2: Switch Docker to Windows Containers

Right-click the Docker Desktop tray icon → **Switch to Windows containers**.

### Step 3: Start the Application

```bash
# From Code/DBProject/ directory
docker-compose up --build
```

### Step 4: Access the Application

- Application: http://localhost/
- Health Check: http://localhost/Health.aspx

### Step 5: Stop the Application

```bash
docker-compose down
```

---

## Build and Push Docker Image

### Linux / macOS

```bash
# From repository root
chmod +x Code/DBProject/scripts/build-push.sh
./Code/DBProject/scripts/build-push.sh
```

The script will prompt for:
1. Image tag (default: `latest`)
2. Registry type: `1` for Azure ACR, `2` for Docker Hub
3. Registry credentials

### Windows

```cmd
REM From repository root
Code\DBProject\scripts\build-push.bat
```

### Manual Build

```bash
# From repository root (build context must be '.')
docker build \
  -f Code/DBProject/Dockerfile \
  -t <REGISTRY>/<IMAGE_NAME>:<TAG> \
  .
```

> ⚠️ **Note**: Windows container builds require Docker Desktop in Windows containers mode and may take 10–20 minutes on first build due to the Windows base image download.

---

## Azure AKS Deployment

### Step 1: Ensure AKS Prerequisites

```bash
# Login to Azure
az login

# Verify AKS cluster has Windows node pool
az aks nodepool list \
  --resource-group <RESOURCE_GROUP> \
  --cluster-name <CLUSTER_NAME> \
  --query "[].{Name:name, OS:osType, Count:count}" \
  --output table
```

### Step 2: Install Application Gateway Ingress Controller (if not installed)

```bash
az aks enable-addons \
  --resource-group <RESOURCE_GROUP> \
  --name <CLUSTER_NAME> \
  --addons ingress-appgw \
  --appgw-name <APPGW_NAME> \
  --appgw-subnet-cidr "10.225.0.0/16"
```

### Step 3: Run the Deployment Script

**Linux / macOS:**
```bash
chmod +x Code/DBProject/scripts/deploy-image.sh
./Code/DBProject/scripts/deploy-image.sh
```

**Windows:**
```cmd
Code\DBProject\scripts\deploy-image.bat
```

The script will prompt for:
- Azure Resource Group name
- AKS Cluster name
- Full Docker image URI (e.g., `myregistry.azurecr.io/clinic-management-system:latest`)
- `SQL_CONNECTION_STRING`
- `REDIS_CONNECTION_STRING`
- `APPINSIGHTS_INSTRUMENTATIONKEY` (optional)

### Step 4: Verify Deployment

```bash
# Check pods are running
kubectl get pods -n clinic-management-system

# Check services
kubectl get svc -n clinic-management-system

# Check ingress and get IP
kubectl get ingress -n clinic-management-system

# View pod logs
kubectl logs -l app=clinic-management-system -n clinic-management-system --tail=50
```

### Step 5: Update Ingress Host

Edit `kubernetes/ingress.yaml` and replace `clinic-management-system.example.com` with your actual domain, then re-apply:

```bash
kubectl apply -f kubernetes/ingress.yaml
```

---

## Kubernetes Manifest Reference

### namespace.yaml
Creates the `clinic-management-system` namespace to isolate all application resources.

### deployment.yaml
- **Replicas**: 2 (for high availability)
- **Node Selector**: `kubernetes.io/os: windows` (required for Windows containers)
- **Image**: Placeholder `{{IMAGE_URI}}` replaced by deploy script
- **Resources**: requests `250m CPU / 512Mi RAM`, limits `500m CPU / 1Gi RAM`
- **Probes**: liveness, readiness, and startup probes on `/Health.aspx`
- **Rolling Update**: zero-downtime deployments (`maxUnavailable: 0`)

### service.yaml
- **Type**: `ClusterIP` — internal cluster access only
- **Port**: 80 → container port 80
- Routes traffic to pods with label `app: clinic-management-system`

### ingress.yaml
- **Class**: `azure/application-gateway` (AGIC)
- Routes all traffic (`/`) to the ClusterIP service
- Update `host` field with your actual domain name

---

## Configuration and Environment Variables

| Variable                        | Description                                      | Required |
|---------------------------------|--------------------------------------------------|----------|
| `SQL_CONNECTION_STRING`         | SQL Server connection string                     | Yes      |
| `REDIS_CONNECTION_STRING`       | Azure Cache for Redis connection string          | Yes      |
| `APPINSIGHTS_INSTRUMENTATIONKEY`| Application Insights key for telemetry           | No       |
| `ASPNET_ENV`                    | ASP.NET environment (`Production`)               | Auto-set |

### Using Azure Key Vault with AKS Workload Identity

For production, store secrets in Azure Key Vault and inject via the Secrets Store CSI Driver:

```bash
# Create Key Vault secret
az keyvault secret set \
  --vault-name <KEYVAULT_NAME> \
  --name "SqlConnectionString" \
  --value "<CONNECTION_STRING>"

az keyvault secret set \
  --vault-name <KEYVAULT_NAME> \
  --name "RedisConnectionString" \
  --value "<REDIS_CONNECTION_STRING>"
```

Then configure a `SecretProviderClass` in your namespace to mount secrets as environment variables.

---

## Health Check Endpoint

The application exposes a health check at `GET /Health.aspx`.

**Healthy Response (HTTP 200):**
```json
{"status":"healthy","components":{"redis":"healthy"}}
```

**Unhealthy Response (HTTP 503):**
```json
{"status":"unhealthy","components":{"redis":"unhealthy"}}
```

The health check verifies:
- Application is running and IIS is serving requests
- Redis session store connectivity

Kubernetes probes are configured with:
- **Startup probe**: 30s initial delay, 10s period, 12 retries (allows up to ~2 min for IIS startup)
- **Readiness probe**: 60s initial delay, 15s period
- **Liveness probe**: 90s initial delay, 30s period

---

## Scaling and Management

### Manual Scaling

```bash
kubectl scale deployment clinic-management-system \
  --replicas=4 \
  -n clinic-management-system
```

### Horizontal Pod Autoscaler (HPA)

```bash
kubectl autoscale deployment clinic-management-system \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n clinic-management-system
```

### Rolling Update (New Image Version)

```bash
kubectl set image deployment/clinic-management-system \
  clinic-management-system=<NEW_IMAGE_URI> \
  -n clinic-management-system

kubectl rollout status deployment/clinic-management-system \
  -n clinic-management-system
```

### View Deployment History

```bash
kubectl rollout history deployment/clinic-management-system \
  -n clinic-management-system
```

---

## Troubleshooting

### Pods Not Starting

```bash
# Describe pod for events
kubectl describe pod -l app=clinic-management-system -n clinic-management-system

# Check pod logs
kubectl logs -l app=clinic-management-system -n clinic-management-system --previous
```

**Common causes:**
- No Windows node pool in AKS cluster → Add Windows node pool (see Prerequisites)
- Image pull failure → Verify ACR credentials and image URI
- Redis connection failure → Check `REDIS_CONNECTION_STRING` environment variable
- SQL connection failure → Check `SQL_CONNECTION_STRING` environment variable

### Health Check Failing

```bash
# Test health endpoint directly from within a pod
kubectl exec -it <POD_NAME> -n clinic-management-system -- \
  powershell -Command "Invoke-WebRequest -Uri http://localhost/Health.aspx -UseBasicParsing"
```

### IIS Startup Slow

Windows containers with IIS can take 60–120 seconds to start. The startup probe allows up to 2 minutes. If pods are being killed before startup completes, increase `failureThreshold` in `deployment.yaml`.

### Ingress Not Accessible

```bash
# Check AGIC pod status
kubectl get pods -n kube-system | grep ingress

# Check ingress events
kubectl describe ingress clinic-management-system-ingress -n clinic-management-system

# Verify Application Gateway health
az network application-gateway show-backend-health \
  --resource-group <RESOURCE_GROUP> \
  --name <APPGW_NAME>
```

### Redis Session Issues

If users are losing sessions, verify:
1. `REDIS_CONNECTION_STRING` is correctly set
2. Redis instance is accessible from AKS pods (check network security groups)
3. Check `Health.aspx` response — it reports Redis connectivity status

---

## Security Considerations

1. **Secrets Management**: Never store connection strings in Kubernetes manifests. Use Azure Key Vault with Secrets Store CSI Driver.
2. **Network Policies**: Restrict pod-to-pod communication using Kubernetes NetworkPolicies.
3. **HTTPS**: Configure TLS termination at the Application Gateway level with a valid certificate.
4. **Image Scanning**: Enable Azure Defender for Container Registries to scan images for vulnerabilities.
5. **Workload Identity**: Use AKS Workload Identity instead of connection strings where possible.
6. **Least Privilege**: The IIS application pool runs as `ApplicationPoolIdentity` (least-privilege Windows identity).
7. **Regular Updates**: Keep the base image (`mcr.microsoft.com/dotnet/framework/runtime:4.8`) updated for security patches.

---

## Rollback Procedure

### Immediate Rollback to Previous Version

```bash
kubectl rollout undo deployment/clinic-management-system \
  -n clinic-management-system
```

### Rollback to Specific Revision

```bash
# List revisions
kubectl rollout history deployment/clinic-management-system \
  -n clinic-management-system

# Rollback to revision 2
kubectl rollout undo deployment/clinic-management-system \
  --to-revision=2 \
  -n clinic-management-system
```

### Verify Rollback

```bash
kubectl rollout status deployment/clinic-management-system \
  -n clinic-management-system

kubectl get pods -n clinic-management-system
```

---

*Generated for Clinic Management System — .NET Framework 4.5.2 ASP.NET Web Forms — Azure AKS Deployment*
