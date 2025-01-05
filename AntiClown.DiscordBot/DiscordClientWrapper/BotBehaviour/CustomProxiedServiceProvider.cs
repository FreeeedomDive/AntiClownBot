using Castle.DynamicProxy;

namespace AntiClown.DiscordBot.DiscordClientWrapper.BotBehaviour;

public class CustomProxiedServiceProvider : IServiceProvider
{
    public CustomProxiedServiceProvider(
        IServiceProvider serviceProvider,
        ProxyGenerator proxyGenerator,
        ILogger<CustomProxiedServiceProvider> logger
    )
    {
        this.serviceProvider = serviceProvider;
        this.proxyGenerator = proxyGenerator;
        this.logger = logger;
    }

    public object? GetService(Type serviceType)
    {
        logger.LogInformation("Requested service: {serviceName}", serviceType.Name);
        var builtService = serviceProvider.GetRequiredService(serviceType);
        try
        {
            var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateClassProxyWithTarget(builtService.GetType(), builtService, interceptors);
        }
        catch(Exception ex)
        {
            // return built service without proxy to prevent crash
            logger.LogError(ex, "Failed to build slash command with proxy");
            return builtService;
        }
    }

    private readonly IServiceProvider serviceProvider;
    private readonly ProxyGenerator proxyGenerator;
    private readonly ILogger<CustomProxiedServiceProvider> logger;
}