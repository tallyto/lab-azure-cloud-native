using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DIOFuncHttp
{
    public class DIOFuncQueue
    {
        [FunctionName("DIOFuncQueue")]
        public void Run(
            [ServiceBusTrigger("myqueuetest", Connection = "sbConnectionString")] string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# Service Bus queue trigger function processed: {myQueueItem}");
        }
    }
}
