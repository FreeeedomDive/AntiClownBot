using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Core.Database;
using AntiClown.Entertainment.Api.Core.Options;
using AntiClown.Entertainment.Api.Middlewares;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Configuration.Extensions;

namespace AntiClown.Entertainment.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // configure AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

        var postgresSection = Configuration.GetSection("PostgreSql");
        services.Configure<DatabaseOptions>(postgresSection);
        var rabbitMqSection = Configuration.GetSection("RabbitMQ");

        // configure database
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
        services.ConfigurePostgreSql();

        services.AddMassTransit(massTransitConfiguration =>
        {
            massTransitConfiguration.UsingRabbitMq((context, rabbitMqConfiguration) =>
            {
                rabbitMqConfiguration.ConfigureEndpoints(context);
                rabbitMqConfiguration.Host(rabbitMqSection["Host"], "/", hostConfiguration =>
                {
                    hostConfiguration.Username(rabbitMqSection["Login"]);
                    hostConfiguration.Password(rabbitMqSection["Password"]);
                });
            });
        });

        // configure repositories
        services.AddTransient<ICommonEventsRepository, CommonEventsRepository>();
        
        // configure validators

        // configure other stuff
        services.AddTransient<IEventsMessageProducer, EventsMessageProducer>();
        services.AddTransient<IScheduler, HangfireScheduler>();

        // configure services
        services.AddTransient<IGuessNumberEventService, GuessNumberEventService>();
        services.AddTransient<ILotteryService, LotteryService>();
        services.AddTransient<IRemoveCoolDownsEventService, RemoveCoolDownsEventService>();

        // configure HangFire
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(postgresSection["ConnectionString"])
        );
        services.AddHangfireServer();

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();

        app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseHangfireDashboard();
    }

    public IConfiguration Configuration { get; }
}