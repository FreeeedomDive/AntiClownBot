namespace CommonServices.IpService;

public interface IIpService
{
    Task<string?> GetIp();
}