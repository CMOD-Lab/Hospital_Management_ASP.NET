#!/bin/bash
# =============================================================================
# deploy-image.sh - Deploy Clinic Management System to Azure AKS
# .NET Framework 4.5.2 ASP.NET Web Application
# Target: Azure Kubernetes Service (AKS) with Windows Node Pool
# =============================================================================
set -e
set -o pipefail

APP_NAME="clinic-management-system"
NAMESPACE="clinic-management-system"
MANIFESTS_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/kubernetes"

echo "=============================================="
echo " Clinic Management System - Deploy to AKS"
echo "=============================================="
echo ""

# ---- Collect Azure / AKS details ----
read -rp "Enter Azure Resource Group name: " RESOURCE_GROUP
if [ -z "$RESOURCE_GROUP" ]; then
  echo "ERROR: Resource group cannot be empty."
  exit 1
fi

read -rp "Enter AKS Cluster name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: AKS cluster name cannot be empty."
  exit 1
fi

read -rp "Enter full Docker image URI (e.g. myregistry.azurecr.io/clinic-management-system:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Image URI cannot be empty."
  exit 1
fi

echo ""
echo "---- Application Environment Variables ----"
echo "Press Enter to skip any optional variable."
echo ""

read -rp "Enter SQL Server connection string (SQL_CONNECTION_STRING): " SQL_CONNECTION_STRING
read -rp "Enter Redis connection string (REDIS_CONNECTION_STRING): " REDIS_CONNECTION_STRING
read -rp "Enter Application Insights key (APPINSIGHTS_INSTRUMENTATIONKEY) [optional]: " APPINSIGHTS_KEY

echo ""
echo "Configuring kubectl for AKS cluster: ${CLUSTER_NAME}"
az aks get-credentials --resource-group "$RESOURCE_GROUP" --name "$CLUSTER_NAME" --overwrite-existing

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to AKS cluster."; exit 1; }

echo ""
echo "Preparing Kubernetes manifests..."

# Work on copies to avoid modifying originals
TEMP_DIR=$(mktemp -d)
cp -r "$MANIFESTS_DIR"/* "$TEMP_DIR"/

# Replace image placeholder
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" "$TEMP_DIR/deployment.yaml"

# Replace namespace placeholder
sed -i "s|{{NAMESPACE}}|${NAMESPACE}|g" "$TEMP_DIR/deployment.yaml"
sed -i "s|{{NAMESPACE}}|${NAMESPACE}|g" "$TEMP_DIR/service.yaml"
sed -i "s|{{NAMESPACE}}|${NAMESPACE}|g" "$TEMP_DIR/ingress.yaml"

# Replace environment variable placeholders
if [ -n "$SQL_CONNECTION_STRING" ]; then
  sed -i "s|{{SQL_CONNECTION_STRING}}|${SQL_CONNECTION_STRING}|g" "$TEMP_DIR/deployment.yaml"
else
  sed -i "s|{{SQL_CONNECTION_STRING}}||g" "$TEMP_DIR/deployment.yaml"
fi

if [ -n "$REDIS_CONNECTION_STRING" ]; then
  sed -i "s|{{REDIS_CONNECTION_STRING}}|${REDIS_CONNECTION_STRING}|g" "$TEMP_DIR/deployment.yaml"
else
  sed -i "s|{{REDIS_CONNECTION_STRING}}||g" "$TEMP_DIR/deployment.yaml"
fi

if [ -n "$APPINSIGHTS_KEY" ]; then
  sed -i "s|{{APPINSIGHTS_INSTRUMENTATIONKEY}}|${APPINSIGHTS_KEY}|g" "$TEMP_DIR/deployment.yaml"
else
  sed -i "s|{{APPINSIGHTS_INSTRUMENTATIONKEY}}||g" "$TEMP_DIR/deployment.yaml"
fi

echo ""
echo "Applying Kubernetes manifests..."

echo "  [1/4] Applying namespace..."
kubectl apply -f "$TEMP_DIR/namespace.yaml"

echo "  [2/4] Applying deployment..."
kubectl apply -f "$TEMP_DIR/deployment.yaml"

echo "  [3/4] Applying service..."
kubectl apply -f "$TEMP_DIR/service.yaml"

echo "  [4/4] Applying ingress..."
kubectl apply -f "$TEMP_DIR/ingress.yaml"

echo ""
echo "Waiting for deployment rollout..."
kubectl rollout status deployment/"${APP_NAME}" -n "${NAMESPACE}" --timeout=300s

echo ""
echo "Verifying deployed resources..."
kubectl get pods,svc,ingress -n "${NAMESPACE}"

echo ""
echo "Retrieving application URL..."
INGRESS_IP=$(kubectl get ingress "${APP_NAME}-ingress" -n "${NAMESPACE}" -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>/dev/null || echo "")
INGRESS_HOST=$(kubectl get ingress "${APP_NAME}-ingress" -n "${NAMESPACE}" -o jsonpath='{.spec.rules[0].host}' 2>/dev/null || echo "")

echo ""
echo "=============================================="
echo " Deployment Complete!"
echo "=============================================="
if [ -n "$INGRESS_IP" ]; then
  echo " Application IP  : http://${INGRESS_IP}"
fi
if [ -n "$INGRESS_HOST" ]; then
  echo " Application Host: http://${INGRESS_HOST}"
fi
echo " Health Check    : http://${INGRESS_HOST:-$INGRESS_IP}/Health.aspx"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/${APP_NAME} -n ${NAMESPACE}"
echo ""

# Cleanup temp files
rm -rf "$TEMP_DIR"
