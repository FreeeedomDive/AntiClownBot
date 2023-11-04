using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Gaming;

public enum PartyPrefix
{
    [ChoiceName("dota")]
    Dota,
    [ChoiceName("cs2")]
    Cs2,
    [ChoiceName("sigame")]
    SiGame,
}