using System.Net.Sockets;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.ExceptionFilters.Rules;

public class MinecraftServerFailedToConnectExceptionRule : IExceptionFilterRule
{
    public bool Filter(Exception exception)
    {
        Console.WriteLine(JsonConvert.SerializeObject(exception));
        return exception is SocketException
               && exception.Message == "Подключение не установлено, т.к. конечный компьютер отверг запрос на подключение";
    }
}