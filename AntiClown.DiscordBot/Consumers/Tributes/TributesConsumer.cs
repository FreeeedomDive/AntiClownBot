using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Tributes;
using AntiClown.DiscordBot.Options;
using AntiClown.Messages.Dto.Tributes;
using MassTransit;
using Microsoft.Extensions.Options;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Tributes;

public class TributesConsumer : IConsumer<TributeMessageDto>
{
    public TributesConsumer(
        IDiscordClientWrapper discordClientWrapper,
        ITributeEmbedBuilder tributeEmbedBuilder,
        IOptions<DiscordOptions> discordOptions,
        ILoggerClient logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.tributeEmbedBuilder = tributeEmbedBuilder;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<TributeMessageDto> context)
    {
        var tribute = context.Message;
        await logger.InfoAsync("Received auto tribute for user {userId}", tribute.UserId);
        var tributeEmbed = await tributeEmbedBuilder.BuildForSuccessfulTributeAsync(tribute.Tribute);
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.TributeChannelId, tributeEmbed);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly ILoggerClient logger;
    private readonly ITributeEmbedBuilder tributeEmbedBuilder;
}