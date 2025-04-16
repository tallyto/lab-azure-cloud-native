#!/bin/bash

echo "Publishing the .NET application..."
dotnet publish -c Release -o publish /p:PublishIISAssets=true

echo "Creating a zip file of the published application..."
cd publish
zip -r ../publish.zip .
cd ..

webAppName=dotnet-api-gateway-lab
resourceGroup=appsvc_windows_eastus

echo "Deploying the application to Azure Web App '$webAppName'..."
az webapp deployment source config-zip \
  --name $webAppName \
  --resource-group $resourceGroup \
  --src publish.zip

echo "Deployment completed."
