using Microsoft.EntityFrameworkCore;
using SmsMachine.Infrastructure;
using SmsMachine.Infrastructure.Data;
using SmsMachine.Infrastructure.Repositories;
using SmsMachine.Interfaces;
using SmsMachine.Services;
using Serilog;

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

// Add services to the container.
builder.Services.AddSingleton(new AreaSxOptions
{
    Password = builder.Configuration["AreaSx:Password"]
});

builder.Services.AddTransient<ISmsService, SmsService>();
builder.Services.AddTransient<ISmsReceiver, AreaSxSmsReceiver>();

builder.Services.AddTransient<INotifyService, NotifyService>();
builder.Services.AddTransient<ISmsInbound, SmsInboundService>();

builder.Services.AddHttpClient<ISmsSender, AreaSxSmsSender>(client =>
{
    var baseUrl = builder.Configuration["AreaSx:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
