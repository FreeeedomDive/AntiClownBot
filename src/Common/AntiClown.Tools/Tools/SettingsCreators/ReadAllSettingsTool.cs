using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Tools.Args;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class ReadAllSettingsTool : ToolBase
{
    public ReadAllSettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        IArgsProvider argsProvider,
        ILogger<ReadAllSettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.argsProvider = argsProvider;
    }

    protected override async Task RunAsync()
    {
        SettingDto[] settings;
        try
        {
            var categoryString = argsProvider.GetArgument("-category");
            var category = Enum.TryParse<SettingsCategory>(categoryString, out var x)
                ? x
                : throw new Exception($"Can't parse {categoryString} as {nameof(SettingsCategory)}");
            settings = await antiClownDataApiClient.Settings.FindAsync(category);
        }
        catch(ArgumentWasNotProvidedException)
        {
            settings = await antiClownDataApiClient.Settings.ReadAllAsync();
        }

        var settingsInfo = settings
                           .OrderBy(x => x.Category)
                           .Select(x => $"{x.Category} | {x.Name}: {x.Value}");

        Logger.LogInformation("Settings:\n{settings}", string.Join("\n", settingsInfo));
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IArgsProvider argsProvider;
}