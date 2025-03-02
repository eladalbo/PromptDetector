using Microsoft.Extensions.Configuration;
using PromptDetector.Data;
using PromptDetector.Domain.Models;
using PromptDetector.Domain.Repositories;
using PromptDetector.Domain.Services;
using PromptDetector.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
// Add configuration file and objects
IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    .Build();

var envConfig = new EnvConfig();
Configuration.Bind(envConfig);
builder.Services.AddSingleton(envConfig);

builder.Services.AddSingleton<IAuditLogsRepository, AuditLogsRepository>();
builder.Services.AddTransient<IAuditLogService, AuditLogService>();
builder.Services.AddTransient<IAiClient, OpenAiClient>();
builder.Services.AddTransient<IDetectService, DetectService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
