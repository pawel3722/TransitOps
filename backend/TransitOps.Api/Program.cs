using Microsoft.EntityFrameworkCore;
using TransitOps.Api.Data;
using TransitOps.Api.Services;
using TransitOps.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<TransitOpsDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDisruptionService, DisruptionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddHttpClient<IAiAnalysisService, GeminiAnalysisService>(
    client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    });

builder.Services.AddScoped<
    IDisruptionAnalysisService,
    DisruptionAnalysisService>();

var app = builder.Build();

app.UseCors("Frontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
