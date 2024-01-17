using AntiClown.Data.Api.Client.Settings;
using AntiClown.Data.Api.Client.Tokens;

namespace AntiClown.Data.Api.Client;

public interface IAntiClownDataApiClient
{
    ISettingsClient Settings { get; set; }
    ITokensClient Tokens { get; set; }
}