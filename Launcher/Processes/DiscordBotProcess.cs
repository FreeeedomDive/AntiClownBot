using System;
using System.Diagnostics;

namespace Launcher.Processes
{
    public class DiscordBotProcess: IProcess
    {
        public string Name => "Discord Bot";

        public void Start()
        {
            Console.WriteLine("Creating discord bot process");
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "D:\\Projects\\AntiClownBot\\AntiClownDiscordBot\\bin\\Debug\\net5.0\\AntiClownDiscordBot.exe",
                    UseShellExecute = false, 
                    RedirectStandardOutput = true
                }
            };
            process.OutputDataReceived += (sender, args) => Console.WriteLine($"{Name}: {args.Data}");
            process.Start();
            process.BeginOutputReadLine();
            
            process.Start();
            process.WaitForExit();
        }
    }
}