using System.Diagnostics;
using System.Diagnostics.Metrics;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Configuration;
using Serilog.Filters;
using Serilog.Sinks.OpenTelemetry;

namespace AntiClown.Core.OpenTelemetry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection serviceCollection,
        string fallbackServiceName,
        bool instrumentAspNetCore = true,
        Action<HttpClientTraceInstrumentationOptions>? configureHttpClientTracing = null
    )
    {
        serviceCollection.AddSingleton(_ => new ActivitySource(fallbackServiceName));
        serviceCollection.AddSingleton(_ => new Meter(fallbackServiceName));
        serviceCollection.AddProxies();

        if (!IsExportEnabled())
        {
            return serviceCollection;
        }

        serviceCollection.AddOpenTelemetry()
                         .ConfigureResource(resource => ConfigureResource(resource, fallbackServiceName))
                         .WithTracing(
                             tracing =>
                             {
                                 if (instrumentAspNetCore)
                                 {
                                     tracing.AddAspNetCoreInstrumentation();
                                 }

                                 tracing
                                     .AddHttpClientInstrumentation(options => configureHttpClientTracing?.Invoke(options))
                                     .AddSource(NpgsqlActivitySourceName)
                                     .AddSource(fallbackServiceName)
                                     .AddOtlpExporter();
                             }
                         )
                         .WithMetrics(
                             metrics =>
                             {
                                 if (instrumentAspNetCore)
                                 {
                                     metrics.AddAspNetCoreInstrumentation();
                                 }

                                 metrics
                                     .AddHttpClientInstrumentation()
                                     .AddRuntimeInstrumentation()
                                     .AddMeter(fallbackServiceName)
                                     .AddView(
                                         HttpServerDurationMetricName,
                                         new ExplicitBucketHistogramConfiguration { Boundaries = DurationBuckets }
                                     )
                                     .AddView(
                                         HttpClientDurationMetricName,
                                         new ExplicitBucketHistogramConfiguration { Boundaries = DurationBuckets }
                                     )
                                     .AddOtlpExporter((_, reader) =>
                                         reader.TemporalityPreference = MetricReaderTemporalityPreference.Cumulative
                                     );
                             }
                         );

        return serviceCollection;
    }

    public static LoggerConfiguration WriteToOpenTelemetry(
        this LoggerSinkConfiguration sinkConfiguration,
        string fallbackServiceName
    )
    {
        return sinkConfiguration.Logger(logger => logger
            .Filter.ByExcluding(Matching.FromSource(OpenTelemetrySourcePrefix))
            .Filter.ByExcluding(Matching.FromSource(GrpcSourcePrefix))
            .WriteTo.OpenTelemetry(options => ConfigureSink(options, fallbackServiceName))
        );
    }

    public static bool IsExportEnabled()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EndpointVariable)))
        {
            return false;
        }

        var disabled = Environment.GetEnvironmentVariable(SdkDisabledVariable);
        return !string.Equals(disabled, "true", StringComparison.OrdinalIgnoreCase);
    }

    public static void StartOpenTelemetry(this IServiceProvider serviceProvider)
    {
        if (!IsExportEnabled())
        {
            return;
        }

        serviceProvider.GetRequiredService<TracerProvider>();
        serviceProvider.GetRequiredService<MeterProvider>();
    }

    private static IServiceCollection AddProxies(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(new ProxyGenerator());
        serviceCollection.AddSingleton<IInterceptor, OpenTelemetryTraceSpanWrapperInterceptor>();

        return serviceCollection;
    }

    private static void ConfigureSink(OpenTelemetrySinkOptions options, string fallbackServiceName)
    {
        options.IncludedData = IncludedData.TraceIdField
                               | IncludedData.SpanIdField
                               | IncludedData.SpecRequiredResourceAttributes
                               | IncludedData.MessageTemplateTextAttribute
                               | IncludedData.SourceContextAttribute;

        if (!ServiceNameComesFromEnvironment())
        {
            options.ResourceAttributes[ServiceNameAttribute] = fallbackServiceName;
        }
    }

    private static void ConfigureResource(ResourceBuilder resource, string fallbackServiceName)
    {
        if (!ServiceNameComesFromEnvironment())
        {
            resource.AddService(fallbackServiceName);
        }
    }

    private static bool ServiceNameComesFromEnvironment()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ServiceNameVariable)))
        {
            return true;
        }

        var attributes = Environment.GetEnvironmentVariable(ResourceAttributesVariable);
        return attributes is not null
               && attributes.Contains($"{ServiceNameAttribute}=", StringComparison.OrdinalIgnoreCase);
    }

    private const string EndpointVariable = "OTEL_EXPORTER_OTLP_ENDPOINT";
    private const string SdkDisabledVariable = "OTEL_SDK_DISABLED";
    private const string ServiceNameVariable = "OTEL_SERVICE_NAME";
    private const string ResourceAttributesVariable = "OTEL_RESOURCE_ATTRIBUTES";
    private const string ServiceNameAttribute = "service.name";

    private const string NpgsqlActivitySourceName = "Npgsql";
    private const string HttpServerDurationMetricName = "http.server.request.duration";
    private const string HttpClientDurationMetricName = "http.client.request.duration";

    private const string OpenTelemetrySourcePrefix = "OpenTelemetry";
    private const string GrpcSourcePrefix = "Grpc";

    private static readonly double[] DurationBuckets =
        [0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];

    public static IServiceCollection AddTransientWithProxy<TInterface, TImplementation>(this IServiceCollection serviceCollection)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        return serviceCollection.AddTransientWithProxy(typeof(TInterface), typeof(TImplementation));
    }

    public static IServiceCollection AddTransientWithProxy(this IServiceCollection serviceCollection, Type serviceType, Type implementationType)
    {
        serviceCollection.AddTransient(implementationType);
        serviceCollection.AddTransient(
            serviceType,
            serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var implementation = serviceProvider.GetRequiredService(implementationType);
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();

                return proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, implementation, interceptors);
            }
        );
        return serviceCollection;
    }

    public static IServiceCollection AddTransientWithProxy<TInterface>(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, TInterface> implementationFactory
    )
        where TInterface : class
    {
        serviceCollection.AddTransient(
            typeof(TInterface),
            serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var implementation = implementationFactory(serviceProvider);
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();

                return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), implementation, interceptors);
            }
        );
        return serviceCollection;
    }

    public static IServiceCollection AddSingletonWithProxy<TInterface, TImplementation>(this IServiceCollection serviceCollection)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        serviceCollection.AddSingleton<TImplementation>();
        serviceCollection.AddSingleton(
            typeof(TInterface),
            serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var implementation = serviceProvider.GetRequiredService<TImplementation>();
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();

                return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), implementation, interceptors);
            }
        );
        return serviceCollection;
    }

    public static IServiceCollection AddSingletonWithProxy<TInterface>(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, TInterface> implementationFactory
    )
        where TInterface : class
    {
        serviceCollection.AddSingleton(
            typeof(TInterface),
            serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var implementation = implementationFactory(serviceProvider);
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();

                return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), implementation, interceptors);
            }
        );
        return serviceCollection;
    }
}
