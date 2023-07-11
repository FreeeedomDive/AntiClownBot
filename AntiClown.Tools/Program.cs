using AntiClown.Tools.Args;
using AntiClown.Tools.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddSingleton<IArgsProvider>(new ArgsProvider(args));

var toolsTypes = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(x => x.GetTypes())
    .Where(x => typeof(ITool).IsAssignableFrom(x) && !x.IsInterface)
    .ToArray();

foreach (var toolType in toolsTypes)
{
    builder.Services.AddSingleton(typeof(ITool), toolType);
}

builder.Services.AddSingleton<IToolsProvider, ToolsProvider>();
builder.Services.AddSingleton<IToolsRunner, ToolsRunner>();

var app = builder.Build();

var toolsRunner = app.Services.GetService<IToolsRunner>()!;
await toolsRunner.RunAsync();