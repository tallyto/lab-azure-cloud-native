# Fun Rent Car

Este projeto é uma Azure Function responsável por processar locações de veículos no sistema Rent a Car. Ela consome mensagens de uma fila do Azure Service Bus, armazena os dados em um banco SQL e publica o evento para a fila de pagamento.

## Funcionalidades
- Consome mensagens da fila `fila-locacao-auto` do Azure Service Bus.
- Desserializa e valida o modelo de locação recebido.
- Insere os dados da locação em uma tabela SQL (`locacao`).
- Publica a mensagem na fila `payment-queue` para processamento de pagamento.

## Tecnologias Utilizadas
- .NET (C#)
- Azure Functions
- Azure Service Bus
- SQL Server
- Newtonsoft.Json

## Configuração

Crie um arquivo `local.settings.json` com as seguintes configurações:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<sua_connection_string_storage>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ServiceBusConnection": "<sua_connection_string_service_bus>",
    "SqlConnectionString": "<sua_connection_string_sql>"
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
- `funRentCar.cs`: Implementação da Azure Function e modelo de dados.
- `table.sql`: Script de criação da tabela `locacao` no banco SQL.

## Observações
- Certifique-se de que as variáveis de ambiente estejam corretamente configuradas.
- O projeto utiliza o pacote `Newtonsoft.Json` para desserialização dos dados.
- Após inserir no banco, a mensagem é encaminhada para a fila de pagamento para processamento posterior.

## Autor
Projeto para fins educacionais e laboratoriais.
