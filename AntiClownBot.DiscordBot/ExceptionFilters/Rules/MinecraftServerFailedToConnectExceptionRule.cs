namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public class MinecraftServerFailedToConnectExceptionRule : IExceptionFilterRule
{
    public bool Filter(Exception exception)
    {
        return exception
                   .StackTrace?
                   .Contains("AntiClownDiscordBotVersion2.ServicesHealth.ServicesHealthChecker.IsMinecraftServerOnline")
               ?? false;
    }
}