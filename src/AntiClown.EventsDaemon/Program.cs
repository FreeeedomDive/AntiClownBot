using AntiClown.Core.OpenTelemetry;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.EventsDaemon.Workers;
using AntiClown.EventsDaemon.Workers.F1Predictions;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddOpenTelemetryTracing(builder.Configuration);

builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));
builder.Services.Configure<AntiClownDataApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDataApi"));

builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(
    serviceProvider => AntiClownEntertainmentApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownEntertainmentApiConnectionOptions>>().Value.ServiceUrl)
);
builder.Services.AddTransient<IAntiClownDataApiClient>(
    serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
);

builder.Services.AddTransient<IF1RacesProvider, F1RacesProvider>();

var workersTypes = AppDomain.CurrentDomain.GetAssemblies()
                          .SelectMany(x => x.GetTypes())
                          .Where(x => typeof(IWorker).IsAssignableFrom(x) && !x.IsAbstract)
                          .ToArray();

foreach (var workerType in workersTypes)
{
    builder.Services.AddSingleton(typeof(IWorker), workerType);
}

var app = builder.Build();
var workers = app.Services.GetServices<IWorker>();
await Task.WhenAll(workers.Select(x => x.StartAsync()));