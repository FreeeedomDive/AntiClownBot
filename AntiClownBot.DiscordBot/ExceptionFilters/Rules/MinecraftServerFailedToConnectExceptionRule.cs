namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public class MinecraftServerFailedToConnectExceptionRule : IExceptionFilterRule
{
    public bool Filter(Exception exception)
    {
        return exception.StackTrace?.Contains("System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.ThrowException") ?? false;
    }
}