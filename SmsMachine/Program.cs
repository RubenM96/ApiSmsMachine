using Microsoft.EntityFrameworkCore;
using Serilog;
using SmsMachine.Api.Infrastructure.Database;
using SmsMachine.Api.Services;
using SmsMachine.Api.Services.Queries;
using SmsMachine.Infrastructure;
using SmsMachine.Infrastructure.Data;
using SmsMachine.Infrastructure.Repositories;
using SmsMachine.Interfaces;
using SmsMachine.Services;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURAZIONE DI BASE ( connection string, logging su file )
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/SmsSendLog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// REGISTRAZIONE INFRASTRUCTURE: DbContext + Repositories
builder.Services.AddDbContext<SmsDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ISmsOutboundRepository, SmsOutboundRepository>();
builder.Services.AddTransient<INotifyRepository, NotifyRepository>();
builder.Services.AddTransient<ISmsInboundRepository, SmsInboundRepository>();
builder.Services.AddTransient<ICampaignRepository, CampaignRepository>();


// CONFIGURAZIONE INTEGRAZIONE AREA SX (Opzioni + HttpClient)
builder.Services.AddSingleton(new AreaSxOptions
{
    Password = builder.Configuration["AreaSx:Password"]
});

builder.Services.AddHttpClient<ISmsSender, AreaSxSmsSender>(client =>
{
    var baseUrl = builder.Configuration["AreaSx:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient<ISmsDiscard, AreaSxSmsDiscard>(client =>
{
    var baseUrl = builder.Configuration["AreaSx:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
});

//  APPLICATION SERVICES & BACKGROUND SERVICES
builder.Services.AddHostedService<CheckSmsOutboundToSendHostedService>();
builder.Services.AddScoped<ICheckSmsOutboundToSend, CheckSmsOutboundToSend>();
builder.Services.AddHostedService<CampaignCompletionHostedService>();
builder.Services.AddScoped<ICampaignCompletionService, CampaignCompletionService>();

builder.Services.AddTransient<ISmsService, SmsService>();
builder.Services.AddTransient<ISmsReceiver, AreaSxSmsReceiver>();
builder.Services.AddTransient<INotifyService, NotifyService>();
builder.Services.AddTransient<ISmsInbound, SmsInboundService>();
builder.Services.AddTransient<ICampaignService, CampaignService>();
builder.Services.AddTransient<IDiscardSmsService, DiscardSmsService>();
builder.Services.AddTransient<ICampaignQueryService, CampaignQueryService>();


// ASP.NET CORE: Controllers, Swagger, ecc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// BUILD APP + INIZIALIZZAZIONE DATABASE
var app = builder.Build();

app.InizializeDatabase();

// CONFIGURAZIONE PIPELINE HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

