using System;
using System.Linq;
using AntiClownBotApi.Commands;
using AntiClownBotApi.Converters;
using AntiClownBotApi.Database;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TelemetryApp.Utilities.Extensions;
using TelemetryApp.Utilities.Middlewares;

namespace AntiClownBotApi
{
    public class Startup
    {
        public const bool IsDevelopment = true;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var postgresSection = Configuration.GetSection("PostgreSql");
            services.Configure<DbOptions>(postgresSection);
            services.AddDbContext<DatabaseContext>(ServiceLifetime.Singleton, ServiceLifetime.Singleton);

            services
                .ConfigureLoggerClient("AntiClownBot", "AntiClownBot.Api")
                .ConfigureApiTelemetryClient("AntiClownBot", "AntiClownBot.Api");

            services.AddTransient<UserRepository>();
            services.AddTransient<ShopRepository>();
            services.AddTransient<ItemRepository>();
            services.AddTransient<GlobalState>();
            services.AddTransient<TributeService>();

            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommand).IsAssignableFrom(p))
                .Where(p => p != typeof(ICommand));

            foreach (var commandType in commandTypes)
                services.AddSingleton(typeof(ICommand), commandType);

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new BaseItemConverter()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Best API ever", Version = "v1"});
            });

            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(postgresSection["ConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() && IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AntiClownApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseWebSockets();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }
    }
}