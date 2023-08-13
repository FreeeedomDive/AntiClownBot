using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Extensions;

public static class DiscordMemberExtensions
{
    public static string ServerOrUserName(this DiscordMember? member)
    {
        return member?.Nickname ?? member?.DisplayName ?? member?.Username ?? "(чел, которого нет на сервере)";
    }
}