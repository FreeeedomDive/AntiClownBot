using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base.Middlewares;

public class CorrectChatCommandUsageMiddleware : ICommandMiddleware
{
    public CorrectChatCommandUsageMiddleware(
        IGuildSettingsService guildSettingsService,
        IDiscordClientWrapper discordClientWrapper
    )
    {
        this.guildSettingsService = guildSettingsService;
        this.discordClientWrapper = discordClientWrapper;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        var command = context.Context.CommandName;
        var channelId = context.Context.Channel.Id;

        // всегда позволять выполнять все команды в чате для тестов
        if (channelId == guildSettings.HiddenTestChannelId)
        {
            await next(context);
            return;
        }

        // запрещаем использовать команды пати в любых других каналах, кроме канала для пати
        if (command == Interactions.Commands.Party_Group && channelId != guildSettings.PartyChannelId)
        {
            var madgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Madge");
            var pointRightEmote = await discordClientWrapper.Emotes.FindEmoteAsync("point_right");
            var partyChannel = await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.PartyChannelId);
            
            await RespondWithErrorAsync(
                context.Context,
                $"{madgeEmote} {pointRightEmote} {partyChannel.Mention}"
            );
            return;
        }

        // запрещаем использовать команды инвентаря и трибутов в любых других каналах, кроме предназначенного для этого канала
        if (command is Interactions.Commands.Inventory
                    or Interactions.Commands.Lootbox
                    or Interactions.Commands.Shop
                    or Interactions.Commands.Lohotron
                    or Interactions.Commands.Tribute
                    or Interactions.Commands.When
            && channelId != guildSettings.TributeChannelId)
        {
            var madgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Madge");
            var pointRightEmote = await discordClientWrapper.Emotes.FindEmoteAsync("point_right");
            var tributeChannel = await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.TributeChannelId);
            await RespondWithErrorAsync(context.Context, $"{madgeEmote} {pointRightEmote} {tributeChannel.Mention}");
            return;
        }

        // запрещаем использовать любые команды, кроме пати, в канале для пати
        if (command != Interactions.Commands.Party_Group && channelId == guildSettings.PartyChannelId)
        {
            var madgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Madge");
            await RespondWithErrorAsync(context.Context, $"{madgeEmote} не срать в чате для пати!");
            return;
        }

        await next(context);
    }

    private static async Task RespondWithErrorAsync(BaseContext context, string errorMessage)
    {
        await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(errorMessage));
    }

    private readonly IGuildSettingsService guildSettingsService;
    private readonly IDiscordClientWrapper discordClientWrapper;
}