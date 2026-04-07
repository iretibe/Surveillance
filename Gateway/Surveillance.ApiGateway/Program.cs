using AspNetCoreRateLimit;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Surveillance.ApiGateway.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "API Gateway")
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(builder.Configuration["Loki:Url"] ?? "http://loki:3100")
    .CreateLogger();

builder.Host.UseSerilog();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms<CustomTransformer>();

// JWT Bearer Authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

// Add authentication & authorization
builder.Services.AddAuthorization();

// Add rate limiting
builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        }
    };
});

// Add CORS
builder.Services.AddCorsPolicy(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri("http://alert-api:8080/health"), "Alert API")
    .AddUrlGroup(new Uri("http://identity-api:8080/health"), "Identity API")
    .AddUrlGroup(new Uri("http://notification-api:8080/health"), "Notification API")
    .AddRedis(builder.Configuration["Redis:ConnectionString"] ?? "redis:6379")
    .AddRabbitMQ(builder.Configuration["RabbitMQ:ConnectionString"] ?? "amqp://guest:guest@rabbitmq:5672");

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("api-gateway"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"] ?? "http://tempo:4317");
            });
    });

// Add HTTP client resilience
builder.Services.AddHttpClient()
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    });

var app = builder.Build();

app.MapGet("/dashboard", async (IHttpClientFactory factory) =>
{
    var client = factory.CreateClient();

    var alerts = await client.GetFromJsonAsync<List<AlertDto>>("http://alert-api/alerts");
    var users = await client.GetFromJsonAsync<List<UserDto>>("http://identity-api/users");

    return new { alerts, users };
});

app.Run();
