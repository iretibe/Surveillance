using AspNetCoreRateLimit;
using Surveillance.ApiGateway.DTOs;

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

app.MapGet("/dashboard", async (IHttpClientFactory factory) =>
{
    var client = factory.CreateClient();

    var alerts = await client.GetFromJsonAsync<List<AlertDto>>("http://alert-api/alerts");
    var users = await client.GetFromJsonAsync<List<UserDto>>("http://identity-api/users");

    return new { alerts, users };
});

app.Run();
