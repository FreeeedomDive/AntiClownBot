using AntiClown.Data.Api.Client.Settings;

namespace AntiClown.Data.Api.Client;

public interface IAntiClownDataApiClient
{
    ISettingsClient Settings { get; set; }
}