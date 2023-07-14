using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.DiscordBot.Consumers.Events.Common;
using AntiClown.DiscordBot.Consumers.Events.Daily;
using AntiClown.DiscordBot.Options;
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
using MassTransit;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<AntiClownApiConnectionOptions>(builder.Configuration.GetSection("AntiClownApi"));
builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));

builder.Services.AddTransient<IAntiClownApiClient>(
    serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetService<IOptions<AntiClownApiConnectionOptions>>()?.Value.ServiceUrl)
);
builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(
    serviceProvider => AntiClownEntertainmentApiClientProvider.Build(serviceProvider.GetService<IOptions<AntiClownEntertainmentApiConnectionOptions>>()?.Value.ServiceUrl)
);

builder.Services.AddTransient<ICommonEventConsumer<GuessNumberEventDto>, GuessNumberEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<LotteryEventDto>, LotteryEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<RaceEventDto>, RaceEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<RemoveCoolDownsEventDto>, RemoveCoolDownsEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<TransfusionEventDto>, TransfusionEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<BedgeEventDto>, BedgeEventConsumer>();

builder.Services.AddTransient<IDailyEventConsumer<AnnounceEventDto>, AnnounceEventConsumer>();
builder.Services.AddTransient<IDailyEventConsumer<ResetsAndPaymentsEventDto>, ResetsAndPaymentsConsumer>();

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

var app = builder.Build();
await app.RunAsync();