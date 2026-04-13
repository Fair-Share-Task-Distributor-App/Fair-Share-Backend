using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fair_Share.Functions
{
    public class Function2
    {
        private readonly ILogger<Function1> _logger;

        public Function2(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function2))]
        public async Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Hello");
        }
    }
}
