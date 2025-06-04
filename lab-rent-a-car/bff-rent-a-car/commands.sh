az acr login --name acrlab007tsousa

docker build -t bff-renat-car:latest .

docker tag bff-rent-car acrlab007tsousa.azurecr.io/bff-rent-car:latest

docker push acrlab007tsousa.azurecr.io/bff-rent-car:latest

# az containerapp env create --name bff-rent-car-env --resource-group lab007 --location eastus2
az containerapp create --name bff-rent-car --resource-group lab007 --environment managedEnvironment-lab007-9b7c --image acrlab007tsousa.azurecr.io/bff-rent-car:latest --registry-server acrlab007tsousa.azurecr.io --ingress external --target-port 3000