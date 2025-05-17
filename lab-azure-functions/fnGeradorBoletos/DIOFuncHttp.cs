using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using System.IO;
using Microsoft.Extensions.Logging;
using BarcodeStandard;

namespace fnGeradorBoletos
{

    public static class DIOFuncHttp
    {

        private static readonly string _serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
        private static readonly string _queueName = "gerador-codigo-barras";

        private static readonly ILogger _logger = new LoggerFactory().CreateLogger("DIOFuncHttp");


        [FunctionName("barcode-generate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string valorOriginal = data?.valorOriginal;
                string dataVencimento = data?.dataVencimento;

                string barcodeData;
                if (string.IsNullOrEmpty(valorOriginal) || string.IsNullOrEmpty(dataVencimento))
                {
                    return new BadRequestObjectResult("Valor original e data de vencimento são obrigatórios.");
                }

                // validar formato da data yyyy-MM-dd
                if (!DateTime.TryParse(dataVencimento.ToString(), out DateTime vencimento))
                {
                    return new BadRequestObjectResult("Data de vencimento inválida. Formato esperado: yyyy-MM-dd.");
                }

                if (!DateTime.TryParseExact(dataVencimento.ToString(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out vencimento))
                {
                    return new BadRequestObjectResult("Data de vencimento inválida. Formato esperado: yyyy-MM-dd.");
                }

                string dateStr = vencimento.ToString("yyyyMMdd");

                // Conversão do valor para centavos ate 8 dígitos

                if (!Decimal.TryParse(valorOriginal, out decimal valorDecimal))
                {
                    return new BadRequestObjectResult("Valor original inválido.");
                }

                int valorCentavos = (int)(valorDecimal * 100);
                string valorStr = valorCentavos.ToString("D8");

                string bankCode = "006";
                string baseCode = string.Concat(bankCode, valorStr, dateStr);

                // preenchimento do barcode para 44 caracteres
                barcodeData = baseCode.Length < 44 ? baseCode.PadRight(44, '0') : baseCode.Substring(0, 44);

                _logger.LogInformation($"Barcode data: {barcodeData}");

                Barcode barcode = new Barcode();

                var skImage = barcode.Encode(BarcodeStandard.Type.Code128, barcodeData);

                using (var encodeData = skImage.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
                {
                    var imageBytes = encodeData.ToArray();

                    var base64Image = Convert.ToBase64String(imageBytes);

                    var resultObject = new
                    {
                        barcode = barcodeData,
                        valorOriginal,
                        DataVencimento = dataVencimento,
                        ImageBase64 = base64Image,
                    };

                    await SendFileFallback(resultObject, _serviceBusConnectionString, _queueName);
                    
                    return new OkObjectResult(resultObject);
                }

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Error: {ex.Message}");
            }
            


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
