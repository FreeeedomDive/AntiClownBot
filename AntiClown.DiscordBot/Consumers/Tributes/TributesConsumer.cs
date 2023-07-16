using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Tributes;
using AntiClown.DiscordBot.Options;
using AntiClown.Messages.Dto.Tributes;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Tributes;

public class TributesConsumer : IConsumer<TributeMessageDto>
{
    public TributesConsumer(
        IDiscordClientWrapper discordClientWrapper,
        ITributeEmbedBuilder tributeEmbedBuilder,
        IOptions<DiscordOptions> discordOptions,
        ILogger<TributesConsumer> logger
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
        logger.LogInformation("Received auto tribute for user {userId}", tribute.UserId);
        var tributeEmbed = await tributeEmbedBuilder.BuildForSuccessfulTributeAsync(tribute.Tribute);
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.TributeChannelId, tributeEmbed);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly ITributeEmbedBuilder tributeEmbedBuilder;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly ILogger<TributesConsumer> logger;
}