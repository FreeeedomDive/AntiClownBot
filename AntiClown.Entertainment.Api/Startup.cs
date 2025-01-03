using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Core.OpenTelemetry;
using AntiClown.Core.Schedules;
using AntiClown.Core.Serializers;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
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
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;
using AntiClown.Entertainment.Api.Core.Options;
using AntiClown.Entertainment.Api.Core.Parties.Repositories;
using AntiClown.Entertainment.Api.Core.Parties.Services;
using AntiClown.Entertainment.Api.Core.Parties.Services.Messages;
using AntiClown.Entertainment.Api.Middlewares;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SqlRepositoryBase.Configuration.Extensions;
using SqlRepositoryBase.Core.Options;
using TelemetryApp.Utilities.Extensions;
using TelemetryApp.Utilities.Middlewares;

namespace AntiClown.Entertainment.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOpenTelemetryTracing(Configuration);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // configure AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

        services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMQ"));
        services.Configure<AntiClownApiConnectionOptions>(Configuration.GetSection("AntiClownApi"));
        services.Configure<AntiClownDataApiConnectionOptions>(Configuration.GetSection("AntiClownDataApi"));
        var telemetryApiUrl = Configuration.GetSection("Telemetry").GetSection("ApiUrl").Value;
        var deployingEnvironment = Configuration.GetValue<string>("DeployingEnvironment");
        services.ConfigureTelemetryClientWithLogger(
            "AntiClownBot" + (string.IsNullOrEmpty(deployingEnvironment) ? "" : $"_{deployingEnvironment}"),
            "EntertainmentApi",
            telemetryApiUrl
        );

        // configure database
        services.ConfigureConnectionStringFromAppSettings(Configuration.GetSection("PostgreSql"))
                .ConfigureDbContextFactory(connectionString => new DatabaseContext(connectionString))
                .ConfigurePostgreSql();

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

        services.AddTransient<IJsonSerializer, NewtonsoftJsonSerializer>();

        // configure repositories
        services.AddTransientWithProxy<ICommonEventsRepository, CommonEventsRepository>();
        services.AddTransientWithProxy<IRaceTracksRepository, RaceTracksRepository>();
        services.AddTransientWithProxy<IRaceDriversRepository, RaceDriversRepository>();
        services.AddTransientWithProxy<ICommonActiveEventsIndexRepository, CommonActiveEventsIndexRepository>();
        services.AddTransientWithProxy<IDailyEventsRepository, DailyEventsRepository>();
        services.AddTransientWithProxy<IActiveDailyEventsIndexRepository, ActiveDailyEventsIndexRepository>();
        services.AddTransientWithProxy<IPartiesRepository, PartiesRepository>();
        services.AddTransientWithProxy<IF1RacesRepository, F1RacesRepository>();
        services.AddTransientWithProxy<IF1PredictionResultsRepository, F1PredictionResultsRepository>();
        services.AddTransientWithProxy<IF1PredictionTeamsRepository, F1PredictionTeamsRepository>();

        // configure other stuff
        services.AddTransientWithProxy<IAntiClownApiClient>(
            serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownApiConnectionOptions>>().Value.ServiceUrl)
        );
        services.AddTransientWithProxy<IAntiClownDataApiClient>(
            serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
        );
        services.AddTransientWithProxy<ICommonEventsMessageProducer, CommonEventsMessageProducer>();
        services.AddTransientWithProxy<IDailyEventsMessageProducer, DailyEventsMessageProducer>();
        services.AddTransientWithProxy<IPartiesMessageProducer, PartiesMessageProducer>();
        services.AddTransientWithProxy<IScheduler, HangfireScheduler>();
        services.AddTransientWithProxy<IRaceGenerator, RaceGenerator>();

        // configure services
        services.AddTransientWithProxy<IGuessNumberEventService, GuessNumberEventService>();
        services.AddTransientWithProxy<ILotteryService, LotteryService>();
        services.AddTransientWithProxy<IRemoveCoolDownsEventService, RemoveCoolDownsEventService>();
        services.AddTransientWithProxy<ITransfusionEventService, TransfusionEventService>();
        services.AddTransientWithProxy<IRaceService, RaceService>();
        services.AddTransientWithProxy<IBedgeService, BedgeService>();
        services.AddTransientWithProxy<IActiveEventsIndexService, ActiveEventsIndexService>();
        services.AddTransientWithProxy<IAnnounceEventService, AnnounceEventService>();
        services.AddTransientWithProxy<IPaymentsAndResetsService, PaymentsAndResetsService>();
        services.AddTransientWithProxy<IActiveDailyEventsIndexService, ActiveDailyEventsIndexService>();
        services.AddTransientWithProxy<IPartiesService, PartiesService>();
        services.AddTransientWithProxy<IF1PredictionsMessageProducer, F1PredictionsMessageProducer>();
        services.AddTransientWithProxy<IF1PredictionsService, F1PredictionsService>();
        services.AddTransientWithProxy<IF1PredictionsStatisticsService, F1PredictionsStatisticsService>();
        services.AddTransientWithProxy<IMinecraftAuthService, MinecraftAuthService>();
        services.AddTransientWithProxy<IMinecraftAccountRepository, MinecraftAccountRepository>();
        services.AddTransientWithProxy<IMinecraftAccountService, MinecraftAccountService>();

        // configure HangFire
        services.AddHangfire(
            (serviceProvider, config) =>
                config.UsePostgreSqlStorage(serviceProvider.GetRequiredService<IOptions<AppSettingsDatabaseOptions>>().Value.ConnectionString)
        );
        services.AddHangfireServer();

        services.AddControllers().AddNewtonsoftJson(
            options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
            }
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseHangfireDashboard();
    }

    public IConfiguration Configuration { get; }
}