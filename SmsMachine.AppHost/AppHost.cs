var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose");

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("db");

var rabbitMq = builder.AddRabbitMQ("rabbitmq");

var api = builder.AddProject<Projects.SmsMachine_Api>("api")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.SmsMachine_Dashboard>("dashboard")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();