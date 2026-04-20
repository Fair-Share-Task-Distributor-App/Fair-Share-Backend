using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Add database resource
var postgres = builder.AddPostgres("postgres").WithDataVolume();
var postgresdb = postgres.AddDatabase("fair-share-db");

// Add Azure Service Bus resource
var serviceBus = builder
    .AddAzureServiceBus("serviceBus")
    .RunAsEmulator(e =>
    {
        e.WithLifetime(ContainerLifetime.Persistent);
    });
serviceBus.AddServiceBusQueue("tasksQueue").WithTestCommands();

// Add API service
var api = builder
    .AddProject<Projects.Fair_Share_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(postgres)
    .WithReference(serviceBus)
    .WaitFor(postgres);

//Add Azure Functions service
var functions = builder
    .AddAzureFunctionsProject<Projects.Fair_Share_Functions>("fair-share-functions")
    .WithReference(postgres)
    .WithReference(serviceBus)
    .WaitFor(serviceBus);

// Add the ASB Emulator UI
builder.AddAsbEmulatorUi("asb-ui", serviceBus);

builder.Build().Run();
