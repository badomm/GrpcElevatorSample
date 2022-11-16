
using ElevatorServer.ElevatorService;
using ElevatorServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});

builder.Services.Configure<ElevatorsOptions>(
    builder.Configuration.GetSection(ElevatorsOptions.ConfigSection));
builder.Services.AddSingleton<IElevatorComService, ElevatorComService>();
builder.Services.AddSingleton<IElevatorBrain, ElevatorBrain>();

builder.Services.AddHostedService<ElevatorRunnerBackgroundService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<ElevatorService>();

app.Run();