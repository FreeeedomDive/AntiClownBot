using System.Diagnostics;
using Castle.DynamicProxy;

namespace AntiClown.Core.OpenTelemetryTracing;

public class OpenTelemetryTraceSpanWrapperInterceptor : IInterceptor
{
    public OpenTelemetryTraceSpanWrapperInterceptor(ActivitySource activitySource)
    {
        this.activitySource = activitySource;
    }

    public void Intercept(IInvocation invocation)
    {
        using var activity = activitySource.StartActivity($"{invocation.Method.DeclaringType!.FullName}.{invocation.Method.Name}");
        invocation.Proceed();
        activity?.SetStatus(ActivityStatusCode.Ok);
    }

    private readonly ActivitySource activitySource;
}