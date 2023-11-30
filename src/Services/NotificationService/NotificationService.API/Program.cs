using NotificationService.API.Settings;
using NotificationService.Application;
using NotificationService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Options configs
builder.Services.AddSettingsConfigurations(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblies(
        AppDomain.CurrentDomain.Load("NotificationService.Application")));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

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
