using System.Diagnostics;
using AntiClown.EventsDaemon.Telemetry;

namespace AntiClown.EventsDaemon.Workers;

public abstract class FixedIntervalPeriodicJobWorker(ILogger logger, EventsDaemonTelemetry telemetry) : IWorker
{
    public async Task StartAsync()
    {
        var delay = await GetMillisecondsBeforeStartAsync();
        var initialDelay = TimeSpan.FromMilliseconds(delay);
        var firstScheduledAt = DateTimeOffset.UtcNow + initialDelay;
        Logger.LogInformation("{WorkerName} will start in {delay}", WorkerName, initialDelay);
        await Task.Delay(delay);

        currentIteration = 1;
        timer = new PeriodicTimer(IterationTime);
        var nextScheduledAt = DateTimeOffset.UtcNow + IterationTime;
        telemetry.RecordScheduleDrift(WorkerName, firstScheduledAt);
        await ExecuteIterationWithLogAsync();
        while (await timer.WaitForNextTickAsync())
        {
            currentIteration++;
            telemetry.RecordScheduleDrift(WorkerName, nextScheduledAt);
            do
            {
                nextScheduledAt += IterationTime;
            } while (nextScheduledAt <= DateTimeOffset.UtcNow);
            await ExecuteIterationWithLogAsync();
        }
    }

    private async Task ExecuteIterationWithLogAsync()
    {
        Logger.LogInformation("{WorkerName} Iteration {i} START at {startTime}", WorkerName, currentIteration, DateTime.UtcNow);
        var startedAt = Stopwatch.GetTimestamp();
        var succeeded = false;
        try
        {
            await ExecuteIterationAsync();
            succeeded = true;
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
        finally
        {
            telemetry.RecordRun(WorkerName, succeeded, Stopwatch.GetElapsedTime(startedAt));
        }
    }

    protected abstract Task<int> GetMillisecondsBeforeStartAsync();
    protected abstract Task ExecuteIterationAsync();

    protected ILogger Logger { get; } = logger;
    protected string WorkerName => this.GetType().Name;
    protected abstract TimeSpan IterationTime { get; set; }
    private int currentIteration;
    private int failedIterations;
    private int successfulIterations;

    private PeriodicTimer timer;
}
