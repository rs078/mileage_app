using MathApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<MathApi.Services.MathService>();
builder.Services.AddHttpClient<MathApi.Services.GasPriceService>();

// Register the DB — SQLite creates a single file called milecalc.db next to the exe.
builder.Services.AddDbContext<MileCalcDb>(opt =>
    opt.UseSqlite("Data Source=milecalc.db")
       .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

// ALLOWED_ORIGIN env var lets Render's frontend URL be set without rebuilding
// Locally, allow both 4200 and 4201 so either ng serve port works
var allowedOrigins = (Environment.GetEnvironmentVariable("ALLOWED_ORIGIN")
    ?? "http://localhost:4200,http://localhost:4201")
    .Split(',', StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

// On every startup, create the DB file + tables if they don't exist yet.
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<MileCalcDb>().Database.EnsureCreated();

app.UseCors();
app.MapControllers();

// Render injects PORT; fall back to 5000 locally
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
