using System;
using AntiClownBotApi.Converters;
using AntiClownBotApi.Database;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new BaseItemConverter()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Best API ever", Version = "v1"});
            });

            var connection = Configuration.GetConnectionString("PostgreSql");
            
            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(connection));
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
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }
    }
}