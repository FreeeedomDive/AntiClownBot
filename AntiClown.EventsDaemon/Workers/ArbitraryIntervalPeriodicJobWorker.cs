namespace AntiClown.EventsDaemon.Workers;

public abstract class ArbitraryIntervalPeriodicJobWorker(ILogger logger) : IWorker
{
    public async Task StartAsync()
    {
        currentIteration = 1;
        while (true)
        {
            var delay = await TryGetMillisecondsBeforeNextIterationAsync();
            if (delay is null)
            {
                return;
            }

            Logger.LogInformation("{WorkerName} will start iteration {iteration} in {delay}", WorkerName, currentIteration, delay.Value);
            await Task.Delay(delay.Value);
            await ExecuteIterationWithLogAsync();
            currentIteration++;
        }
    }

    private async Task ExecuteIterationWithLogAsync()
    {
        Logger.LogInformation("{WorkerName} Iteration {i} START at {startTime}", WorkerName, currentIteration, DateTime.UtcNow);
        try
        {
            await ExecuteIterationAsync();
            successfulIterations++;
            Logger.LogInformation(
                "{WorkerName} Iteration {i} SUCCESS at {startTime} ({success} succeeded, {failed} failed)",
                WorkerName,
                currentIteration,
                DateTime.UtcNow,
                successfulIterations,
                failedIterations
            );
        }
        catch (Exception e)
        {
            failedIterations++;
            Logger.LogError(
                "{WorkerName} Iteration {i} FAIL at {startTime} ({success} succeeded, {failed} failed)\n{exception}",
                WorkerName,
                currentIteration,
                DateTime.UtcNow,
                successfulIterations,
                failedIterations,
                e
            );
        }
    }

    protected abstract Task<TimeSpan?> TryGetMillisecondsBeforeNextIterationAsync();
    protected abstract Task ExecuteIterationAsync();

    protected ILogger Logger { get; } = logger;
    protected string WorkerName => GetType().Name;

    private int currentIteration;
    private int failedIterations;
    private int successfulIterations;
}