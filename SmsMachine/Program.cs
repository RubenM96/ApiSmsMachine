using Microsoft.EntityFrameworkCore;
using Serilog;
using SmsMachine.Api.Services;
using SmsMachine.Infrastructure;
using SmsMachine.Infrastructure.Data;
using SmsMachine.Infrastructure.Repositories;
using SmsMachine.Interfaces;
using SmsMachine.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/SmsSendLog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddDbContext<SmsDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<ISmsOutboundRepository, SmsOutboundRepository>();
builder.Services.AddTransient<INotifyRepository, NotifyRepository>();
builder.Services.AddTransient<ISmsInboundRepository, SmsInboundRepository>();
builder.Services.AddTransient<ICampaignRepository, CampaignRepository>();
builder.Services.AddTransient<ISmsQueueRepository, SmsQueueRepository>();

// Add services to the container.
builder.Services.AddSingleton(new AreaSxOptions
{
    Password = builder.Configuration["AreaSx:Password"]
});

builder.Services.AddTransient<ISmsService, SmsService>();
builder.Services.AddTransient<ISmsReceiver, AreaSxSmsReceiver>();

builder.Services.AddTransient<INotifyService, NotifyService>();
builder.Services.AddTransient<ISmsInbound, SmsInboundService>();
builder.Services.AddTransient<ICampaignService, CampaignService>();
builder.Services.AddTransient<ISmsQueueService, SmsQueueService>();
builder.Services.AddTransient<IDiscardSmsService, DiscardSmsService>();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy
            .WithOrigins("https://localhost:7069")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

//gestire meglio questo con init db o in seed db
var app = builder.Build();
var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<SmsDbContext>();
db.Database.Migrate();

app.UseCors("AllowBlazor");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
