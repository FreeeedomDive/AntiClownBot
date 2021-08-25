using System;

namespace AntiClownBot
{
    static class Program
    {
        static void Main()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                var message = $"{eventArgs.Exception.Message}\n{eventArgs.Exception.StackTrace}";
                NLogWrapper.GetDefaultLogger().Error(message);
            };
            var app = new MainDiscordHandler();
        }
    }
}