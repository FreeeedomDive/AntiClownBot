using System;
using System.Diagnostics;

namespace Launcher.Processes
{
    public class BackendProcess: IProcess
    {
        public string Name => "Server";

        public void Start()
        {
            Console.WriteLine("Creating main server process");
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "D:\\Projects\\AntiClownBot\\AntiClownApi\\bin\\Debug\\net5.0\\AntiClownApi.exe",
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