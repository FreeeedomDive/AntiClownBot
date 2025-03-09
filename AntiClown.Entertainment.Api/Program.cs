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
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Teams;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Statistics;
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
using Serilog;
using SqlRepositoryBase.Configuration.Extensions;
using SqlRepositoryBase.Core.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddOpenTelemetryTracing(builder.Configuration);
var assemblies = AppDomain.CurrentDomain.GetAssemblies();

// configure AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<AntiClownApiConnectionOptions>(builder.Configuration.GetSection("AntiClownApi"));
builder.Services.Configure<AntiClownDataApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDataApi"));

// configure database
builder.Services.ConfigureConnectionStringFromAppSettings(builder.Configuration.GetSection("PostgreSql"))
        .ConfigureDbContextFactory(connectionString => new DatabaseContext(connectionString))
        .ConfigurePostgreSql();

builder.Services.AddMassTransit(
    massTransitConfiguration =>
    {
        massTransitConfiguration.SetKebabCaseEndpointNameFormatter();
        massTransitConfiguration.UsingRabbitMq(
            (context, rabbitMqConfiguration) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
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

builder.Services.AddTransient<IJsonSerializer, NewtonsoftJsonSerializer>();

// configure repositories
builder.Services.AddTransientWithProxy<ICommonEventsRepository, CommonEventsRepository>();
builder.Services.AddTransientWithProxy<IRaceTracksRepository, RaceTracksRepository>();
builder.Services.AddTransientWithProxy<IRaceDriversRepository, RaceDriversRepository>();
builder.Services.AddTransientWithProxy<ICommonActiveEventsIndexRepository, CommonActiveEventsIndexRepository>();
builder.Services.AddTransientWithProxy<IDailyEventsRepository, DailyEventsRepository>();
builder.Services.AddTransientWithProxy<IActiveDailyEventsIndexRepository, ActiveDailyEventsIndexRepository>();
builder.Services.AddTransientWithProxy<IPartiesRepository, PartiesRepository>();
builder.Services.AddTransientWithProxy<IF1RacesRepository, F1RacesRepository>();
builder.Services.AddTransientWithProxy<IF1PredictionResultsRepository, F1PredictionResultsRepository>();
builder.Services.AddTransientWithProxy<IF1PredictionTeamsRepository, F1PredictionTeamsRepository>();
builder.Services.AddTransientWithProxy<IF1BingoCardsRepository, F1BingoCardsRepository>();
builder.Services.AddTransientWithProxy<IF1BingoBoardsRepository, F1BingoBoardsRepository>();

// configure other stuff
builder.Services.AddTransientWithProxy<IAntiClownApiClient>(
    serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownApiConnectionOptions>>().Value.ServiceUrl)
);
builder.Services.AddTransientWithProxy<IAntiClownDataApiClient>(
    serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
);
builder.Services.AddTransientWithProxy<ICommonEventsMessageProducer, CommonEventsMessageProducer>();
builder.Services.AddTransientWithProxy<IDailyEventsMessageProducer, DailyEventsMessageProducer>();
builder.Services.AddTransientWithProxy<IPartiesMessageProducer, PartiesMessageProducer>();
builder.Services.AddTransientWithProxy<IScheduler, HangfireScheduler>();
builder.Services.AddTransientWithProxy<IRaceGenerator, RaceGenerator>();

// configure services
builder.Services.AddTransientWithProxy<IGuessNumberEventService, GuessNumberEventService>();
builder.Services.AddTransientWithProxy<ILotteryService, LotteryService>();
builder.Services.AddTransientWithProxy<IRemoveCoolDownsEventService, RemoveCoolDownsEventService>();
builder.Services.AddTransientWithProxy<ITransfusionEventService, TransfusionEventService>();
builder.Services.AddTransientWithProxy<IRaceService, RaceService>();
builder.Services.AddTransientWithProxy<IBedgeService, BedgeService>();
builder.Services.AddTransientWithProxy<IActiveEventsIndexService, ActiveEventsIndexService>();
builder.Services.AddTransientWithProxy<IAnnounceEventService, AnnounceEventService>();
builder.Services.AddTransientWithProxy<IPaymentsAndResetsService, PaymentsAndResetsService>();
builder.Services.AddTransientWithProxy<IActiveDailyEventsIndexService, ActiveDailyEventsIndexService>();
builder.Services.AddTransientWithProxy<IPartiesService, PartiesService>();
builder.Services.AddTransientWithProxy<IF1PredictionsMessageProducer, F1PredictionsMessageProducer>();
builder.Services.AddTransientWithProxy<IF1PredictionsResultBuilder, F1PredictionsResultBuilder>();
builder.Services.AddTransientWithProxy<IF1PredictionsService, F1PredictionsService>();
builder.Services.AddTransientWithProxy<IF1PredictionsStatisticsService, F1PredictionsStatisticsService>();
builder.Services.AddTransientWithProxy<IMinecraftAuthService, MinecraftAuthService>();
builder.Services.AddTransientWithProxy<IMinecraftAccountRepository, MinecraftAccountRepository>();
builder.Services.AddTransientWithProxy<IMinecraftAccountService, MinecraftAccountService>();
builder.Services.AddTransientWithProxy<IF1BingoBoardsService, F1BingoBoardsService>();
builder.Services.AddTransientWithProxy<IF1BingoCardsService, F1BingoCardsService>();

// configure HangFire
builder.Services.AddHangfire(
    (serviceProvider, config) =>
        config.UsePostgreSqlStorage(serviceProvider.GetRequiredService<IOptions<AppSettingsDatabaseOptions>>().Value.ConnectionString)
);
builder.Services.AddHangfireServer();

builder.Services.AddControllers().AddNewtonsoftJson(
    options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
    }
);

var app = builder.Build();
app.UseHttpsRedirection();

app.UseRouting();
app.UseWebSockets();

app.UseSerilogRequestLogging();
app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.UseHangfireDashboard();

await app.RunAsync();