using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.SocialRating;

public class WhenCommandModule : ApplicationCommandModule
{
    public WhenCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IGuildSettingsService guildSettingsService,
        IRandomizer randomizer
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.guildSettingsService = guildSettingsService;
        this.randomizer = randomizer;
    }

    [SlashCommand("when", "Узнать время следующего подношения императору")]
    public async Task When(InteractionContext context)
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        if (context.Channel.Id != guildSettings.TributeChannelId && context.Channel.Id != guildSettings.HiddenTestChannelId)
        {
            var madgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Madge");
            var pointRightEmote = await discordClientWrapper.Emotes.FindEmoteAsync("point_right");
            var tributeChannel = await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.TributeChannelId);
            await discordClientWrapper.Messages.RespondAsync(context,
                $"{madgeEmote} {pointRightEmote} {tributeChannel.Mention}");
            return;
        }

        var embedBuilder = new DiscordEmbedBuilder();
        embedBuilder.WithTitle("А когда же подношение???");

        var result = await apiClient.Users.WhenNextTributeAsync(context.User.Id);
        var cooldownHasPassed = DateTime.Now > result.NextTribute;

        if (cooldownHasPassed)
        {
            embedBuilder.WithColor(DiscordColor.Green);
            embedBuilder.AddField(
                "Уже пора!!!",
                $"Срочно нужно исполнить партийный долг {(await discordClientWrapper.Emotes.FindEmoteAsync("flag_cn")).ToString().Multiply(3)}"
            );
            embedBuilder.AddField(
                $"А мог бы прийти и пораньше {await discordClientWrapper.Emotes.FindEmoteAsync("Clueless")}",
                $"Ты опоздал на {Utility.GetTimeDiff(result.NextTribute)}");
            await discordClientWrapper.Messages.RespondAsync(context, embedBuilder.Build());
            return;
        }

        embedBuilder.WithColor(DiscordColor.Red);
        var roflan = randomizer.GetRandomNumberBetween(0, 100);
        if (roflan == 69)
        {
            var aRolf = $"{await discordClientWrapper.Emotes.FindEmoteAsync("aRolf")}".Multiply(5);
            embedBuilder.AddField("Завтра в 3", aRolf);
        }
        else
        {
            embedBuilder.AddField($"А подношение император XI через {Utility.GetTimeDiff(result.NextTribute)}",
                $"Приходи не раньше чем {Utility.NormalizeTime(result.NextTribute)}");
        }

        await discordClientWrapper.Messages.RespondAsync(context, embedBuilder.Build());
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IRandomizer randomizer;
}