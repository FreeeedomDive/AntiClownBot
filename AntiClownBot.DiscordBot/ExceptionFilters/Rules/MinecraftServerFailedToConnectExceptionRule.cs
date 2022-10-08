using System.Net.Sockets;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public class MinecraftServerFailedToConnectExceptionRule : IExceptionFilterRule
{
    public bool Filter(Exception exception)
    {
        return exception is SocketException
               && (exception.StackTrace?.Contains("TcpClient.CompleteConnectAsync") ?? false);
    }
}