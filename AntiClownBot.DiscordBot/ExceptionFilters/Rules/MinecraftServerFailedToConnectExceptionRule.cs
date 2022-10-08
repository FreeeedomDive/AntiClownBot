namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public class MinecraftServerFailedToConnectExceptionRule : IExceptionFilterRule
{
    public bool Filter(Exception exception)
    {
        return exception
                   .StackTrace?
                   .Contains("TcpClient.CompleteConnectAsync(Task task)")
               ?? false;;
    }
}