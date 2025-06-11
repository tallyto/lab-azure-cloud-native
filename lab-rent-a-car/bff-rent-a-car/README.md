# BFF Rent a Car

Este projeto é o Backend For Frontend (BFF) do sistema Rent a Car, responsável por receber requisições HTTP do frontend e encaminhar mensagens para o Azure Service Bus.

## Funcionalidades
- Recebe requisições de locação de veículos via endpoint HTTP.
- Valida os dados recebidos.
- Envia mensagens para uma fila do Azure Service Bus para processamento assíncrono.

## Tecnologias Utilizadas
- Node.js
- Express
- Azure Service Bus SDK
- dotenv
- cors

## Como Executar Localmente

1. **Clone o repositório:**
   ```bash
   git clone <url-do-repositorio>
   ```
2. **Acesse a pasta do projeto:**
   ```bash
   cd lab-azure-cloud-native/lab-rent-a-car/bff-rent-a-car
   ```
3. **Instale as dependências:**
   ```bash
   npm install
   ```
4. **Configure as variáveis de ambiente:**
   Crie um arquivo `.env` na raiz do projeto com o seguinte conteúdo:
   ```env
   SB_CONN_STRING=<sua_connection_string_do_service_bus>
   SB_QUEUE_NAME=<nome_da_fila>
   ```
5. **Inicie o servidor:**
   ```bash
   node index.js
   ```

O servidor estará disponível em `http://localhost:3000`.

## Endpoint

### POST `/api/locacao`
Registra uma nova locação de veículo.

**Body (JSON):**
```json
{
  "nome": "Nome do Cliente",
  "email": "email@exemplo.com",
  "modelo": "Modelo do Carro",
  "ano": 2024,
  "tempoAluguel": 5,
  "data": "2025-06-11"
}
```

**Respostas:**
- `200 OK`: Locação registrada com sucesso.
- `400 Bad Request`: Dados obrigatórios ausentes.
- `500 Internal Server Error`: Erro ao processar a locação ou variáveis de ambiente não configuradas.

## Observações
- Certifique-se de utilizar a connection string do namespace do Service Bus, **não** da fila específica.
- O projeto pode ser facilmente adaptado para outros brokers de mensagens ou integrações.

## Autor
- Projeto para fins educacionais.
