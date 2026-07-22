using LegacyWebBridge.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Read Port from appsettings.json, environment variable, or command line (default to 12345)
var httpPort = builder.Configuration.GetValue<int>("Port", 12345);
builder.WebHost.UseUrls($"http://0.0.0.0:{httpPort}");

// 2. Add Services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 3. Configure Single-Port Middleware Pipeline
app.UseDefaultFiles(); 
app.UseStaticFiles(); 

app.UseRouting();
app.UseAuthorization();

// 4. Map API Endpoints
app.MapControllers();

// 5. Single Page Application (SPA) Fallback
app.MapFallbackToFile("index.html");

app.Run();
