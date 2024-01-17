using System.Text.Json;
using RecommendationService.Infrastructure.AppSettings;
using UserManagementService.API.Settings;
using UserManagementService.Application;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.ApiGateway;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Gateway gatewayConfig = builder.Configuration.GetSection("Gateway").Get<Gateway>();

// Gateway Abstraction
// NOTE: Not able to access IOptions from infrastructure project, so this will suffice for now.
builder.Services.AddScoped<IApiGateway>(_ => new ApiGateway(gatewayConfig));

builder.Services.AddSettingsConfigurations(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(
        AppDomain.CurrentDomain.Load("UserManagementService.Application")));

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

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}


app.UseCors(options => { options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
