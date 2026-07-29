namespace AntiClown.DiscordBot.Telemetry;

public static class DiscordTelemetryValues
{
    public static class CommandOutcomes
    {
        public const string Ok = "ok";
        public const string Rejected = "rejected";
        public const string Error = "error";
    }

    public static class ApiOperations
    {
        public const string GatewayGet = "gateway.get";
        public const string UserGetCurrent = "user.get_current";
        public const string ApplicationGetCurrent = "application.get_current";
        public const string VoiceRegionsList = "voice_regions.list";
        public const string CommandsRegister = "commands.register";
        public const string GuildGet = "guild.get";
        public const string ChannelsList = "channels.list";
        public const string MembersList = "members.list";
        public const string MemberGet = "member.get";
        public const string MemberModify = "member.modify";
        public const string ChannelModify = "channel.modify";
        public const string MessageSend = "message.send";
        public const string MessageGet = "message.get";
        public const string MessageModify = "message.modify";
        public const string MessageDelete = "message.delete";
        public const string ThreadCreate = "thread.create";
        public const string ReactionAdd = "reaction.add";
        public const string ReactionRemove = "reaction.remove";
        public const string RoleCreate = "role.create";
        public const string RoleGrant = "role.grant";
        public const string RoleRevoke = "role.revoke";
        public const string InteractionResponseCreate = "interaction_response.create";
        public const string InteractionResponseGet = "interaction_response.get";
        public const string InteractionResponseEdit = "interaction_response.edit";
        public const string InteractionResponseDelete = "interaction_response.delete";
        public const string Other = "other";
    }

    public static class ApiOutcomes
    {
        public const string Ok = "ok";
        public const string Cancelled = "cancelled";
        public const string RateLimited = "rate_limited";
        public const string Unauthorized = "unauthorized";
        public const string Forbidden = "forbidden";
        public const string NotFound = "not_found";
        public const string BadRequest = "bad_request";
        public const string UpstreamError = "upstream_error";
        public const string Error = "error";
    }

    public static class ReconnectReasons
    {
        public const string ServerRequested = "server_requested";
        public const string HeartbeatTimeout = "heartbeat_timeout";
        public const string SocketError = "socket_error";
        public const string ConnectionClosed = "connection_closed";
    }

    public static class CacheNames
    {
        public const string UsersByApiId = "users_by_api_id";
        public const string UsersByDiscordId = "users_by_discord_id";
        public const string Emotes = "emotes";
    }

    public static class CacheResults
    {
        public const string Hit = "hit";
        public const string Miss = "miss";
    }

}
