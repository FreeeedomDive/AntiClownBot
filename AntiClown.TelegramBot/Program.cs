using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.TelegramBot.Options;
using AntiClown.TelegramBot.TelegramWorker;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelemetryApp.Utilities.Extensions;

var builder = WebApplication.CreateBuilder(args);

var telemetrySettingsSection = builder.Configuration.GetRequiredSection("Telemetry");
builder.Services.ConfigureTelemetryClientWithLogger("SpotifyListenTogether", "TelegramBot", telemetrySettingsSection["ApiUrl"]);

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

builder.Services.AddTransient<ITelegramBotWorker, TelegramBotWorker>();
builder.Services.AddSingleton<ITelegramBotClient>(
    serviceProvider =>
    {
        var telegramSettings = serviceProvider.GetRequiredService<IOptions<TelegramSettings>>();
        return new TelegramBotClient(telegramSettings.Value.BotToken);
    }
);

var app = builder.Build();

var telegramBotWorker = app.Services.GetRequiredService<ITelegramBotWorker>();
await telegramBotWorker.StartAsync();