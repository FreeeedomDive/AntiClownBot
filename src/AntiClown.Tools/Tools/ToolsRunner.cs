using AntiClown.Tools.Args;

namespace AntiClown.Tools.Tools;

public class ToolsRunner : IToolsRunner
{
    public ToolsRunner(
        IToolsProvider toolsProvider,
        IArgsProvider argsProvider,
        ILogger<ToolsRunner> logger
    )
    {
        this.toolsProvider = toolsProvider;
        this.argsProvider = argsProvider;
        this.logger = logger;
    }

    public async Task RunAsync()
    {
        var toolName = GetToolName();
        var tool = toolsProvider.FindByName(toolName);
        if (tool is null)
        {
            var tools = toolsProvider.GetAll();
            var names = tools.Select(x => x.Name).ToArray();
            logger.LogError(
                "Can't find a tool with name \"{toolName}\"\nAvailable tools:\n{tools}", toolName,
                string.Join("\n", names)
            );
            return;
        }

        await tool.ExecuteAsync();
    }

    private string GetToolName()
    {
        try
        {
            return argsProvider.GetArgument("-tool");
        }
        catch (ArgumentWasNotProvidedException)
        {
            var tools = toolsProvider.GetAll();
            var names = tools.Select(x => x.Name).ToArray();
            logger.LogError(
                "Provide a name for tool to launch\nAvailable tools:\n{tools}",
                string.Join("\n", names)
            );
            throw;
        }
    }

    private readonly IArgsProvider argsProvider;
    private readonly ILogger<ToolsRunner> logger;

    private readonly IToolsProvider toolsProvider;
}