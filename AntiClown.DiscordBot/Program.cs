using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Consumers.Events.Common;
using AntiClown.DiscordBot.Consumers.Events.Daily;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.DiscordClientWrapper.BotBehaviour;
using AntiClown.DiscordBot.EmbedBuilders.Transactions;
using AntiClown.DiscordBot.EmbedBuilders.Tributes;
using AntiClown.DiscordBot.Options;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.DiscordBot.SlashCommands.Base.Middlewares;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Bedge;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;
using AntiClown.Entertainment.Api.Dto.DailyEvents.ResetsAndPayments;
using DSharpPlus;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging();

        ConfigureOptions(builder);
        BuildApiClients(builder);
        BuildDiscordServices(builder);
        BuildDiscordCaches(builder);
        BuildSlashCommands(builder);
        BuildEmbedBuilders(builder);
        BuildCommonEventsConsumers(builder);
        BuildDailyEventsConsumers(builder);
        BuildMassTransit(builder);

        var app = builder.Build();

        var discordBotBehaviour = app.Services.GetRequiredService<IDiscordBotBehaviour>();
        await discordBotBehaviour.ConfigureAsync();

        var slashCommandsExecutor = app.Services.GetRequiredService<ICommandExecutor>();
        slashCommandsExecutor.AddMiddleware<LoggingMiddleware>();
        slashCommandsExecutor.AddMiddleware<DeferredMessageMiddleware>();
        slashCommandsExecutor.AddMiddleware<CorrectChatCommandUsageMiddleware>();

        var discordClientWrapper = app.Services.GetRequiredService<IDiscordClientWrapper>();

        await Task.WhenAll(app.RunAsync(), discordClientWrapper.StartDiscordAsync(), InitializeCachesAsync(app.Services));
    }

    private static void ConfigureOptions(WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection("Discord"));
        builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
        builder.Services.Configure<AntiClownApiConnectionOptions>(builder.Configuration.GetSection("AntiClownApi"));
        builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));
    }

    private static void BuildApiClients(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IAntiClownApiClient>(
            serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetService<IOptions<AntiClownApiConnectionOptions>>()?.Value.ServiceUrl)
        );
        builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(
            serviceProvider => AntiClownEntertainmentApiClientProvider.Build(serviceProvider.GetService<IOptions<AntiClownEntertainmentApiConnectionOptions>>()?.Value.ServiceUrl)
        );
    }

    private static void BuildDiscordServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<DiscordClient>(
            serviceProvider =>
            {
                var settings = serviceProvider.GetService<IOptions<Settings>>()!.Value;
                var discordOptions = serviceProvider.GetService<IOptions<DiscordOptions>>()!.Value;
                return new DiscordClient(
                    new DiscordConfiguration
                    {
                        Token = discordOptions.ApiToken,
                        TokenType = TokenType.Bot,
                        MinimumLogLevel = Enum.TryParse<LogLevel>(settings.LogLevel, out var level) ? level : LogLevel.Information,
                        Intents = DiscordIntents.All,
                    }
                );
            }
        );
        builder.Services.AddTransient<IDiscordClientWrapper, DiscordClientWrapper.DiscordClientWrapper>();
        builder.Services.AddTransient<IDiscordBotBehaviour, DiscordBotBehaviour>();
    }

    private static void BuildDiscordCaches(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IUsersCache, UsersCache>();
        builder.Services.AddSingleton<IEmotesCache, EmotesCache>();
    }

    private static void BuildSlashCommands(WebApplicationBuilder builder)
    {
        var middlewareTypes = AppDomain.CurrentDomain.GetAssemblies()
                                       .SelectMany(s => s.GetTypes())
                                       .Where(p => typeof(ICommandMiddleware).IsAssignableFrom(p) && !p.IsInterface);
        foreach (var middlewareType in middlewareTypes)
        {
            builder.Services.AddTransient(middlewareType);
            builder.Services.AddTransient(typeof(ICommandMiddleware), middlewareType);
        }

        builder.Services.AddSingleton<ICommandExecutor, CommandExecutor>();
    }

    private static void BuildEmbedBuilders(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITributeEmbedBuilder, TributeEmbedBuilder>();
        builder.Services.AddTransient<ITransactionsEmbedBuilder, TransactionsEmbedBuilder>();
    }

    private static void BuildCommonEventsConsumers(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICommonEventConsumer<GuessNumberEventDto>, GuessNumberEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<LotteryEventDto>, LotteryEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<RaceEventDto>, RaceEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<RemoveCoolDownsEventDto>, RemoveCoolDownsEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<TransfusionEventDto>, TransfusionEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<BedgeEventDto>, BedgeEventConsumer>();
    }

    private static void BuildDailyEventsConsumers(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IDailyEventConsumer<AnnounceEventDto>, AnnounceEventConsumer>();
        builder.Services.AddTransient<IDailyEventConsumer<ResetsAndPaymentsEventDto>, ResetsAndPaymentsConsumer>();
    }

    private static void BuildMassTransit(WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(
            massTransitConfiguration =>
            {
                massTransitConfiguration.AddConsumers(AppDomain.CurrentDomain.GetAssemblies());
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
    }

    private static async Task InitializeCachesAsync(IServiceProvider serviceProvider)
    {
        await Task.Delay(5 * 1000);

        var usersCache = serviceProvider.GetRequiredService<IUsersCache>();
        var emotesCache = serviceProvider.GetRequiredService<IEmotesCache>();

        await Task.WhenAll(usersCache.InitializeAsync(), emotesCache.InitializeAsync());
    }
}