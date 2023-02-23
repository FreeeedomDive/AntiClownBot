namespace AntiClownDiscordBotVersion2.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class ObsoleteCommandAttribute : Attribute
{
    public ObsoleteCommandAttribute(string? slashCommand)
    {
        SlashCommand = slashCommand;
    }
    
    public string? SlashCommand { get; }
}