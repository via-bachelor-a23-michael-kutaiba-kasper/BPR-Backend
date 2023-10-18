using EventManagementService.API.Settings;
using EventManagementService.Application;

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
        AppDomain.CurrentDomain.Load("EventManagementService.Application")));

builder.Services.AddApplicationServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors(options => { options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });

// poc pubsub 
var env = System.Environment.GetEnvironmentVariable("PUBSUB_EMULATOR_HOST");
Console.WriteLine(env);



// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();