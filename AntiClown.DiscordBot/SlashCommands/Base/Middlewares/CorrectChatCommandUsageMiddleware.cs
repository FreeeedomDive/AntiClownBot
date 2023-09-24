using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Models.Interactions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

public class CorrectChatCommandUsageMiddleware : ICommandMiddleware
{
    public CorrectChatCommandUsageMiddleware(
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.emotesCache = emotesCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.discordClientWrapper = discordClientWrapper;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        var allGuildSettings = await antiClownDataApiClient.Settings.FindAsync(SettingsCategory.DiscordGuild);
        var command = context.Context.CommandName;
        var channelId = context.Context.Channel.Id;

        // всегда позволять выполнять все команды в чате для тестов
        if (channelId == allGuildSettings.First(x => x.Name == "HiddenTestChannelId").GetValue<ulong>())
        {
            await next(context);
            return;
        }

        // запрещаем использовать команды пати в любых других каналах, кроме канала для пати

        var partyChannelId = allGuildSettings.First(x => x.Name == "PartyChannelId").GetValue<ulong>();
        if (command == InteractionsIds.CommandsNames.Party_Group && channelId != partyChannelId)
        {
            var madgeEmote = await emotesCache.GetEmoteAsTextAsync("Madge");
            var pointRightEmote = await emotesCache.GetEmoteAsTextAsync("point_right");
            var partyChannel = await discordClientWrapper.Channels.FindDiscordChannel(partyChannelId);
            
            await RespondWithErrorAsync(
                context.Context,
                $"{madgeEmote} {pointRightEmote} {partyChannel.Mention}"
            );
            return;
        }

        var tributeChannelId = allGuildSettings.First(x => x.Name == "TributeChannelId").GetValue<ulong>();
        // запрещаем использовать команды инвентаря и трибутов в любых других каналах, кроме предназначенного для этого канала
        if (command is InteractionsIds.CommandsNames.Inventory
                    or InteractionsIds.CommandsNames.LootBox
                    or InteractionsIds.CommandsNames.Shop_Open
                    or InteractionsIds.CommandsNames.Lohotron
                    or InteractionsIds.CommandsNames.Tribute
                    or InteractionsIds.CommandsNames.When
            && channelId != tributeChannelId)
        {
            var madgeEmote = await emotesCache.GetEmoteAsTextAsync("Madge");
            var pointRightEmote = await emotesCache.GetEmoteAsTextAsync("point_right");
            var tributeChannel = await discordClientWrapper.Channels.FindDiscordChannel(tributeChannelId);
            await RespondWithErrorAsync(context.Context, $"{madgeEmote} {pointRightEmote} {tributeChannel.Mention}");
            return;
        }

        // запрещаем использовать любые команды, кроме пати, в канале для пати
        if (command != InteractionsIds.CommandsNames.Party_Group && channelId == partyChannelId)
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
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
}