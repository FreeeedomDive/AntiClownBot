using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.Tools.Args;

namespace AntiClown.Tools.Tools.Integrations;

public class CreateCustomUserIntegrationTool(
    IAntiClownApiClient antiClownApiClient,
    IArgsProvider argsProvider,
    ILogger<CreateCustomUserIntegrationTool> logger
) : ToolBase(logger)
{
    protected override async Task RunAsync()
    {
        var userId = argsProvider.GetArgument<Guid>("userId");
        var integrationName = argsProvider.GetArgument("integrationName");
        var integrationUserId = argsProvider.GetArgument("integrationUserId");

        await antiClownApiClient.Users.CreateIntegrationAsync(
            new CreateCustomIntegrationDto
            {
                UserId = userId,
                IntegrationName = integrationName,
                IntegrationUserId = integrationUserId,
            }
        );
    }
}