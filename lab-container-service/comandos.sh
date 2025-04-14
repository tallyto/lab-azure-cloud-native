# Adicione a extensão Azure Container Apps ao Azure CLI e certifique-se de que está atualizada
az extension add --name containerapp --upgrade

# Registre o provedor de recursos Microsoft.App na sua assinatura do Azure
az provider register --namespace Microsoft.App

# Verifique o status do registro do provedor de recursos Microsoft.App
az provider show -n Microsoft.App

# Registre o provedor de recursos Microsoft.OperationalInsights na sua assinatura do Azure
az provider register --namespace Microsoft.OperationalInsights

# Defina variáveis para o grupo de recursos, localização e ambiente do container
myRG=tsousacontainerapp
myLocation=eastus
myContainerEnv=hsouza-env-001

# Crie um grupo de recursos na localização especificada
az group create --name $myRG --location $myLocation

# Crie um ambiente de Container Apps no grupo de recursos e localização especificados
az containerapp env create --name $myContainerEnv --resource-group $myRG --location $myLocation

# Crie um aplicativo de container com os parâmetros especificados
az containerapp create \
    --name myapp \  # Nome do aplicativo de container
    --environment $myContainerEnv \  # Referência ao ambiente do container
    --resource-group $myRG \  # Grupo de recursos onde o aplicativo será criado
    --image mcr.microsoft.com/azuredocs/aci-helloworld:latest \  # Imagem Docker a ser implantada
    --target-port 80 \  # Porta que o aplicativo irá escutar
    --ingress external \  # Habilitar entrada externa para acesso público
    --query properties.configuration.ingress.fqdn  # Exibir o nome de domínio totalmente qualificado (FQDN) do aplicativo