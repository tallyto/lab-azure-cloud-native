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

                // Cálculo do fator de vencimento (dias desde 07/10/1997)
                DateTime baseDate = new DateTime(1997, 10, 7);
                int fatorVencimento = (vencimento - baseDate).Days;
                string fatorVencimentoStr = fatorVencimento.ToString("D4");

                // Conversão do valor para centavos, 10 dígitos
                if (!Decimal.TryParse(valorOriginal, out decimal valorDecimal))
                {
                    return new BadRequestObjectResult("Valor original inválido.");
                }
                long valorCentavos = (long)(valorDecimal * 100);
                string valorStr = valorCentavos.ToString("D10");

                // Montagem dos campos do código de barras
                string banco = "001"; // Exemplo: Banco do Brasil
                string moeda = "9";   // Real
                string campoLivre = new string('0', 25); // Preenchido com zeros

                // Monta sem o DV geral
                string codigoSemDV = banco + moeda + fatorVencimentoStr + valorStr + campoLivre;

                // Calcula DV geral (módulo 11)
                int dvGeral = CalcularModulo11(codigoSemDV);

                // Monta o código de barras final (44 dígitos)
                barcodeData = banco + moeda + dvGeral.ToString() + fatorVencimentoStr + valorStr + campoLivre;

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

        [FunctionName("barcode-validate")]
        public static async Task<IActionResult> ValidateBarcode(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string barcode = data?.barcode;

                if (string.IsNullOrEmpty(barcode))
                {
                    return new BadRequestObjectResult("Barcode é obrigatório.");
                }

                // Validação do DV geral (módulo 11)
                // Estrutura: BBBM D FFFF VVVVVVVVVV CCCCCCCCCCCCCCCCCCCCCCCCCCC
                // BBB: banco, M: moeda, D: DV, FFFF: fator vencimento, VVVVVVVVVV: valor, C...: campo livre
                string codigoSemDV = barcode.Substring(0, 4) + barcode.Substring(5); // Remove o DV (posição 4)
                int dvInformado = int.Parse(barcode.Substring(4, 1));
                int dvCalculado = CalcularModulo11(codigoSemDV);

                bool dvValido = dvInformado == dvCalculado;

                // Retorna objeto detalhado
                var result = new
                {
                    isValid = dvValido,
                    dvInformado,
                    dvCalculado
                };

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Erro: {ex.Message}");
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

        // Função para cálculo do módulo 11 (DV geral)
        private static int CalcularModulo11(string input)
        {
            int soma = 0;
            int peso = 2;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                soma += (input[i] - '0') * peso;
                peso++;
                if (peso > 9) peso = 2;
            }
            int resto = soma % 11;
            int dv = 11 - resto;
            if (dv == 0 || dv == 10 || dv == 11) dv = 1;
            return dv;
        }
    }
}
