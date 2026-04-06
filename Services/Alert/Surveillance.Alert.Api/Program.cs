using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.Grafana.Loki;
using Surveillance.Alert.Application;
using Surveillance.Alert.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://loki:3100")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("alert-service"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("Saga")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://tempo:4317");
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Correlation ID
app.Use(async (context, next) =>
{
    var correlationId = Guid.NewGuid().ToString();

    context.TraceIdentifier = correlationId;

    LogContext.PushProperty("CorrelationId", correlationId);

    await next();
});

// Prometheus
app.MapPrometheusScrapingEndpoint();

app.MapControllers();

app.Run();
