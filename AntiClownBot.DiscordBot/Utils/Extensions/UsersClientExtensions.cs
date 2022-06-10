using AntiClownApiClient.UsersClient;

namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class UsersClientExtensions
{
    public static async Task<int> GetUserBalanceAsync(this IUsersClient usersClient, ulong userId)
    {
        var userRating = await usersClient.RatingAsync(userId);
        return userRating.ScamCoins;
    }
}