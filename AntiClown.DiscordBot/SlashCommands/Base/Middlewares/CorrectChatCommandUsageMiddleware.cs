using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Options;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

public class CorrectChatCommandUsageMiddleware : ICommandMiddleware
{
    public CorrectChatCommandUsageMiddleware(
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IOptions<DiscordOptions> discordOptions
    )
    {
        this.emotesCache = emotesCache;
        this.discordOptions = discordOptions;
        this.discordClientWrapper = discordClientWrapper;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        var guildSettings = discordOptions.Value;
        var command = context.Context.CommandName;
        var channelId = context.Context.Channel.Id;

        // всегда позволять выполнять все команды в чате для тестов
        if (channelId == guildSettings.HiddenTestChannelId)
        {
            await next(context);
            return;
        }

        // запрещаем использовать команды пати в любых других каналах, кроме канала для пати
        if (command == InteractionsIds.CommandsNames.Party_Group && channelId != guildSettings.PartyChannelId)
        {
            var madgeEmote = await emotesCache.GetEmoteAsTextAsync("Madge");
            var pointRightEmote = await emotesCache.GetEmoteAsTextAsync("point_right");
            var partyChannel = await discordClientWrapper.Channels.FindDiscordChannel(guildSettings.PartyChannelId);
            
            await RespondWithErrorAsync(
                context.Context,
                $"{madgeEmote} {pointRightEmote} {partyChannel.Mention}"
            );
            return;
        }

        // запрещаем использовать команды инвентаря и трибутов в любых других каналах, кроме предназначенного для этого канала
        if (command is InteractionsIds.CommandsNames.Inventory
                    or InteractionsIds.CommandsNames.LootBox
                    or InteractionsIds.CommandsNames.Shop
                    or InteractionsIds.CommandsNames.Lohotron
                    or InteractionsIds.CommandsNames.Tribute
                    or InteractionsIds.CommandsNames.When
            && channelId != guildSettings.TributeChannelId)
        {
            var madgeEmote = await emotesCache.GetEmoteAsTextAsync("Madge");
            var pointRightEmote = await emotesCache.GetEmoteAsTextAsync("point_right");
            var tributeChannel = await discordClientWrapper.Channels.FindDiscordChannel(guildSettings.TributeChannelId);
            await RespondWithErrorAsync(context.Context, $"{madgeEmote} {pointRightEmote} {tributeChannel.Mention}");
            return;
        }

        // запрещаем использовать любые команды, кроме пати, в канале для пати
        if (command != InteractionsIds.CommandsNames.Party_Group && channelId == guildSettings.PartyChannelId)
        {
            var madgeEmote = await emotesCache.GetEmoteAsTextAsync("Madge");
            await RespondWithErrorAsync(context.Context, $"{madgeEmote} не срать в чате для пати!");
            return;
        }

        await next(context);
    }

    private static async Task RespondWithErrorAsync(BaseContext context, string errorMessage)
    {
        await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(errorMessage));
    }

    private readonly IEmotesCache emotesCache;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IDiscordClientWrapper discordClientWrapper;
}