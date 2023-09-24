namespace AntiClown.Data.Api.Core.SettingsStoring.Domain;

public class Setting
{
    public string Category { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}