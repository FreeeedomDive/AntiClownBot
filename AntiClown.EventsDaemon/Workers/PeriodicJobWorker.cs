namespace AntiClown.EventsDaemon.Workers;

public abstract class PeriodicJobWorker
{
    protected PeriodicJobWorker(ILogger logger, TimeSpan iterationTime)
    {
        this.iterationTime = iterationTime;
        Logger = logger;
    }

    public async Task StartAsync()
    {
        var delay = CalculateTimeBeforeStart();
        Logger.LogInformation("{WorkerName} will start in {delay}", WorkerName, TimeSpan.FromMilliseconds(delay));
        await Task.Delay(delay);

        var currentIteration = 1;
        timer = new PeriodicTimer(iterationTime);
        await ExecuteIterationWithLogAsync(currentIteration);
        while (await timer.WaitForNextTickAsync())
        {
            await ExecuteIterationWithLogAsync(++currentIteration);
        }
    }

    private async Task ExecuteIterationWithLogAsync(int currentIteration)
    {
        Logger.LogInformation("{WorkerName} starts iteration {i} at {startTime}", WorkerName, currentIteration, DateTime.UtcNow);
        try
        {
            await ExecuteIterationAsync();
            Logger.LogInformation("{WorkerName} succeeded iteration {i} at {startTime}", WorkerName, currentIteration, DateTime.UtcNow);
        }
        catch (Exception e)
        {
            Logger.LogError("{WorkerName} FAILED iteration {i} at {startTime}\n{exception}", WorkerName, currentIteration, DateTime.UtcNow, e);
        }
    }

    protected ILogger Logger { get; }
    protected abstract int CalculateTimeBeforeStart();
    protected abstract Task ExecuteIterationAsync();
    protected abstract string WorkerName { get; }

    private PeriodicTimer timer;
    private readonly TimeSpan iterationTime;
}