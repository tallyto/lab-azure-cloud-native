Claro! Aqui está uma versão mais funcional e organizada do seu `README.md`, com instruções mais claras e organizadas para facilitar o uso:

---

# 🚀 Azure Functions - Projeto DIOFunc

Este projeto demonstra como criar **Azure Functions** com **gatilhos HTTP** e **gatilhos de fila do Azure Service Bus** utilizando `.NET`.

---

## 📦 Pré-requisitos

* [.NET SDK](https://dotnet.microsoft.com/en-us/download)
* [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
* [Azure CLI (opcional)](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
* Conta no [Azure](https://azure.microsoft.com/)

---

## 📁 Criar o projeto Azure Functions

```bash
func init DIOFuncHttp --worker-runtime dotnet
cd DIOFuncHttp
```

---

## 🌐 Criar uma Azure Function HTTP

```bash
func new --name DIOFuncHttp --template "HTTP trigger" --authlevel "anonymous"
```

> Essa função responderá a requisições HTTP sem autenticação.

---

## 📨 Criar uma Azure Function com gatilho do Service Bus

1. Instale o pacote necessário:

```bash
dotnet add package Microsoft.Azure.WebJobs.Extensions.ServiceBus
```

2. Crie a função com o gatilho de fila:

```bash
func new --name DIOFuncQueue --template "Service Bus Queue trigger"
```

---

## ⚙️ Configuração do `local.settings.json`

Adicione a connection string do Service Bus:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "sbConnectionString": "<sua-connection-string-do-service-bus>"
  }
}
```

---

## ▶️ Executar o projeto localmente

```bash
func start
```

---

## 💡 Dicas

* Use o [Azure Service Bus Explorer](https://learn.microsoft.com/en-us/azure/service-bus-messaging/explorer) para testar e monitorar suas filas.
* Para enviar mensagens para a fila, você pode usar uma aplicação console ou ferramentas como Postman + Azure REST API.