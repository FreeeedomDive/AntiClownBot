using System.Diagnostics;
using AntiClown.Core.OpenTelemetry;
using Castle.DynamicProxy;
using FluentAssertions;
using NUnit.Framework;

namespace AntiClown.Core.OpenTelemetry.Tests;

[TestFixture]
[NonParallelizable]
public class OpenTelemetryTraceSpanWrapperInterceptorTests
{
    [Test]
    public async Task Intercept_WhenAsyncMethodThrowsAfterAwait_TracksFullDurationAndError()
    {
        var sourceName = $"{nameof(OpenTelemetryTraceSpanWrapperInterceptorTests)}-{Guid.NewGuid():N}";
        Activity? stoppedActivity = null;

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == sourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => stoppedActivity = activity,
        };
        ActivitySource.AddActivityListener(listener);

        using var activitySource = new ActivitySource(sourceName);
        var target = new AsyncFailingService();
        var interceptor = new OpenTelemetryTraceSpanWrapperInterceptor(activitySource);
        var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget<IAsyncFailingService>(target, interceptor);

        var invocationTask = proxy.FailAfterAwaitAsync();
        await target.Started;
        await Task.Delay(MinimumExpectedDuration);

        stoppedActivity.Should().BeNull("the intercepted operation is still awaiting its continuation");

        target.Release();
        Func<Task> invocation = async () => { await invocationTask; };
        await invocation.Should().ThrowAsync<InvalidOperationException>().WithMessage(ExpectedErrorMessage);

        stoppedActivity.Should().NotBeNull();
        stoppedActivity!.DisplayName.Should().Be(
            $"{typeof(IAsyncFailingService).FullName}.{nameof(IAsyncFailingService.FailAfterAwaitAsync)}"
        );
        stoppedActivity.Status.Should().Be(ActivityStatusCode.Error);
        stoppedActivity.Duration.Should().BeGreaterThanOrEqualTo(MinimumExpectedDuration);
    }

    public interface IAsyncFailingService
    {
        Task<int> FailAfterAwaitAsync();
    }

    public sealed class AsyncFailingService : IAsyncFailingService
    {
        public Task Started => started.Task;

        public async Task<int> FailAfterAwaitAsync()
        {
            started.SetResult();
            await continuation.Task;
            throw new InvalidOperationException(ExpectedErrorMessage);
        }

        public void Release()
        {
            continuation.SetResult();
        }

        private readonly TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource continuation = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private const string ExpectedErrorMessage = "Failure after await";
    private static readonly TimeSpan MinimumExpectedDuration = TimeSpan.FromMilliseconds(50);
}
