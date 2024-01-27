using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.MinecraftRegister;

public class MinecraftRegisterCommand : SlashCommandModuleWithMiddlewares
{
    public MinecraftRegisterCommand(ICommandExecutor commandExecutor,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IUsersCache usersCache) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.MineRegistration,
        "Регистрация на майнкрафт сервере. НЕ юзай свои обычные пароли, дискорд не сильно защищает передачу сообщений на бэк. Используй эту же команду для смены пароля у пользователя. Разрешена регистрация нескольких пользователей.")]
    public async Task Register(InteractionContext context,
        [Option("login", "Ник какой хочешь")] string login,
        [Option("password", "Пароль(придумай новый, не тот, который используешь на других сайтах)")]
        string password)
    {
        await ExecuteEphemeralAsync(context, async () =>
        {
            if (login.Length < 3 || login.Length > 16 || password.Length < 4 || password.Length > 50)
            {
                await RespondToInteractionAsync(context, "Пошёл нахуй");
                return;
            }
            
            var result = await antiClownEntertainmentApiClient.MinecraftRegisterClient.Register(new RegisterRequest
            {
                DiscordId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id),
                Username = login,
                Password = password
            });

            if (!result)
            {
                await RespondToInteractionAsync(context,
                    "Произошёл прикол, регистрация отменяется. Возможно чел с таким ником уже зареган другим юзером дискорда, а возможно всё поломалось нахуй");
                return;
            }

            await RespondToInteractionAsync(context, "Поздравляю брат, ты смог зарегистрироваться, брат.");
        });
    }


    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IUsersCache usersCache;
}