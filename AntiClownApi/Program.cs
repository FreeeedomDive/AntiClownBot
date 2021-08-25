using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AntiClownBotApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Migration.MigrationProcess.StartMigration();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}