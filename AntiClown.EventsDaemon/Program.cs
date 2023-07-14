using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.EventsDaemon.Options;
using AntiClown.EventsDaemon.Workers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.Configure<CommonEventsWorkerOptions>(builder.Configuration.GetSection("CommonEventsWorker"));
builder.Services.Configure<DailyEventsWorkerOptions>(builder.Configuration.GetSection("DailyEventsWorker"));
builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));

builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(
    serviceProvider => AntiClownEntertainmentApiClientProvider.Build(serviceProvider.GetService<IOptions<AntiClownEntertainmentApiConnectionOptions>>()?.Value.ServiceUrl)
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