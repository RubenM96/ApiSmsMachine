using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose");

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

//var db = sql.AddDatabase("db");
var db = sql.AddDatabase("DefaultConnection");

var rabbitMq = builder.AddRabbitMQ("rabbitmq");


var api = builder.AddProject<Projects.SmsMachine_Api>("api")
    .WithReference(sql)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.SmsMachine_Dashboard>("dashboard")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();