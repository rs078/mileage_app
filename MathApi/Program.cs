var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<MathApi.Services.MathService>();

// ALLOWED_ORIGIN env var lets Render's frontend URL be set without rebuilding
var allowedOrigin = Environment.GetEnvironmentVariable("ALLOWED_ORIGIN") ?? "http://localhost:4200";

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.MapControllers();

// Render injects PORT; fall back to 5000 locally
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
