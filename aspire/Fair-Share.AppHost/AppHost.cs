using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var acaEnv = builder.AddAzureContainerAppEnvironment("env");

// Add database resource
var postgres = builder
    .AddAzurePostgresFlexibleServer("postgres")
    .WithPasswordAuthentication()
    // Below only affects for development purposes to run as db as container.
    .RunAsContainer(x =>
        x.WithDataVolume()
            .WithPgAdmin(x => x.WithLifetime(ContainerLifetime.Persistent))
            .WithLifetime(ContainerLifetime.Persistent)
    );
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
    .WithReference(postgresdb)
    .WithReference(serviceBus)
    .WaitFor(serviceBus)
    .WaitFor(postgresdb)
    .PublishAsAzureContainerApp(
        (infrasture, app) =>
        {
            app.Template.Scale.MinReplicas = 0;
            app.Template.Scale.MaxReplicas = 1;
        }
    );

//Add Azure Functions service
var functions = builder
    .AddAzureFunctionsProject<Projects.Fair_Share_Functions>("fair-share-functions")
    .WithReference(postgresdb)
    .WithReference(serviceBus)
    .WaitFor(serviceBus)
    .WaitFor(postgresdb)
    .PublishAsAzureContainerApp(
        (infrasture, app) =>
        {
            app.Template.Scale.MinReplicas = 0;
            app.Template.Scale.MaxReplicas = 1;
        }
    );

// Add the ASB Emulator UI
builder.AddAsbEmulatorUi("asb-ui", serviceBus);

builder.Build().Run();
