using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class DiscordMemberExtensions
{
    public static string ServerOrUserName(this DiscordMember member)
    {
        return member.Nickname ?? member.Username;
    }
}