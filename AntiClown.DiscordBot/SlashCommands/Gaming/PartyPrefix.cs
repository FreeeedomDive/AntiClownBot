using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Gaming;

public enum PartyPrefix
{
    [ChoiceName("dota")]
    Dota,
    [ChoiceName("csgo")]
    CsGo,
    [ChoiceName("sigame")]
    SiGame,
}