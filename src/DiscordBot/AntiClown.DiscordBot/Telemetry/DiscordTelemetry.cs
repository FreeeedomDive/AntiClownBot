using System.Diagnostics;
using System.Diagnostics.Metrics;
using DSharpPlus;
using DSharpPlus.EventArgs;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Metrics;

namespace AntiClown.DiscordBot.Telemetry;

public sealed class DiscordTelemetry
{
    public DiscordTelemetry(Meter meter)
    {
        commands = meter.CreateCounter<long>(CommandsName, "{command}");
        commandDuration = meter.CreateHistogram<double>(CommandDurationName, "s");
        apiCalls = meter.CreateCounter<long>(ApiCallsName, "{call}");
        apiCallDuration = meter.CreateHistogram<double>(ApiCallDurationName, "s");
        gatewayConnectedGauge = meter.CreateObservableGauge<int>(GatewayConnectedName, () => Volatile.Read(ref gatewayConnected));
        gatewayReconnects = meter.CreateCounter<long>(GatewayReconnectsName, "{reconnect}");
        cache = meter.CreateCounter<long>(CacheName, "{lookup}");
    }

    public static void ConfigureMetrics(MeterProviderBuilder metrics)
    {
        metrics
            .AddView(CommandDurationName, new ExplicitBucketHistogramConfiguration { Boundaries = DurationBuckets })
            .AddView(ApiCallDurationName, new ExplicitBucketHistogramConfiguration { Boundaries = DurationBuckets });
    }

    public static void ConfigureHttpClientTracing(HttpClientTraceInstrumentationOptions options)
    {
        options.EnrichWithHttpRequestMessage = (activity, request) =>
        {
            if (TryRedactDiscordCredential(request.RequestUri, out var sanitizedUrl))
            {
                activity.SetTag("url.full", sanitizedUrl);
                activity.SetTag("http.url", sanitizedUrl);
            }
        };
        options.EnrichWithHttpResponseMessage = (activity, response) =>
        {
            if ((int)response.StatusCode != 429)
            {
                return;
            }

            var retryAfter = response.Headers.RetryAfter;
            var retryAfterSeconds = retryAfter?.Delta?.TotalSeconds
                                    ?? (retryAfter?.Date - DateTimeOffset.UtcNow)?.TotalSeconds;
            if (retryAfterSeconds is not null)
            {
                activity.SetTag("discord.retry_after", Math.Max(retryAfterSeconds.Value, 0));
            }
        };
    }

    public void RecordCommand(string command, string outcome, TimeSpan elapsed)
    {
        var tags = new TagList
        {
            { "command", command },
            { "outcome", outcome },
        };
        commands.Add(1, tags);
        commandDuration.Record(elapsed.TotalSeconds, tags);
    }

    public void RecordApiCall(string operation, string outcome, TimeSpan elapsed)
    {
        var tags = new TagList
        {
            { "operation", operation },
            { "outcome", outcome },
        };
        apiCalls.Add(1, tags);
        apiCallDuration.Record(elapsed.TotalSeconds, tags);
    }

    public void RecordCacheLookup(string cacheName, bool found)
    {
        var tags = new TagList
        {
            { "cache", cacheName },
            { "result", found ? DiscordTelemetryValues.CacheResults.Hit : DiscordTelemetryValues.CacheResults.Miss },
        };
        cache.Add(1, tags);
    }

    public void SubscribeToGateway(DiscordClient client)
    {
        if (Interlocked.Exchange(ref gatewaySubscribed, 1) != 0)
        {
            return;
        }

        client.SocketOpened += OnSocketOpened;
        client.SocketClosed += OnSocketClosed;
        client.SocketErrored += OnSocketErrored;
        client.Zombied += OnZombied;
    }

    private Task OnSocketOpened(DiscordClient _, SocketEventArgs __)
    {
        string? reconnectReason = null;
        lock (gatewayStateLock)
        {
            Volatile.Write(ref gatewayConnected, 1);
            if (gatewayHasOpened)
            {
                reconnectReason = pendingReconnectReason ?? DiscordTelemetryValues.ReconnectReasons.ConnectionClosed;
            }

            gatewayHasOpened = true;
            pendingReconnectReason = null;
        }

        if (reconnectReason is not null)
        {
            gatewayReconnects.Add(1, new KeyValuePair<string, object?>("reason", reconnectReason));
        }

        return Task.CompletedTask;
    }

    private Task OnSocketClosed(DiscordClient _, SocketCloseEventArgs eventArgs)
    {
        var reason = eventArgs.CloseCode == ServerRequestedReconnectCode
            ? DiscordTelemetryValues.ReconnectReasons.ServerRequested
            : DiscordTelemetryValues.ReconnectReasons.ConnectionClosed;
        MarkGatewayDisconnected(reason, overwritePendingReason: false);
        return Task.CompletedTask;
    }

    private Task OnSocketErrored(DiscordClient _, SocketErrorEventArgs __)
    {
        MarkGatewayDisconnected(DiscordTelemetryValues.ReconnectReasons.SocketError, overwritePendingReason: true);
        return Task.CompletedTask;
    }

    private Task OnZombied(DiscordClient _, ZombiedEventArgs eventArgs)
    {
        if (eventArgs.GuildDownloadCompleted)
        {
            MarkGatewayDisconnected(DiscordTelemetryValues.ReconnectReasons.HeartbeatTimeout, overwritePendingReason: true);
        }

        return Task.CompletedTask;
    }

    private static bool TryRedactDiscordCredential(Uri? uri, out string sanitizedUrl)
    {
        sanitizedUrl = string.Empty;
        if (uri is null
            || !(uri.Host.Equals(DiscordHost, StringComparison.OrdinalIgnoreCase)
                 || uri.Host.Equals(LegacyDiscordHost, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var credentialIndex = -1;
        for (var index = 0; index < segments.Length; index++)
        {
            if (segments[index] is "interactions" or "webhooks" && index + 2 < segments.Length)
            {
                credentialIndex = index + 2;
                break;
            }
        }

        if (credentialIndex < 0)
        {
            return false;
        }

        segments[credentialIndex] = Redacted;
        var builder = new UriBuilder(uri)
        {
            Path = "/" + string.Join('/', segments),
            Query = string.Empty,
            Fragment = string.Empty,
        };
        sanitizedUrl = builder.Uri.AbsoluteUri;
        return true;
    }

    private void MarkGatewayDisconnected(string reason, bool overwritePendingReason)
    {
        lock (gatewayStateLock)
        {
            Volatile.Write(ref gatewayConnected, 0);
            if (overwritePendingReason || pendingReconnectReason is null)
            {
                pendingReconnectReason = reason;
            }
        }
    }

    public const string CommandsName = "anticlown.discord.commands";
    public const string CommandDurationName = "anticlown.discord.command.duration";
    public const string ApiCallsName = "anticlown.discord.api.calls";
    public const string ApiCallDurationName = "anticlown.discord.api.call.duration";
    public const string GatewayConnectedName = "anticlown.discord.gateway.connected";
    public const string GatewayReconnectsName = "anticlown.discord.gateway.reconnects";
    public const string CacheName = "anticlown.discord.cache";

    private const int ServerRequestedReconnectCode = 4000;
    private const string DiscordHost = "discord.com";
    private const string LegacyDiscordHost = "discordapp.com";
    private const string Redacted = "REDACTED";

    private static readonly double[] DurationBuckets =
        [0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];

    private readonly Counter<long> apiCalls;
    private readonly Histogram<double> apiCallDuration;
    private readonly Counter<long> cache;
    private readonly Histogram<double> commandDuration;
    private readonly Counter<long> commands;
    private readonly ObservableGauge<int> gatewayConnectedGauge;
    private readonly Counter<long> gatewayReconnects;
    private readonly object gatewayStateLock = new();

    private int gatewayConnected;
    private bool gatewayHasOpened;
    private int gatewaySubscribed;
    private string? pendingReconnectReason;
}
