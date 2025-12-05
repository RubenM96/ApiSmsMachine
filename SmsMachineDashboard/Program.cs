using SmsMachine.Dashboard.Components;
using SmsMachine.Dashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServiceDiscovery();
builder.Services.AddHttpClient<ICampaignService,CampaignService>(s => { s.BaseAddress = new Uri("https+http://api"); })
    .AddServiceDiscovery();

//builder.Services.AddScoped<CampaignService>();
//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri("https://api")
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
