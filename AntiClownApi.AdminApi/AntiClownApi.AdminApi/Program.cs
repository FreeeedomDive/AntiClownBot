namespace AntiClownApi.AdminApi
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