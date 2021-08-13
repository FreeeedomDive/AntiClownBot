using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiClownBotApi.Database;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Classes.Items;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AntiClownBotApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}