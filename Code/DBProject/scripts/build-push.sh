#!/bin/bash
# =============================================================================
# build-push.sh - Build and Push Docker Image
# Clinic Management System (.NET Framework 4.5.2 ASP.NET Web Application)
# Supports: Azure Container Registry (ACR) and Docker Hub
# =============================================================================
set -e

PROJECT_NAME="clinic-management-system"
DOCKERFILE_PATH="Code/DBProject/Dockerfile"
BUILD_CONTEXT="."

echo "=============================================="
echo " Clinic Management System - Build & Push"
echo "=============================================="
echo ""

# Sanitize project name for Docker tag compliance
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

# Prompt for image tag
read -rp "Enter image tag [latest]: " IMAGE_TAG_INPUT
IMAGE_TAG=$(echo "${IMAGE_TAG_INPUT:-latest}" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
if [ -z "$IMAGE_TAG" ]; then
  IMAGE_TAG="latest"
fi

echo ""
echo "Select container registry:"
echo "  1. Azure Container Registry (ACR)"
echo "  2. Docker Hub"
read -rp "Enter choice [1 or 2]: " REGISTRY_CHOICE

echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
  # ---- Azure Container Registry ----
  read -rp "Enter ACR name (e.g. myregistry): " ACR_NAME
  if [ -z "$ACR_NAME" ]; then
    echo "ERROR: ACR name cannot be empty."
    exit 1
  fi

  ACR_LOGIN_SERVER="${ACR_NAME}.azurecr.io"
  FULL_IMAGE_NAME="${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to Azure Container Registry: ${ACR_LOGIN_SERVER}"
  az acr login --name "$ACR_NAME"
  echo "ACR login successful."

elif [ "$REGISTRY_CHOICE" = "2" ]; then
  # ---- Docker Hub ----
  read -rp "Enter Docker Hub username: " DOCKER_USERNAME
  if [ -z "$DOCKER_USERNAME" ]; then
    echo "ERROR: Docker Hub username cannot be empty."
    exit 1
  fi
  read -rsp "Enter Docker Hub password/token: " DOCKER_PASSWORD
  echo ""
  if [ -z "$DOCKER_PASSWORD" ]; then
    echo "ERROR: Docker Hub password cannot be empty."
    exit 1
  fi

  FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to Docker Hub..."
  echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
  echo "Docker Hub login successful."

else
  echo "ERROR: Invalid choice. Please enter 1 or 2."
  exit 1
fi

echo ""
echo "Building Docker image..."
echo "  Image : ${FULL_IMAGE_NAME}"
echo "  File  : ${DOCKERFILE_PATH}"
echo "  Context: ${BUILD_CONTEXT}"
echo ""

docker build \
  -f "${DOCKERFILE_PATH}" \
  -t "${FULL_IMAGE_NAME}" \
  "${BUILD_CONTEXT}"

echo ""
echo "Build successful. Pushing image to registry..."
docker push "${FULL_IMAGE_NAME}"

echo ""
echo "=============================================="
echo " Image pushed successfully!"
echo " ${FULL_IMAGE_NAME}"
echo "=============================================="
echo ""
echo "Next step: Run scripts/deploy-image.sh to deploy to Azure AKS."
