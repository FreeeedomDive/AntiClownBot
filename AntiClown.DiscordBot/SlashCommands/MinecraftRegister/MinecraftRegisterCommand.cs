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
        "Регистрация и редактирование майнкрафт аккаунта. Придумай уникальный пароль!!")]
    public async Task Register(InteractionContext context,
        [Option("login", "Твой ник(любой)")] string login,
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

            var registrationMessage = GetMessageByRegistrationStatus(result);
            await RespondToInteractionAsync(context, registrationMessage);
        });
    }

    private static string GetMessageByRegistrationStatus(RegistrationStatusDto statusDto)
    {
        return statusDto switch
        {
            RegistrationStatusDto.FailedUpdate_NicknameOwnedByOtherUser =>
                "Чел, ты думал сможешь спиздить ник другого чела если отредактируешь свой аккаунт?))) Ты еблан?",
            RegistrationStatusDto.FailedCreate_NicknameOwnedByOtherUser =>
                "Придумай более уникальный ник, такой уже кто то зарегистрировал",
            RegistrationStatusDto.SuccessCreate => "Поздравляю брат, ты смог зарегистрироваться, брат",
            RegistrationStatusDto.SuccessUpdate => "Ты поменял логин и/или пароль своего майнкрафт аккаунта. Молодец.",
            _ => throw new ArgumentOutOfRangeException(nameof(statusDto), statusDto, "Какой то неприкольный статус регистрации майнкрафт аккаунта, лол.")
        };
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IUsersCache usersCache;
}