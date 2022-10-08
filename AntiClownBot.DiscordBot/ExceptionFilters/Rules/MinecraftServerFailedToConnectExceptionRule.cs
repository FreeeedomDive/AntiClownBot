namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public class MinecraftServerFailedToConnectExceptionRule : IExceptionFilterRule
{
    public bool Filter(Exception exception)
    {
        var result = exception
                         .StackTrace?
                         .Contains("AntiClownDiscordBotVersion2.ServicesHealth.ServicesHealthChecker.IsMinecraftServerOnline")
                     ?? false;
        Console.WriteLine($"{nameof(MinecraftServerFailedToConnectExceptionRule)} filter result is {result}");
        return result;
    }
}