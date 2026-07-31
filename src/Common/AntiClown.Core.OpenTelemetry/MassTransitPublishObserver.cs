using System.Diagnostics;
using System.Diagnostics.Metrics;
using MassTransit;

namespace AntiClown.Core.OpenTelemetry;

internal sealed class MassTransitPublishObserver(Meter meter) : IPublishObserver
{
    private readonly Counter<long> published = meter.CreateCounter<long>("anticlown.messaging.published");
    private readonly string producer = meter.Name;

    public Task PrePublish<T>(PublishContext<T> context)
        where T : class
    {
        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context)
        where T : class
    {
        Record("success");
        return Task.CompletedTask;
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception)
        where T : class
    {
        Record("error");
        return Task.CompletedTask;
    }

    private void Record(string outcome)
    {
        var tags = new TagList
        {
            { "producer", producer },
            { "outcome", outcome },
        };
        published.Add(1, tags);
    }
}
