using AntiClown.DiscordBot.Ai.Client;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Other;

public class AiCommandModule(ICommandExecutor commandExecutor, IAiClient aiClient) : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.Ai, "Задать вопрос ИИ")]
    public async Task ChangeNickname(
        InteractionContext context,
        [Option("request", "Запрос")]
        string request
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var response = await aiClient.GetResponseAsync(request);
                await RespondToInteractionAsync(context, string.IsNullOrEmpty(response) ? "Ответа нет :(" : response);
            }
        );
    }
}