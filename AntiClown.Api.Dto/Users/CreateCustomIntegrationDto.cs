namespace AntiClown.Api.Dto.Users;

public class CreateCustomIntegrationDto
{
    public Guid UserId { get; set; }
    public string IntegrationName { get; set; }
    public string IntegrationUserId { get; set; }
}