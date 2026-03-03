using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.GuessNumber;

public class GuessNumberEmbedBuilder : IGuessNumberEmbedBuilder
{
    public GuessNumberEmbedBuilder(
        IAntiClownDataApiClient antiClownDataApiClient,
        IUsersCache usersCache
    )
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.usersCache = usersCache;
    }

    public async Task<DiscordEmbed> BuildAsync(GuessNumberEventDto guessNumberEvent)
    {
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle("Угадай число")
                           .WithColor(DiscordColor.Azure)
                           .WithFooter($"EventId: {guessNumberEvent.Id}");
        if (guessNumberEvent.Finished)
        {
            var winnersIds = guessNumberEvent.NumberToUsers.TryGetValue(guessNumberEvent.Result, out var result) ? result.ToArray() : Array.Empty<Guid>();
            var winners = await Task.WhenAll(winnersIds.Select(x => usersCache.GetMemberByApiIdAsync(x)));
            var winnersString = winners.Length == 0
                ? "Никто не угадал"
                : $"Правильно угадавшие игроки получают добычу-коробку:\n{string.Join("\n", winners.Select(x => x.ServerOrUserName()))}";
            embedBuilder.AddField($"Правильный ответ: {(int)guessNumberEvent.Result}", winnersString);
        }
        else
        {
            var waitingTime = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.CommonEvents, "GuessNumberEvent.WaitingTimeInMilliseconds");
            embedBuilder.AddField("Я загадал число, угадайте его!", $"У вас {waitingTime / (60 * 1000)} минут");
        }

        return embedBuilder.Build();
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IUsersCache usersCache;
}