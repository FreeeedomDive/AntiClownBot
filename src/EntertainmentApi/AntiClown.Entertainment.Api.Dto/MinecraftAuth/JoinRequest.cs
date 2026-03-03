namespace AntiClown.Entertainment.Api.Dto.MinecraftAuth;

public class JoinRequest
{
    public string AccessToken { get; set; }
    public string UserUUID { get; set; }
    public string ServerID { get; set; }
}