using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.SocialRating;

public class WhenCommandModule : SlashCommandModuleWithMiddlewares
{
    public WhenCommandModule(
        ICommandExecutor commandExecutor,
        IEmotesProvider emotesProvider,
        IApiClient apiClient,
        IRandomizer randomizer
    ) : base(commandExecutor)
    {
        this.emotesProvider = emotesProvider;
        this.apiClient = apiClient;
        this.randomizer = randomizer;
    }

    [SlashCommand(Interactions.Commands.When, "Узнать время следующего подношения императору")]
    public async Task When(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle("А когда же подношение???");

            var result = await apiClient.Users.WhenNextTributeAsync(context.User.Id);
            var cooldownHasPassed = DateTime.Now > result.NextTribute;

            if (cooldownHasPassed)
            {
                embedBuilder.WithColor(DiscordColor.Green);
                embedBuilder.AddField(
                    "Уже пора!!!",
                    $"Срочно нужно исполнить партийный долг {(await emotesProvider.GetEmoteAsTextAsync("flag_cn")).Multiply(3)}"
                );
                embedBuilder.AddField(
                    $"А мог бы прийти и пораньше {await emotesProvider.GetEmoteAsTextAsync("Clueless")}",
                    $"Ты опоздал на {Utility.GetTimeDiff(result.NextTribute)}");
                await RespondToInteractionAsync(context, embedBuilder.Build());
                return;
            }

            embedBuilder.WithColor(DiscordColor.Red);
            var roflan = randomizer.GetRandomNumberBetween(0, 100);
            if (roflan == 69)
            {
                var aRolf = $"{await emotesProvider.GetEmoteAsTextAsync("aRolf")}".Multiply(5);
                embedBuilder.AddField("Завтра в 3", aRolf);
            }
            else
            {
                embedBuilder.AddField($"А подношение император XI через {Utility.GetTimeDiff(result.NextTribute)}",
                    $"Приходи не раньше чем {Utility.NormalizeTime(result.NextTribute)}");
            }

            await RespondToInteractionAsync(context, embedBuilder.Build());
        });
    }

    private readonly IEmotesProvider emotesProvider;
    private readonly IApiClient apiClient;
    private readonly IRandomizer randomizer;
}