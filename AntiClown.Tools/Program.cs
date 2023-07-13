using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Client.Configuration;
using AntiClown.Tools.Args;
using AntiClown.Tools.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddSingleton<IArgsProvider>(new ArgsProvider(args));

var antiClownApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("ApiUrl").Value!;
var antiClownEntertainmentApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("EntertainmentApiUrl").Value!;

builder.Services.AddTransient<IAntiClownApiClient>(_ => AntiClownApiClientProvider.Build(antiClownApiServiceUrl));
builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(_ => AntiClownEntertainmentApiClientProvider.Build(antiClownEntertainmentApiServiceUrl));

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