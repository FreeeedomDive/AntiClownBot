using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;

namespace AntiClown.EventsDaemon.Telemetry;

public sealed class EventsDaemonTelemetry
{
    public EventsDaemonTelemetry(Meter meter)
    {
        runs = meter.CreateCounter<long>(RunsName, "{run}");
        duration = meter.CreateHistogram<double>(DurationName, "s");
        lastSuccess = meter.CreateGauge<double>(LastSuccessName, "s");
        scheduleDrift = meter.CreateHistogram<double>(ScheduleDriftName, "s");
    }

    public static void ConfigureMetrics(MeterProviderBuilder metrics)
    {
        metrics
            .AddView(DurationName, new ExplicitBucketHistogramConfiguration { Boundaries = DurationBuckets })
            .AddView(ScheduleDriftName, new ExplicitBucketHistogramConfiguration { Boundaries = DriftBuckets });
    }

    public void RecordRun(string worker, bool succeeded, TimeSpan elapsed)
    {
        var runTags = new TagList
        {
            { "worker", worker },
            { "result", succeeded ? Success : Error },
        };
        runs.Add(1, runTags);

        var workerTags = new TagList
        {
            { "worker", worker },
        };
        duration.Record(elapsed.TotalSeconds, workerTags);

        if (succeeded)
        {
            lastSuccess.Record(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d, workerTags);
        }
    }

    public void RecordScheduleDrift(string worker, DateTimeOffset scheduledAt)
    {
        var tags = new TagList
        {
            { "worker", worker },
        };
        var driftSeconds = Math.Max((DateTimeOffset.UtcNow - scheduledAt).TotalSeconds, 0);
        scheduleDrift.Record(driftSeconds, tags);
    }

    public const string RunsName = "anticlown.events.worker.runs";
    public const string DurationName = "anticlown.events.worker.duration";
    public const string LastSuccessName = "anticlown.events.worker.last.success.timestamp.seconds";
    public const string ScheduleDriftName = "anticlown.events.worker.schedule.drift";

    private const string Success = "success";
    private const string Error = "error";

    private static readonly double[] DurationBuckets =
        [0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];

    private static readonly double[] DriftBuckets =
        [1, 5, 10, 30, 60, 120, 300, 600, 1800, 3600];

    private readonly Histogram<double> duration;
    private readonly Gauge<double> lastSuccess;
    private readonly Counter<long> runs;
    private readonly Histogram<double> scheduleDrift;
}
