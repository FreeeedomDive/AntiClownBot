using AntiClown.DiscordBot.SlashCommands.Base;
using Castle.DynamicProxy;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.DiscordClientWrapper.BotBehaviour;

public class CustomProxiedServiceProvider : IServiceProvider
{
    public CustomProxiedServiceProvider(
        IServiceProvider serviceProvider,
        ProxyGenerator proxyGenerator,
        ILoggerClient loggerClient
    )
    {
        this.serviceProvider = serviceProvider;
        this.proxyGenerator = proxyGenerator;
        this.loggerClient = loggerClient;
    }

    public object? GetService(Type serviceType)
    {
        var builtService = serviceProvider.GetRequiredService(serviceType);
        try
        {
            var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateClassProxyWithTarget(serviceType, builtService, interceptors);
        }
        catch(Exception ex)
        {
            // return built service without proxy to prevent crash
            loggerClient.ErrorAsync(ex, "Failed to build slash command with proxy").GetAwaiter().GetResult();
            return builtService;
        }
    }

    private readonly IServiceProvider serviceProvider;
    private readonly ProxyGenerator proxyGenerator;
    private readonly ILoggerClient loggerClient;
}