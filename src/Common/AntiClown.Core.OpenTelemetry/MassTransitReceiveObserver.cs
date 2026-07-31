using System.Diagnostics;
using System.Diagnostics.Metrics;
using MassTransit;

namespace AntiClown.Core.OpenTelemetry;

internal sealed class MassTransitReceiveObserver(Meter meter) : IReceiveObserver
{
    private readonly Counter<long> consumed = meter.CreateCounter<long>("anticlown.messaging.consumed");
    private readonly Counter<long> deadLettered = meter.CreateCounter<long>("anticlown.messaging.dead_lettered");
    private readonly Histogram<double> messageAge = meter.CreateHistogram<double>("anticlown.messaging.message.age", "s");

    public Task PreReceive(ReceiveContext context)
    {
        return Task.CompletedTask;
    }

    public Task PostReceive(ReceiveContext context)
    {
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        where T : class
    {
        RecordConsumed(consumerType, "success");
        RecordMessageAge(context, consumerType);
        deadLettered.Add(0, new TagList { { "consumer", consumerType } });
        return Task.CompletedTask;
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        where T : class
    {
        RecordConsumed(consumerType, "error");
        RecordMessageAge(context, consumerType);
        deadLettered.Add(1, new TagList { { "consumer", consumerType } });
        return Task.CompletedTask;
    }

    public Task ReceiveFault(ReceiveContext context, Exception exception)
    {
        deadLettered.Add(1, new TagList { { "consumer", GetEndpointName(context.InputAddress) } });
        return Task.CompletedTask;
    }

    private void RecordConsumed(string consumerType, string outcome)
    {
        var tags = new TagList
        {
            { "consumer", consumerType },
            { "outcome", outcome },
        };
        consumed.Add(1, tags);
    }

    private void RecordMessageAge<T>(ConsumeContext<T> context, string consumerType)
        where T : class
    {
        if (!context.SentTime.HasValue)
        {
            return;
        }

        var age = DateTime.UtcNow - context.SentTime.Value;
        messageAge.Record(Math.Max(0, age.TotalSeconds), new TagList { { "consumer", consumerType } });
    }

    private static string GetEndpointName(Uri inputAddress)
    {
        var path = inputAddress.AbsolutePath.Trim('/');
        var separator = path.LastIndexOf('/');
        return separator >= 0 ? path[(separator + 1)..] : path;
    }
}
