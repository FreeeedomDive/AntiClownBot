using System.Net.Sockets;

namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public class MinecraftServerFailedToConnectExceptionRule : IExceptionFilterRule
{
    public bool Filter(Exception exception)
    {
        return exception is SocketException
               && exception.Message == "Подключение не установлено, т.к. конечный компьютер отверг запрос на подключение";
    }
}