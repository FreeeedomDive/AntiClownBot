/* Generated file */

using AntiClown.Data.Api.Client.Rights;
using AntiClown.Data.Api.Client.Settings;
using AntiClown.Data.Api.Client.Tokens;

namespace AntiClown.Data.Api.Client;

public interface IAntiClownDataApiClient
{
    IRightsClient Rights { get; }
    ISettingsClient Settings { get; }
    ITokensClient Tokens { get; }
}
