using TelemetryApp.Api.Client.Log;

namespace AntiClown.EventsDaemon.Workers;

public abstract class PeriodicJobWorker
{
    protected PeriodicJobWorker(
        ILoggerClient logger,
        TimeSpan iterationTime
    )
    {
        this.iterationTime = iterationTime;
        Logger = logger;
    }

    public async Task StartAsync()
    {
        var delay = CalculateTimeBeforeStart();
        await Logger.InfoAsync("{WorkerName} will start in {delay}", WorkerName, TimeSpan.FromMilliseconds(delay));
        await Task.Delay(delay);

        currentIteration = 1;
        timer = new PeriodicTimer(iterationTime);
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

    protected abstract int CalculateTimeBeforeStart();
    protected abstract Task ExecuteIterationAsync();

    protected ILoggerClient Logger { get; }
    protected abstract string WorkerName { get; }
    private readonly TimeSpan iterationTime;
    private int currentIteration;
    private int failedIterations;
    private int successfulIterations;

    private PeriodicTimer timer;
}