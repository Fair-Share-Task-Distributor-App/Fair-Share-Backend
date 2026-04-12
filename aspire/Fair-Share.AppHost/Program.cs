using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Add database resource
var postgres = builder.AddConnectionString("DefaultConnection");

// Add API service
var api = builder
    .AddProject<Projects.Fair_Share_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
