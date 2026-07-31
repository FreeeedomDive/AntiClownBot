using System.Diagnostics;
using OpenTelemetry;

namespace AntiClown.DiscordBot.Telemetry;

public sealed class DiscordApiMetricsProcessor(DiscordTelemetry telemetry) : BaseProcessor<Activity>
{
    public override void OnEnd(Activity activity)
    {
        if (activity.Source.Name != HttpClientActivitySourceName || !TryGetDiscordApiUri(activity, out var uri))
        {
            return;
        }

        var method = GetTagValue(activity, HttpRequestMethodTag, LegacyHttpMethodTag)?.ToUpperInvariant() ?? string.Empty;
        var operation = ClassifyOperation(method, GetApiPathSegments(uri));
        var outcome = ClassifyOutcome(activity);
        telemetry.RecordApiCall(operation, outcome, activity.Duration);
    }

    private static bool TryGetDiscordApiUri(Activity activity, out Uri uri)
    {
        var value = GetTagValue(activity, UrlFullTag, LegacyHttpUrlTag);
        if (!Uri.TryCreate(value, UriKind.Absolute, out uri!))
        {
            return false;
        }

        return uri.Host.Equals(DiscordHost, StringComparison.OrdinalIgnoreCase)
               || uri.Host.Equals(LegacyDiscordHost, StringComparison.OrdinalIgnoreCase);
    }

    private static string[] GetApiPathSegments(Uri uri)
    {
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var offset = segments.Length > 0 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        if (segments.Length > offset + 1 && IsApiVersion(segments[offset]))
        {
            offset++;
        }

        return segments[offset..];
    }

    private static bool IsApiVersion(string segment)
    {
        return segment.Length > 1
               && segment[0] == 'v'
               && segment.AsSpan(1).IndexOfAnyExceptInRange('0', '9') < 0;
    }

    private static string ClassifyOperation(string method, string[] path)
    {
        if (method == "GET" && path is ["gateway", "bot"])
            return DiscordTelemetryValues.ApiOperations.GatewayGet;
        if (method == "GET" && path is ["users", "@me"])
            return DiscordTelemetryValues.ApiOperations.UserGetCurrent;
        if (method == "GET" && path is ["oauth2", "applications", "@me"])
            return DiscordTelemetryValues.ApiOperations.ApplicationGetCurrent;
        if (method == "GET" && path is ["voice", "regions"])
            return DiscordTelemetryValues.ApiOperations.VoiceRegionsList;
        if (method == "PUT" && (path is ["applications", _, "commands"] || path is ["applications", _, "guilds", _, "commands"]))
            return DiscordTelemetryValues.ApiOperations.CommandsRegister;
        if (method == "GET" && path is ["guilds", _])
            return DiscordTelemetryValues.ApiOperations.GuildGet;
        if (method == "GET" && path is ["guilds", _, "channels"])
            return DiscordTelemetryValues.ApiOperations.ChannelsList;
        if (method == "GET" && path is ["guilds", _, "members"])
            return DiscordTelemetryValues.ApiOperations.MembersList;
        if (method == "GET" && path is ["guilds", _, "members", _])
            return DiscordTelemetryValues.ApiOperations.MemberGet;
        if (method == "PATCH" && path is ["guilds", _, "members", _])
            return DiscordTelemetryValues.ApiOperations.MemberModify;
        if (method == "PATCH" && path is ["channels", _])
            return DiscordTelemetryValues.ApiOperations.ChannelModify;
        if (method == "POST" && path is ["channels", _, "messages"])
            return DiscordTelemetryValues.ApiOperations.MessageSend;
        if (method == "GET" && path is ["channels", _, "messages", _])
            return DiscordTelemetryValues.ApiOperations.MessageGet;
        if (method == "PATCH" && path is ["channels", _, "messages", _])
            return DiscordTelemetryValues.ApiOperations.MessageModify;
        if (method == "DELETE" && path is ["channels", _, "messages", _])
            return DiscordTelemetryValues.ApiOperations.MessageDelete;
        if (method == "POST" && path is ["channels", _, "messages", _, "threads"])
            return DiscordTelemetryValues.ApiOperations.ThreadCreate;
        if (method == "PUT" && path is ["channels", _, "messages", _, "reactions", ..])
            return DiscordTelemetryValues.ApiOperations.ReactionAdd;
        if (method == "DELETE" && path is ["channels", _, "messages", _, "reactions", ..])
            return DiscordTelemetryValues.ApiOperations.ReactionRemove;
        if (method == "POST" && path is ["guilds", _, "roles"])
            return DiscordTelemetryValues.ApiOperations.RoleCreate;
        if (method == "PUT" && path is ["guilds", _, "members", _, "roles", _])
            return DiscordTelemetryValues.ApiOperations.RoleGrant;
        if (method == "DELETE" && path is ["guilds", _, "members", _, "roles", _])
            return DiscordTelemetryValues.ApiOperations.RoleRevoke;
        if (method == "POST" && path is ["interactions", _, _, "callback"])
            return DiscordTelemetryValues.ApiOperations.InteractionResponseCreate;
        if (method == "GET" && path is ["webhooks", _, _, "messages", "@original"])
            return DiscordTelemetryValues.ApiOperations.InteractionResponseGet;
        if (method == "PATCH" && path is ["webhooks", _, _, "messages", "@original"])
            return DiscordTelemetryValues.ApiOperations.InteractionResponseEdit;
        if (method == "DELETE" && path is ["webhooks", _, _, "messages", "@original"])
            return DiscordTelemetryValues.ApiOperations.InteractionResponseDelete;

        return DiscordTelemetryValues.ApiOperations.Other;
    }

    private static string ClassifyOutcome(Activity activity)
    {
        var errorType = GetTagValue(activity, ErrorTypeTag);
        if (errorType?.Contains("Canceled", StringComparison.OrdinalIgnoreCase) == true
            || errorType?.Contains("Cancelled", StringComparison.OrdinalIgnoreCase) == true)
        {
            return DiscordTelemetryValues.ApiOutcomes.Cancelled;
        }

        var statusCode = GetStatusCode(activity);
        return statusCode switch
        {
            >= 200 and < 300 => DiscordTelemetryValues.ApiOutcomes.Ok,
            400 => DiscordTelemetryValues.ApiOutcomes.BadRequest,
            401 => DiscordTelemetryValues.ApiOutcomes.Unauthorized,
            403 => DiscordTelemetryValues.ApiOutcomes.Forbidden,
            404 => DiscordTelemetryValues.ApiOutcomes.NotFound,
            429 => DiscordTelemetryValues.ApiOutcomes.RateLimited,
            >= 400 and < 500 => DiscordTelemetryValues.ApiOutcomes.BadRequest,
            >= 500 => DiscordTelemetryValues.ApiOutcomes.UpstreamError,
            null when activity.Status == ActivityStatusCode.Error => DiscordTelemetryValues.ApiOutcomes.UpstreamError,
            _ => DiscordTelemetryValues.ApiOutcomes.Error,
        };
    }

    private static int? GetStatusCode(Activity activity)
    {
        var value = activity.GetTagItem(HttpResponseStatusCodeTag) ?? activity.GetTagItem(LegacyHttpStatusCodeTag);
        return value switch
        {
            int statusCode => statusCode,
            long statusCode => (int)statusCode,
            string statusCode when int.TryParse(statusCode, out var parsed) => parsed,
            _ => null,
        };
    }

    private static string? GetTagValue(Activity activity, params string[] names)
    {
        foreach (var name in names)
        {
            if (activity.GetTagItem(name)?.ToString() is { Length: > 0 } value)
            {
                return value;
            }
        }

        return null;
    }

    private const string HttpClientActivitySourceName = "System.Net.Http";
    private const string DiscordHost = "discord.com";
    private const string LegacyDiscordHost = "discordapp.com";

    private const string HttpRequestMethodTag = "http.request.method";
    private const string HttpResponseStatusCodeTag = "http.response.status_code";
    private const string UrlFullTag = "url.full";
    private const string ErrorTypeTag = "error.type";

    private const string LegacyHttpMethodTag = "http.method";
    private const string LegacyHttpStatusCodeTag = "http.status_code";
    private const string LegacyHttpUrlTag = "http.url";
}
