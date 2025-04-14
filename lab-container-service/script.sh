docker build -t blog-container-service:latest .

docker run -d -p 80:80 blog-container-service:latest

az login

az group create --name containerapplab003 --location eastus

# create container registry
az acr create --resource-group containerapplab003 --name containerapplab003 --sku Basic

# login to container registry
az acr login --name containerapplab003

# Tag the image
docker tag blog-container-service:latest containerapplab003.azurecr.io/blog-container-service:latest

# Push the image to the registry
docker push containerapplab003.azurecr.io/blog-container-service:latest


# create Environment container app

az containerapp env create --name containerapplab003 \
    --resource-group containerapplab003 \
    --location eastus 

# create container app

az containerapp create \
    --name containerapplab003 \
    --environment containerapplab003 \
    --resource-group containerapplab003 \
    --image containerapplab003.azurecr.io/blog-container-service:latest \
    --target-port 80 \
    --ingress external \
    --query properties.configuration.ingress.fqdn \
    --registry-user containerapplab003 \
    --registry-password $myPassword \
    --registry-server containerapplab003.azurecr.io 
