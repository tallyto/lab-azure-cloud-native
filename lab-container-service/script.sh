#!/bin/bash

# Variáveis de ambiente
RESOURCE_GROUP="containerapplab003" # Nome do grupo de recursos
LOCATION="eastus" # Localização do recurso
ACR_NAME="containerapplab003" # Nome do Azure Container Registry
IMAGE_NAME="blog-container-service" # Nome da imagem Docker
IMAGE_TAG="latest" # Tag da imagem Docker
ACR_SERVER="$ACR_NAME.azurecr.io" # Servidor do Azure Container Registry
CONTAINER_APP_ENV="containerapplab003" # Nome do ambiente do Container App
CONTAINER_APP_NAME="containerapplab003" # Nome do Container App
TARGET_PORT=80 # Porta alvo do container

# Build da imagem Docker
docker build -t $IMAGE_NAME:$IMAGE_TAG .

# Executa o container localmente para teste
docker run -d -p $TARGET_PORT:$TARGET_PORT $IMAGE_NAME:$IMAGE_TAG

# Login no Azure
az login

# Criação do grupo de recursos
az group create --name $RESOURCE_GROUP --location $LOCATION

# Criação do Azure Container Registry (ACR)
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic

# Login no ACR
az acr login --name $ACR_NAME

# Tag da imagem para o ACR
docker tag $IMAGE_NAME:$IMAGE_TAG $ACR_SERVER/$IMAGE_NAME:$IMAGE_TAG

# Push da imagem para o ACR
docker push $ACR_SERVER/$IMAGE_NAME:$IMAGE_TAG

# Criação do ambiente do Azure Container App
az containerapp env create --name $CONTAINER_APP_ENV \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION 

# Criação do Azure Container App
az containerapp create \
    --name $CONTAINER_APP_NAME \
    --environment $CONTAINER_APP_ENV \
    --resource-group $RESOURCE_GROUP \
    --image $ACR_SERVER/$IMAGE_NAME:$IMAGE_TAG \
    --target-port $TARGET_PORT \
    --ingress external \
    --query properties.configuration.ingress.fqdn \
    --registry-user $ACR_NAME \
    --registry-password $myPassword \
    --registry-server $ACR_SERVER
