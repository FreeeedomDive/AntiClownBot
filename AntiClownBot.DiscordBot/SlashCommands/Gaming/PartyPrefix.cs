using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Gaming;

public enum PartyPrefix
{
    [ChoiceName("dota")]
    Dota,
    [ChoiceName("csgo")]
    CsGo,
    [ChoiceName("sigame")]
    SiGame
}