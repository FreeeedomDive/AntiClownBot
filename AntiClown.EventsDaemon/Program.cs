using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.EventsDaemon.Workers;
using Microsoft.Extensions.Options;
using TelemetryApp.Utilities.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
var telemetryApiUrl = builder.Configuration.GetSection("Telemetry").GetSection("ApiUrl").Value;
var deployingEnvironment = builder.Configuration.GetValue<string>("DeployingEnvironment");
builder.Services.ConfigureTelemetryClientWithLogger(
    "AntiClownBot" + (string.IsNullOrEmpty(deployingEnvironment) ? "" : $"_{deployingEnvironment}"),
    "EventsDaemon",
    telemetryApiUrl
);

builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));
builder.Services.Configure<AntiClownDataApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDataApi"));

builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(
    serviceProvider => AntiClownEntertainmentApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownEntertainmentApiConnectionOptions>>().Value.ServiceUrl)
);
builder.Services.AddTransient<IAntiClownDataApiClient>(
    serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
);

var toolsTypes = AppDomain.CurrentDomain.GetAssemblies()
                          .SelectMany(x => x.GetTypes())
                          .Where(x => typeof(PeriodicJobWorker).IsAssignableFrom(x) && !x.IsAbstract)
                          .ToArray();

foreach (var toolType in toolsTypes)
{
    builder.Services.AddSingleton(typeof(PeriodicJobWorker), toolType);
}

var app = builder.Build();
var workers = app.Services.GetServices<PeriodicJobWorker>();
await Task.WhenAll(workers.Select(x => x.StartAsync()));