using TelemetryApp.Api.Client.Log;

namespace AntiClown.EventsDaemon.Workers;

public abstract class FixedIntervalPeriodicJobWorker : IWorker
{
    protected FixedIntervalPeriodicJobWorker(
        ILoggerClient logger
    )
    {
        Logger = logger;
    }

    public async Task StartAsync()
    {
        var delay = await GetMillisecondsBeforeStartAsync();
        await Logger.InfoAsync("{WorkerName} will start in {delay}", WorkerName, TimeSpan.FromMilliseconds(delay));
        await Task.Delay(delay);

        currentIteration = 1;
        timer = new PeriodicTimer(IterationTime);
        await ExecuteIterationWithLogAsync();
        while (await timer.WaitForNextTickAsync())
        {
            currentIteration++;
            await ExecuteIterationWithLogAsync();
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

    protected abstract Task<int> GetMillisecondsBeforeStartAsync();
    protected abstract Task ExecuteIterationAsync();

    protected ILoggerClient Logger { get; }
    protected string WorkerName => this.GetType().Name;
    protected abstract TimeSpan IterationTime { get; set; }
    private int currentIteration;
    private int failedIterations;
    private int successfulIterations;

    private PeriodicTimer timer;
}