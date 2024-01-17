using System.Text.Json;
using FirebaseAdmin.Auth;
using RecommendationService.API.Settings;
using RecommendationService.Application;
using RecommendationService.Infrastructure;
using RecommendationService.Infrastructure.ApiGateway;
using RecommendationService.Infrastructure.AppSettings;

var builder = WebApplication.CreateBuilder(args);

// AppSettings configurations
builder.Services.AddAppSettingsConfigurations(builder.Configuration);
Gateway gatewayConfig = builder.Configuration.GetSection("Gateway").Get<Gateway>();

// Gateway Abstraction
// NOTE: Not able to access IOptions from infrastructure project, so this will suffice for now.
builder.Services.AddScoped<IApiGateway>(_ => new ApiGateway(gatewayConfig));


// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(
        AppDomain.CurrentDomain.Load("RecommendationService.Application")));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Register HTTP client here: 
builder.Services.AddHttpClient("HTTP_CLIENT")
    .ConfigureHttpClient((_, client) =>
    {
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.Timeout = TimeSpan.FromSeconds(30);
    });


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options => { options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });

app.UseAuthorization();

app.MapControllers();

app.Run();

