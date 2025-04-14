
# Lab Azure Cloud Native - Container Service

Este projeto demonstra como criar e implantar um serviço de contêiner usando o Docker e o Azure Container Apps. O script `script.sh` automatiza o processo de construção, registro e implantação de uma aplicação em contêiner.

## Pré-requisitos

Antes de executar o script, certifique-se de ter os seguintes itens instalados e configurados:

- [Docker](https://www.docker.com/)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- Uma conta ativa no [Microsoft Azure](https://azure.microsoft.com/)

## Passos do Script

1. **Construção da Imagem Docker**:
    O script cria uma imagem Docker chamada `blog-container-service:latest`.

2. **Execução Local do Contêiner**:
    O contêiner é executado localmente na porta 80.

3. **Login no Azure**:
    O script realiza login na sua conta Azure.

4. **Criação do Resource Group**:
    Um grupo de recursos chamado `containerapplab003` é criado na região `eastus`.

5. **Criação do Azure Container Registry (ACR)**:
    Um registro de contêiner chamado `containerapplab003` é criado com o SKU `Basic`.

6. **Login no ACR**:
    O script realiza login no registro de contêiner.

7. **Tag e Push da Imagem**:
    A imagem Docker é marcada e enviada para o ACR.

8. **Criação do Ambiente do Azure Container App**:
    Um ambiente para o Azure Container App é criado.

9. **Criação do Azure Container App**:
    O aplicativo em contêiner é criado e configurado para ser acessível externamente na porta 80.

## Como Executar

1. Clone este repositório:
    ```bash
    git clone https://github.com/tallyto/lab-azure-cloud-native.git
    cd lab-azure-cloud-native/lab-container-service
    ```

2. Torne o script executável:
    ```bash
    chmod +x script.sh
    ```

3. Execute o script:
    ```bash
    ./script.sh
    ```

4. Durante a execução, será solicitado o login no Azure e a senha do registro de contêiner (`$myPassword`).

## Resultado

Após a execução bem-sucedida do script, o aplicativo estará disponível em um domínio gerado pelo Azure. O domínio será exibido no final do script.

## Observações

- Certifique-se de substituir `$myPassword` pela senha do registro de contêiner.
- O script utiliza a região `eastus` e nomes fixos para os recursos. Altere conforme necessário.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).
