using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.Tools.Args;
using AntiClown.Tools.Tools;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddSingleton<IArgsProvider>(new ArgsProvider(args));

var antiClownApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("ApiUrl").Value!;
var antiClownEntertainmentApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("EntertainmentApiUrl").Value!;
var antiClownDataApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("DataApiUrl").Value!;

builder.Services.AddTransient<IAntiClownApiClient>(_ => AntiClownApiClientProvider.Build(antiClownApiServiceUrl));
builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(_ => AntiClownEntertainmentApiClientProvider.Build(antiClownEntertainmentApiServiceUrl));
builder.Services.AddTransient<IAntiClownDataApiClient>(_ => AntiClownDataApiClientProvider.Build(antiClownDataApiServiceUrl));

var toolsTypes = AppDomain.CurrentDomain.GetAssemblies()
                          .SelectMany(x => x.GetTypes())
                          .Where(x => typeof(ToolBase).IsAssignableFrom(x) && !x.IsAbstract)
                          .ToArray();

foreach (var toolType in toolsTypes)
{
    builder.Services.AddSingleton(typeof(ToolBase), toolType);
}

builder.Services.AddSingleton<IToolsProvider, ToolsProvider>();
builder.Services.AddSingleton<IToolsRunner, ToolsRunner>();

var app = builder.Build();

var toolsRunner = app.Services.GetService<IToolsRunner>()!;
await toolsRunner.RunAsync();