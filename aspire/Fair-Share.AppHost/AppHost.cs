using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Add database resource
var postgres = builder.AddConnectionString("DefaultConnection");

// Add Azure Service Bus resource
var serviceBus = builder
    .AddAzureServiceBus("serviceBus")
    .RunAsEmulator(e =>
    {
        e.WithLifetime(ContainerLifetime.Persistent);
    });
var queue = serviceBus.AddServiceBusQueue("tasksQueue");

// Add API service
var api = builder
    .AddProject<Projects.Fair_Share_Api>("api")
    .WithReference(postgres)
    .WithReference(serviceBus)
    .WaitFor(postgres);

//Add Azure Functions service
var functions = builder
    .AddAzureFunctionsProject<Projects.Fair_Share_Functions>("fair-share-functions")
    .WithReference(serviceBus)
    .WithReference(postgres)
    .WaitFor(serviceBus);

builder.AddAzureFunctionsProject<Projects.test>("test");

builder.Build().Run();
