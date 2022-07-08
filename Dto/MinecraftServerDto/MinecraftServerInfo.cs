namespace Dto.MinecraftServerDto;

public class MinecraftServerInfo
{
    public bool Online { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    public MinecraftServerPlayerInfo Players { get; set; }
    public string Version { get; set; }
}