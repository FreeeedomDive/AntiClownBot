using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Launcher.Processes;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var processes = new List<IProcess>
            {
                new BackendProcess(),
                new DiscordBotProcess()
            };
            foreach (var process in processes)
            {
                Console.WriteLine($"Starting {process.Name}");
                Task.Run(() =>
                {
                    process.Start();
                    Console.WriteLine($"{process.Name} has started!");
                    Thread.Sleep(5000);
                });
            }
        }
    }
}