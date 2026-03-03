using System.Diagnostics;
using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AntiClown.Core.OpenTelemetry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        var serilogConfigurationSection = configuration.GetSection("Serilog");
        var seqConfiguration = serilogConfigurationSection.GetSection("WriteTo").GetChildren().FirstOrDefault(x => x["Name"] == "Seq");
        if (seqConfiguration is null)
        {
            return serviceCollection;
        }

        var applicationName = serilogConfigurationSection.GetSection("Properties")["Application"]!;
        var seqConfigurationArgs = seqConfiguration.GetSection("Args");
        var seqEndpoint = seqConfigurationArgs["serverUrl"]!;
        var seqApiKey = seqConfigurationArgs["apiKey"]!;

        var tracingSource = new ActivitySource(applicationName);
        serviceCollection.AddSingleton(tracingSource);
        serviceCollection.AddProxies();
        serviceCollection.AddOpenTelemetry()
                         .ConfigureResource(r => r.AddService(applicationName))
                         .WithTracing(
                             tracing =>
                             {
                                 tracing
                                     .AddSource(applicationName)
                                     .AddAspNetCoreInstrumentation()
                                     .AddOtlpExporter(
                                         opt =>
                                         {
                                             opt.Endpoint = new Uri($"{seqEndpoint}/ingest/otlp/v1/traces");
                                             opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                                             opt.Headers = $"X-Seq-ApiKey={seqApiKey}";
                                         }
                                     );
                             }
                         );

        return serviceCollection;
    }

    private static IServiceCollection AddProxies(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(new ProxyGenerator());
        serviceCollection.AddSingleton<IInterceptor, OpenTelemetryTraceSpanWrapperInterceptor>();

        return serviceCollection;
    }

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