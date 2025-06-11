# Fun Payment Process

Este projeto é uma Azure Function responsável pelo processamento de pagamentos no contexto do sistema Rent a Car. Ela consome mensagens de uma fila do Azure Service Bus, processa o pagamento, define um status aleatório e, em caso de aprovação, envia uma notificação para outra fila.

## Funcionalidades
- Consome mensagens da fila `payment-queue` do Azure Service Bus.
- Desserializa e valida o modelo de pagamento recebido.
- Define o status do pagamento de forma aleatória: `Aprovado`, `Reprovado` ou `Em análise`.
- Se aprovado, envia mensagem de notificação para uma fila de notificações.
- Persiste o resultado no Azure Cosmos DB.

## Tecnologias Utilizadas
- .NET (C#)
- Azure Functions
- Azure Service Bus
- Azure Cosmos DB
- Microsoft.Extensions.Logging
- System.Text.Json

## Configuração

Crie um arquivo `local.settings.json` com as seguintes configurações:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<sua_connection_string_storage>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ServiceBusConnection": "<sua_connection_string_service_bus>",
    "NotificationQueue": "<nome_da_fila_de_notificacao>",
    "CosmosDBConnection": "<sua_connection_string_cosmosdb>",
    "CosmosDB": "<nome_do_banco>",
    "CosmosContainer": "<nome_do_container>"
  }
}
```

## Como Executar Localmente
1. Instale as dependências do .NET e Azure Functions Core Tools.
2. Execute o comando:
   ```bash
   func start
   ```

## Estrutura Principal
- `FunPaymentProcess.cs`: Implementação da Azure Function.
- `model/PaymentModel.cs`: Modelo de dados do pagamento.

## Observações
- O status do pagamento é definido aleatoriamente para simular diferentes cenários.
- Em caso de aprovação, uma mensagem é enviada para a fila de notificações.
- O resultado do processamento é salvo no Cosmos DB.

## Autor
Projeto para fins educacionais e laboratoriais.
