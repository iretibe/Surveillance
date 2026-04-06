using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication("Bearer").AddJwtBearer();

builder.Services.AddAuthorization();

// Rate Limiting
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

var app = builder.Build();

app.MapGet("/dashboard", async (HttpClient client) =>
{
    var alerts = await client.GetFromJsonAsync("/alerts");
    var users = await client.GetFromJsonAsync("/users");

    return new { alerts, users };
});

app.Run();
