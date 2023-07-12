using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Client.Configuration;
using AntiClown.EventsDaemon.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

var antiClownApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("ApiUrl").Value!;
var antiClownEntertainmentApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("EntertainmentApiUrl").Value!;

builder.Services.AddTransient<IAntiClownApiClient>(_ => AntiClownApiClientProvider.Build(antiClownApiServiceUrl));
builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(_ => AntiClownEntertainmentApiClientProvider.Build(antiClownEntertainmentApiServiceUrl));

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