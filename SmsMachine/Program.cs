using SmsMachine.Infrastructure;
using SmsMachine.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new AreaSxOptions
{
    Password = builder.Configuration["AreaSx:Password"]
});
builder.Services.AddTransient<ISmsReceiver, AreaSxSmsReceiver>();
builder.Services.AddTransient<ISmsSender, AreaSxSmsSender>();
builder.Services.AddHttpClient<AreaSxSmsSender>(client =>
{
    var baseUrl = builder.Configuration["AreaSx:BaseUrl"];
    client.BaseAddress=new Uri(baseUrl);
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
