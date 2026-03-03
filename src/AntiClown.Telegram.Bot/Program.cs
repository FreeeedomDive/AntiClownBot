using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.DiscordBot.Client;
using AntiClown.DiscordBot.Client.Configuration;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.Telegram.Bot.Caches.Users;
using AntiClown.Telegram.Bot.Interactivity.Parties;
using AntiClown.Telegram.Bot.Options;
using AntiClown.Telegram.Bot.TelegramWorker;
using AntiClown.TelegramBot.Options;
using AntiClown.TelegramBot.TelegramWorker;
using MassTransit;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddMassTransit(
    massTransitConfiguration =>
    {
        massTransitConfiguration.AddConsumers(AppDomain.CurrentDomain.GetAssemblies());
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

var telegramSettingsSection = builder.Configuration.GetRequiredSection("Telegram");
builder.Services.Configure<TelegramSettings>(telegramSettingsSection);

builder.Services.Configure<AntiClownApiConnectionOptions>(builder.Configuration.GetSection("AntiClownApi"));
builder.Services.AddTransient<IAntiClownApiClient>(
    serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownApiConnectionOptions>>().Value.ServiceUrl)
);

builder.Services.Configure<AntiClownDataApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDataApi"));
builder.Services.AddTransient<IAntiClownDataApiClient>(
    serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
);

builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));
builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(
    serviceProvider => AntiClownEntertainmentApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownEntertainmentApiConnectionOptions>>().Value.ServiceUrl)
);

builder.Services.Configure<AntiClownDiscordApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDiscordApi"));
builder.Services.AddTransient<IAntiClownDiscordBotClient>(
    serviceProvider => AntiClownDiscordApiClientProvider.Build(
        serviceProvider.GetRequiredService<IOptions<AntiClownDiscordApiConnectionOptions>>().Value.ServiceUrl
    )
);

builder.Services.AddSingleton<ITelegramBotClient>(
    serviceProvider =>
    {
        var telegramSettings = serviceProvider.GetRequiredService<IOptions<TelegramSettings>>();
        return new TelegramBotClient(telegramSettings.Value.BotToken);
    }
);
builder.Services.AddTransient<ITelegramBotWorker, TelegramBotWorker>();

builder.Services.AddSingleton<IPartiesService, PartiesService>();

var app = builder.Build();

/*
var usersCache = app.Services.GetRequiredService<IUsersCache>();
await usersCache.InitializeAsync();
*/

var telegramBotWorker = app.Services.GetRequiredService<ITelegramBotWorker>();
await Task.WhenAll(telegramBotWorker.StartAsync(), app.RunAsync());
