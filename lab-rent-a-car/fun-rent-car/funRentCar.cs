using System;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace fun_rent_car
{
    public class funRentCar
    {
        [FunctionName("funRentCar")]
        public void Run([ServiceBusTrigger("fila-locacao-auto", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            // Parse JSON to RentModel
            RentModel rent = null;
            try
            {
                // Faz o parse do JSON considerando snake_case para camelCase
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
                };
                dynamic obj = JsonConvert.DeserializeObject(myQueueItem);
                rent = new RentModel
                {
                    Nome = obj.nome,
                    Email = obj.email,
                    Modelo = obj.modelo,
                    Ano = obj.ano,
                    TempoAluguel = obj.tempoAluguel,
                    Data = obj.data,
                    DataInsercao = obj.dataInsercao != null ? obj.dataInsercao : DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                log.LogError($"Error parsing message: {ex.Message}");
                return;
            }

            // Get connection string from environment
            string connectionString = System.Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(connectionString))
            {
                log.LogError("SQL connection string is missing.");
                return;
            }

            // Insert into database
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO locacao (nome, email, modelo, ano, tempo_aluguel, data, data_insercao) VALUES (@Nome, @Email, @Modelo, @Ano, @TempoAluguel, @Data, @DataInsercao)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nome", rent.Nome ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", rent.Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Modelo", rent.Modelo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Ano", rent.Ano);
                        cmd.Parameters.AddWithValue("@TempoAluguel", rent.TempoAluguel ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Data", rent.Data);
                        cmd.Parameters.AddWithValue("@DataInsercao", rent.DataInsercao);
                        cmd.ExecuteNonQuery();
                    }
                }
                log.LogInformation("Data inserted into database successfully.");

                // Publish to payment-queue
                try
                {
                    var serviceBusConnectionString = System.Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);
                    if (string.IsNullOrEmpty(serviceBusConnectionString))
                    {
                        log.LogError("Service Bus connection string is missing.");
                        return;
                    }
                    var client = new Azure.Messaging.ServiceBus.ServiceBusClient(serviceBusConnectionString);
                    var sender = client.CreateSender("payment-queue");
                    var messageBody = JsonConvert.SerializeObject(rent);
                    var message = new Azure.Messaging.ServiceBus.ServiceBusMessage(messageBody);
                    sender.SendMessageAsync(message).GetAwaiter().GetResult();
                    log.LogInformation("Message sent to payment-queue.");
                }
                catch (Exception ex)
                {
                    log.LogError($"Error sending message to payment-queue: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Database insert error: {ex.Message}");
            }
        }
    }

    public class RentModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public string TempoAluguel { get; set; } // Corrigido para string
        public DateTime Data { get; set; }
        public DateTime DataInsercao { get; set; }
    }
}
