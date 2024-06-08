using TelemetryApp.Api.Client.Log;

namespace AntiClown.EventsDaemon.Workers;

public abstract class ArbitraryIntervalPeriodicJobWorker : IWorker
{
    public ArbitraryIntervalPeriodicJobWorker(
        ILoggerClient logger
    )
    {
        Logger = logger;
    }

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

            await Logger.InfoAsync("{WorkerName} will start iteration {iteration} in {delay}", WorkerName, currentIteration, TimeSpan.FromMilliseconds(delay.Value));
            await Task.Delay(delay.Value);
            await ExecuteIterationWithLogAsync();
            currentIteration++;
        }
    }

    private async Task ExecuteIterationWithLogAsync()
    {
        await Logger.InfoAsync("{WorkerName} Iteration {i} START at {startTime}", WorkerName, currentIteration, DateTime.UtcNow);
        try
        {
            await ExecuteIterationAsync();
            successfulIterations++;
            await Logger.InfoAsync(
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
            await Logger.ErrorAsync(
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

    protected abstract Task<int?> TryGetMillisecondsBeforeNextIterationAsync();
    protected abstract Task ExecuteIterationAsync();

    protected ILoggerClient Logger { get; }
    protected string WorkerName => GetType().Name;

    private int currentIteration;
    private int failedIterations;
    private int successfulIterations;
}