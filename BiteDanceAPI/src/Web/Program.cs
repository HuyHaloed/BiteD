using BiteDanceAPI.Application;
using BiteDanceAPI.Infrastructure;
using BiteDanceAPI.Infrastructure.Data;
using BiteDanceAPI.Web;
using NSwag.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Destructure.ToMaximumDepth(3)
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Day) // note: log storage
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // hoặc ListenLocalhost nếu chỉ dùng local
});


// Add services to the container.
builder.Services.AddKeyVaultIfConfigured(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting(); 

// Use CORS middleware
app.UseCors();

app.MapControllers();

app.MapRazorPages();

app.MapEndpoints();

app.MapFallbackToFile("index.html");


// Configure the HTTP request pipeline.
await app.InitialiseDatabaseAsync(); // TODO: move to dev only
if (app.Environment.IsDevelopment()) { }
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");

app.UseSwaggerUi(settings =>
{
    settings.Path = "/swagger";
    settings.DocumentPath = "/swagger/specification.json";
    settings.OAuth2Client = new OAuth2ClientSettings()
    {
        ClientId = app.Configuration.GetSection("AzureAdSpa:ClientId").Value
    };
});

app.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");

app.UseExceptionHandler(options => { });

//app.Map("/", () => Results.Redirect("/swagger"));
app.Run();

namespace BiteDanceAPI.Web
{
    public partial class Program;
}
