namespace Dto.MinecraftServerDto;

public class MinecraftServerPlayerInfo
{
    public int Online { get; set; }
    public int MaxPlayers { get; set; }
    public string[]? Players { get; set; }
}