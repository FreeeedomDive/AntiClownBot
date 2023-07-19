using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Bedge;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Transfusion;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.Announce;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.Messages;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.PaymentsAndResets;
using AntiClown.Entertainment.Api.Core.Database;
using AntiClown.Entertainment.Api.Core.Options;
using AntiClown.Entertainment.Api.Core.Parties.Repositories;
using AntiClown.Entertainment.Api.Core.Parties.Services;
using AntiClown.Entertainment.Api.Core.Parties.Services.Messages;
using AntiClown.Entertainment.Api.Middlewares;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        services.Configure<DatabaseOptions>(Configuration.GetSection("PostgreSql"));
        services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMQ"));
        services.Configure<AntiClownApiConnectionOptions>(Configuration.GetSection("AntiClownApi"));

        // configure database
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
        services.ConfigurePostgreSql();

        services.AddMassTransit(
            massTransitConfiguration =>
            {
                massTransitConfiguration.SetKebabCaseEndpointNameFormatter();
                massTransitConfiguration.UsingRabbitMq(
                    (context, rabbitMqConfiguration) =>
                    {
                        var rabbitMqOptions = context.GetService<IOptions<RabbitMqOptions>>()!.Value;
                        rabbitMqConfiguration.ConfigureEndpoints(context);
                        rabbitMqConfiguration.Host(
                            rabbitMqOptions.Host, "/", hostConfiguration =>
                            {
                                hostConfiguration.Username(rabbitMqOptions.Login);
                                hostConfiguration.Password(rabbitMqOptions.Password);
                            }
                        );
                    }
                );
            }
        );

        // configure repositories
        services.AddTransient<ICommonEventsRepository, CommonEventsRepository>();
        services.AddTransient<IRaceTracksRepository, RaceTracksRepository>();
        services.AddTransient<IRaceDriversRepository, RaceDriversRepository>();
        services.AddTransient<ICommonActiveEventsIndexRepository, CommonActiveEventsIndexRepository>();
        services.AddTransient<IDailyEventsRepository, DailyEventsRepository>();
        services.AddTransient<IActiveDailyEventsIndexRepository, ActiveDailyEventsIndexRepository>();
        services.AddTransient<IPartiesRepository, PartiesRepository>();

        // configure other stuff
        services.AddTransient<IAntiClownApiClient>(
            serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetService<IOptions<AntiClownApiConnectionOptions>>()?.Value.ServiceUrl)
        );
        services.AddTransient<ICommonEventsMessageProducer, CommonEventsMessageProducer>();
        services.AddTransient<IDailyEventsMessageProducer, DailyEventsMessageProducer>();
        services.AddTransient<IPartiesMessageProducer, PartiesMessageProducer>();
        services.AddTransient<IScheduler, HangfireScheduler>();
        services.AddTransient<IRaceGenerator, RaceGenerator>();

        // configure services
        services.AddTransient<IGuessNumberEventService, GuessNumberEventService>();
        services.AddTransient<ILotteryService, LotteryService>();
        services.AddTransient<IRemoveCoolDownsEventService, RemoveCoolDownsEventService>();
        services.AddTransient<ITransfusionEventService, TransfusionEventService>();
        services.AddTransient<IRaceService, RaceService>();
        services.AddTransient<IBedgeService, BedgeService>();
        services.AddTransient<IActiveEventsIndexService, ActiveEventsIndexService>();
        services.AddTransient<IAnnounceEventService, AnnounceEventService>();
        services.AddTransient<IPaymentsAndResetsService, PaymentsAndResetsService>();
        services.AddTransient<IActiveDailyEventsIndexService, ActiveDailyEventsIndexService>();
        services.AddTransient<IPartiesService, PartiesService>();

        // configure HangFire
        services.AddHangfire(
            (serviceProvider, config) =>
                config.UsePostgreSqlStorage(serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString)
        );
        services.AddHangfireServer();

        services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.TypeNameHandling = TypeNameHandling.All);
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