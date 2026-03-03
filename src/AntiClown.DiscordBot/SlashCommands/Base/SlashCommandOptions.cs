using AntiClown.Data.Api.Dto.Rights;

namespace AntiClown.DiscordBot.SlashCommands.Base;

public class SlashCommandOptions
{
    public bool IsEphemeral { get; set; }
    public RightsDto[] RequiredRights { get; set; } = Array.Empty<RightsDto>();
}