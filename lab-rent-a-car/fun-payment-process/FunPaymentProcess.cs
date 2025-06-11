using Azure.Messaging.ServiceBus;
using fun_payment_process.model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace fun_payment_process
{
    public class FunPaymentProcess
    {
        private readonly ILogger<FunPaymentProcess> _logger;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new () { PropertyNameCaseInsensitive = true };
        // Corrigido: inicialização correta do array de status
        private readonly string[] StatusList = new string[] { "Aprovado", "Reprovado", "Em análise" };
        private readonly Random random = new ();
        private readonly IConfiguration _configuration;

        public FunPaymentProcess(ILogger<FunPaymentProcess> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function(nameof(FunPaymentProcess))]
        [CosmosDBOutput("%CosmosDB%", "%CosmosContainer%", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "/IdPayment")]
        public async Task<object?> Run(
            [ServiceBusTrigger("payment-queue", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            PaymentModel? payment = null;

            try
            {
                payment = JsonSerializer.Deserialize<PaymentModel>(message.Body.ToString(), _jsonSerializerOptions);
                if (payment == null)
                {
                    await messageActions.DeadLetterMessageAsync(message, null, "The message could not be deserialized.");
                    await messageActions.CompleteMessageAsync(message);
                    return null;
                }
                int index = random.Next(StatusList.Length);
                string status = StatusList[index];
                payment.Status = status;

                if(status == "Aprovado")
                {
                    payment.DataAprovacao = DateTime.Now;
                    await SentDoNotificationQueue(payment);
                }

                await messageActions.CompleteMessageAsync(message);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize message body.");
                await messageActions.DeadLetterMessageAsync(message, null, $"Erro: {ex.Message}");
                await messageActions.CompleteMessageAsync(message);
                return null;
            }
        }

        private async Task SentDoNotificationQueue(PaymentModel payment)
        {
            var connectionString = _configuration.GetSection("ServiceBusConnection").Value ?? string.Empty;
            var queueName = _configuration.GetSection("NotificationQueue").Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(queueName))
            {
                _logger.LogError("ServiceBusConnection or NotificationQueue configuration is missing.");
                return;
            }
            var serviceBusClient = new ServiceBusClient(connectionString);
            var sender = serviceBusClient.CreateSender(queueName);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(payment))
            {
                ContentType = "application/json",
            };
            message.ApplicationProperties["IdPayment"] = payment.IdPayment;
            message.ApplicationProperties["type"] = "notification";
            message.ApplicationProperties["message"] = "Pagamento aprovado com sucesso!";
            try
            {
                await sender.SendMessageAsync(message);
                _logger.LogInformation("Message sent to notification queue: {id}", payment.IdPayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to notification queue");
            }
        }
    }
}
