using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fair_Share.Functions
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        //[Function(nameof(Function1))]
        //public async Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        //{
        //    _logger.LogInformation("Hello");
        //}

        [Function(nameof(Function1))]
        public async Task Run(
            [ServiceBusTrigger("tasksQueue", Connection = "servicebus")]
                ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions
        )
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
