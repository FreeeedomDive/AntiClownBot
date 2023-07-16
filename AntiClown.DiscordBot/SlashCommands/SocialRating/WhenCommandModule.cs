using AntiClown.Api.Client;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Tools.Utility.Extensions;
using AntiClown.Tools.Utility.Random;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.SocialRating;

public class WhenCommandModule : SlashCommandModuleWithMiddlewares
{
    public WhenCommandModule(
        IAntiClownApiClient antiClownApiClient,
        ICommandExecutor commandExecutor,
        IEmotesCache emotesCache,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.emotesCache = emotesCache;
        this.usersCache = usersCache;
        this.antiClownApiClient = antiClownApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.When, "Узнать время следующего подношения императору")]
    public async Task When(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                var embedBuilder = new DiscordEmbedBuilder();
                embedBuilder.WithTitle("А когда же подношение???");

                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var result = await antiClownApiClient.Tribute.ReadNextTributeInfoAsync(userId);
                var now = DateTime.UtcNow;
                var cooldownHasPassed = now > result.NextTributeDateTime;

                var tributeDateTimeDifference = now.GetDifferenceTimeSpan(result.NextTributeDateTime);
                if (cooldownHasPassed)
                {
                    embedBuilder.WithColor(DiscordColor.Green);
                    embedBuilder.AddField(
                        "Уже пора!!!",
                        $"Срочно нужно исполнить партийный долг {(await emotesCache.GetEmoteAsTextAsync("flag_cn")).Multiply(3)}"
                    );
                    embedBuilder.AddField(
                        $"А мог бы прийти и пораньше {await emotesCache.GetEmoteAsTextAsync("Clueless")}",
                        $"Ты опоздал на {tributeDateTimeDifference.ToTimeDiffString()}"
                    );
                    await RespondToInteractionAsync(context, embedBuilder.Build());
                    return;
                }

                embedBuilder.WithColor(DiscordColor.Red);
                var roflan = Randomizer.GetRandomNumberBetween(0, 100);
                if (roflan == 69)
                {
                    var aRolf = $"{await emotesCache.GetEmoteAsTextAsync("aRolf")}".Multiply(5);
                    embedBuilder.AddField("Завтра в 3", aRolf);
                }
                else
                {
                    embedBuilder.AddField(
                        $"А подношение император XI через {tributeDateTimeDifference.ToTimeDiffString()}",
                        $"Приходи не раньше чем {result.NextTributeDateTime.ToYekaterinburgTime()}"
                    );
                }

                await RespondToInteractionAsync(context, embedBuilder.Build());
            }
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;

    private readonly IEmotesCache emotesCache;
    private readonly IUsersCache usersCache;
}