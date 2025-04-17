namespace AntiClown.Telegram.Bot.TelegramWorker;

public class PartyGameOptions
{
    public string CommandPrefix { get; private init; } = string.Empty;
    public string RoleIdKey { get; private init; } = string.Empty;
    public string PartyName { get; private init; } = string.Empty;
    public int MaxPlayers { get; private init; }

    public static readonly PartyGameOptions[] AllowedParties = new[]
    {
        new PartyGameOptions
        {
            CommandPrefix = "/cs",
            RoleIdKey = "CsRoleId",
            PartyName = "CS2",
            MaxPlayers = 5,
        },
        new PartyGameOptions
        {
            CommandPrefix = "/dota",
            RoleIdKey = "DotaRoleId",
            PartyName = "Dota",
            MaxPlayers = 5,
        },
        new PartyGameOptions
        {
            CommandPrefix = "/repo",
            RoleIdKey = "RepoRoleId",
            PartyName = "R.E.P.O",
            MaxPlayers = 6,
        },
    };
}