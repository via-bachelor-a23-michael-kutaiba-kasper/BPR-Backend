using EventManagementService.API.Settings;
using EventManagementService.Application;
using EventManagementService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Options configs
builder.Services.AddSettingsConfigurations(builder.Configuration);

// Add services to the container.
builder.Services.AddScoped<IConnectionStringManager, ConnectionStringManager>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(
        AppDomain.CurrentDomain.Load("EventManagementService.Application")));

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


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();