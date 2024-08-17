using AntiClown.DiscordBot.SlashCommands.Base;
using Castle.DynamicProxy;

namespace AntiClown.DiscordBot.DiscordClientWrapper.BotBehaviour;

public class CustomProxiedServiceProvider : IServiceProvider
{
    public CustomProxiedServiceProvider(
        IServiceProvider serviceProvider,
        ProxyGenerator proxyGenerator
    )
    {
        this.serviceProvider = serviceProvider;
        this.proxyGenerator = proxyGenerator;
    }

    public object? GetService(Type serviceType)
    {
        var builtService = serviceProvider.GetRequiredService(serviceType);
        var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
        return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(SlashCommandModuleWithMiddlewares), builtService, interceptors);
    }

    private readonly IServiceProvider serviceProvider;
    private readonly ProxyGenerator proxyGenerator;
}