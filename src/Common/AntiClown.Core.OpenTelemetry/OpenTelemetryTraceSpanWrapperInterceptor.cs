using System.Diagnostics;
using System.Reflection;
using Castle.DynamicProxy;

namespace AntiClown.Core.OpenTelemetry;

public class OpenTelemetryTraceSpanWrapperInterceptor : IInterceptor
{
    public OpenTelemetryTraceSpanWrapperInterceptor(ActivitySource activitySource)
    {
        this.activitySource = activitySource;
    }

    public void Intercept(IInvocation invocation)
    {
        var returnType = invocation.Method.ReturnType;

        if (returnType == typeof(Task))
        {
            invocation.ReturnValue = InterceptAsync(invocation);
            return;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            invocation.ReturnValue = InterceptAsyncWithResultMethod
                                     .MakeGenericMethod(returnType.GenericTypeArguments[0])
                                     .Invoke(this, [invocation]);
            return;
        }

        InterceptSynchronous(invocation);
    }

    private void InterceptSynchronous(IInvocation invocation)
    {
        using var activity = StartActivity(invocation);

        try
        {
            invocation.Proceed();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception exception)
        {
            RecordException(activity, exception);
            throw;
        }
    }

    private async Task InterceptAsync(IInvocation invocation)
    {
        using var activity = StartActivity(invocation);

        try
        {
            invocation.Proceed();
            await ((Task)invocation.ReturnValue!).ConfigureAwait(false);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception exception)
        {
            RecordException(activity, exception);
            throw;
        }
    }

    private async Task<TResult> InterceptAsyncWithResult<TResult>(IInvocation invocation)
    {
        using var activity = StartActivity(invocation);

        try
        {
            invocation.Proceed();
            var result = await ((Task<TResult>)invocation.ReturnValue!).ConfigureAwait(false);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception exception)
        {
            RecordException(activity, exception);
            throw;
        }
    }

    private Activity? StartActivity(IInvocation invocation)
    {
        return activitySource.StartActivity($"{invocation.Method.DeclaringType!.FullName}.{invocation.Method.Name}");
    }

    private static void RecordException(Activity? activity, Exception exception)
    {
        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity?.AddException(exception);
    }

    private readonly ActivitySource activitySource;

    private static readonly MethodInfo InterceptAsyncWithResultMethod =
        typeof(OpenTelemetryTraceSpanWrapperInterceptor)
            .GetMethod(nameof(InterceptAsyncWithResult), BindingFlags.Instance | BindingFlags.NonPublic)!;
}
