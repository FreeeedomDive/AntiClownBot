using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Options;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.EmbedBuilders.F1Predictions;

public class F1PredictionsEmbedBuilder(
    IUsersCache usersCache,
    IOptions<WebOptions> webOptions
) : IF1PredictionsEmbedBuilder
{
    public async Task<DiscordEmbed> BuildBingoCompletedAsync(Guid userId)
    {
        var member = await usersCache.GetMemberByApiIdAsync(userId);
        return new DiscordEmbedBuilder()
               .WithTitle("Бинго!")
               .WithColor(DiscordColor.Orange)
               .AddField(
                   $"У {member.ServerOrUserName()} бинго!",
                   $"{webOptions.Value.FrontApplicationUrl}/user/{userId}/f1Predictions/bingo"
               )
               .Build();
    }

}
