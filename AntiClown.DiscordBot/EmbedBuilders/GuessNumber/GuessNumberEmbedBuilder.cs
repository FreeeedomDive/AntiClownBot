using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Tools.Utility.Extensions;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.EmbedBuilders.GuessNumber;

public class GuessNumberEmbedBuilder : IGuessNumberEmbedBuilder
{
    public GuessNumberEmbedBuilder(
        IUsersCache usersCache,
        IOptions<Settings> settings
    )
    {
        this.usersCache = usersCache;
        this.settings = settings;
    }

    public async Task<DiscordEmbed> BuildAsync(GuessNumberEventDto guessNumberEvent)
    {
        var ping = DateTime.UtcNow.IsNightTime() || !settings.Value.PingOnEvents ? "" : "@everyone ";
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle("Угадай число")
                           .WithColor(guessNumberEvent.Finished ? DiscordColor.Black : DiscordColor.Azure)
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
            embedBuilder.AddField($"{ping}Я загадал число, угадайте его!", "У вас 10 минут");
        }

        return embedBuilder.Build();
    }

    private readonly IOptions<Settings> settings;
    private readonly IUsersCache usersCache;
}