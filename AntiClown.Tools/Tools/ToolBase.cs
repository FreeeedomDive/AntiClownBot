using System.Diagnostics;

namespace AntiClown.Tools.Tools;

public abstract class ToolBase
{
    protected ToolBase(ILogger logger)
    {
        Logger = logger;
    }

    public async Task ExecuteAsync()
    {
        Logger.LogInformation("Start executing tool {Name}", Name);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            await RunAsync();
        }
        catch (Exception e)
        {
            Logger.LogError("Unexpected exception in tool {Name}:\n{Exception}", Name, e);
        }
        Logger.LogInformation("Tool {Name} was executed in {ms}ms", Name, stopwatch.ElapsedMilliseconds);
    }

    protected ILogger Logger { get; }

    protected abstract Task RunAsync();
    public abstract string Name { get; }
}