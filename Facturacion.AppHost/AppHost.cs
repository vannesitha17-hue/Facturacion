
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Facturacion_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
