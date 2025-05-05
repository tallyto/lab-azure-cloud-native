using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;

namespace fnGeradorBoletos
{

    public static class DIOFuncHttp
    {

        private static readonly string _serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
        private static readonly string _queueName = "gerador-codigo-barras";


        [FunctionName("barcode-generate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {

            try
            {
                var resultObject = new
                {
                    barcode = "1234567890123456789012345678901234567890",
                    valorOgirinal = 100.00,
                    DataVencimento = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"),
                    ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAJYAAAB4CAYAAAD1j3x2AAAAOXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZSBDcmVhdGlvbiB0aW1lADIwMjMtMDktMjVUMjE6NTI6NTYtMDQ6MDAq7+5cAAAAJXRFWHRkYXRlIFRpbWUAMjAyMy0wOS0yNVQyMToyNToyNi0wNDowMK8r7gAAAABJRU5ErkJggg=="
                };

                await SendFileFallback(resultObject, _serviceBusConnectionString, _queueName);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Error: {ex.Message}");
            }
            return new OkObjectResult("Message sent to Service Bus queue successfully.");


        }

        private static async Task SendFileFallback(object resultObject, string serviceBusConnectionString, string queueName)
        {
            await using var client = new ServiceBusClient(serviceBusConnectionString);
            ServiceBusSender sender = client.CreateSender(queueName);

            var messageBody = JsonConvert.SerializeObject(resultObject);

            ServiceBusMessage message = new ServiceBusMessage(messageBody);

            await sender.SendMessageAsync(message);
            Console.WriteLine($"Message sent to queue: {queueName}");
        }
    }
}
