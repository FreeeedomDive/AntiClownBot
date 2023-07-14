using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.DiscordBot.Consumers.Events.Common;
using AntiClown.DiscordBot.Consumers.Events.Daily;
using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Client.Configuration;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Bedge;
using AntiClown.EntertainmentApi.Dto.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Lottery;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using AntiClown.EntertainmentApi.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Transfusion;
using AntiClown.EntertainmentApi.Dto.DailyEvents.Announce;
using AntiClown.EntertainmentApi.Dto.DailyEvents.ResetsAndPayments;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

var antiClownApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("ApiUrl").Value!;
var antiClownEntertainmentApiServiceUrl = builder.Configuration.GetSection("AntiClown").GetSection("EntertainmentApiUrl").Value!;

builder.Services.AddTransient<IAntiClownApiClient>(_ => AntiClownApiClientProvider.Build(antiClownApiServiceUrl));
builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(_ => AntiClownEntertainmentApiClientProvider.Build(antiClownEntertainmentApiServiceUrl));

builder.Services.AddTransient<ICommonEventConsumer<GuessNumberEventDto>, GuessNumberEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<LotteryEventDto>, LotteryEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<RaceEventDto>, RaceEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<RemoveCoolDownsEventDto>, RemoveCoolDownsEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<TransfusionEventDto>, TransfusionEventConsumer>();
builder.Services.AddTransient<ICommonEventConsumer<BedgeEventDto>, BedgeEventConsumer>();

builder.Services.AddTransient<IDailyEventConsumer<AnnounceEventDto>, AnnounceEventConsumer>();
builder.Services.AddTransient<IDailyEventConsumer<ResetsAndPaymentsEventDto>, ResetsAndPaymentsConsumer>();

var rabbitMqSection = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddMassTransit(
    massTransitConfiguration =>
    {
        massTransitConfiguration.AddConsumers(AppDomain.CurrentDomain.GetAssemblies());
        massTransitConfiguration.SetKebabCaseEndpointNameFormatter();
        massTransitConfiguration.UsingRabbitMq(
            (context, rabbitMqConfiguration) =>
            {
                rabbitMqConfiguration.ConfigureEndpoints(context);
                rabbitMqConfiguration.Host(
                    rabbitMqSection["Host"], "/", hostConfiguration =>
                    {
                        hostConfiguration.Username(rabbitMqSection["Login"]);
                        hostConfiguration.Password(rabbitMqSection["Password"]);
                    }
                );
            }
        );
    }
);

var app = builder.Build();
await app.RunAsync();