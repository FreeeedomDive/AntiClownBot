using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.MinecraftRegister;

[SlashCommandGroup(InteractionsIds.CommandsNames.MinecraftGroup, "Команды для работы с аккаунтом майнкрафта")]
public class MinecraftRegisterCommand : SlashCommandModuleWithMiddlewares
{
    public MinecraftRegisterCommand(ICommandExecutor commandExecutor,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IAntiClownApiClient antiClownApiClient,
        IUsersCache usersCache) : base(commandExecutor)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.antiClownApiClient = antiClownApiClient;
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

            var result = await antiClownEntertainmentApiClient.MinecraftAccount.RegisterAsync(new RegisterRequest
            {
                DiscordId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id),
                Username = login,
                Password = password,
            });

            var registrationMessage = GetMessageByRegistrationStatus(result.SuccessfulStatus);
            await RespondToInteractionAsync(context, registrationMessage);
        });
    }

    [SlashCommand(InteractionsIds.CommandsNames.MineSkin,
        "Установка скина и/или плаща(платно). Бери скины тут: namemc.com")]
    public async Task SetSkin(InteractionContext context,
        [Option("skinUrl", "URL скина. Цена: 2000. Бери ссылку на картинку тут: namemc.com")]
        string? skinUrl = null,
        [Option("capeUrl", "URL плаща. Цена: 10000. Бери ссылку на картинку тут: namemc.com")]
        string? capeUrl = null)
    {
        await ExecuteAsync(context, async () =>
        {
            if (skinUrl == null && capeUrl == null)
            {
                await RespondToInteractionAsync(context, "Не указана ни одна из ссылок");
                return;
            }

            var discordUserId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
            var economy = await antiClownApiClient.Economy.ReadAsync(discordUserId);

            if (await IsPlayerBeggar(context, skinUrl, capeUrl, economy))
                return;

            var hasAccount = await antiClownEntertainmentApiClient.MinecraftAccount.HasRegistrationByDiscordUserAsync(discordUserId);
            if (!hasAccount.HasRegistration)
            {
                await RespondToInteractionAsync(context, "Сначала зарегайся лол");
                return;
            }

            await antiClownEntertainmentApiClient.MinecraftAccount.SetSkinAsync(new ChangeSkinRequest
            {
                DiscordUserId = discordUserId,
                SkinUrl = skinUrl,
                CapeUrl = capeUrl,
            });

            var totalPrice = (skinUrl is null ? 0 : SkinPrice) + (capeUrl is null ? 0 : CapePrice);
            await antiClownApiClient.Economy.UpdateScamCoinsAsync(
                discordUserId,
                new UpdateScamCoinsDto
                {
                    UserId = discordUserId,
                    Reason = "Установка скина или плаща",
                    ScamCoinsDiff = -totalPrice,
                });

            await RespondToInteractionAsync(context, "Скины успешно установлены");
        });
    }

    [SlashCommand(InteractionsIds.CommandsNames.MineShowUsers,
        "Посмотреть список зарегистрированных никнеймов игроков")]
    public async Task GetAllNicknames(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                var usernames = await antiClownEntertainmentApiClient.MinecraftAccount.GetAllNicknamesAsync();
                await RespondToInteractionAsync(context,
                    $"Никнеймы зареганных игроков:\n{string.Join('\n', usernames)}");
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.MineHelp,
        "Посмотреть список зарегистрированных никнеймов игроков")]
    public async Task Help(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                await RespondToInteractionAsync(context,
                    $"-reg: Регистрация и редактирование аккаунта. После регистрации меняет ник и пароль. " +
                    $"НЕ рекомендуется использовать пароли, которые используются где то ещё. На бэке пароли хешируются, " +
                    $"но неизвестно как дискорд хранит атрибуты команд у себя на серверах." +
                    $"\n" +
                    $"-skin: Устанавливает или изменяет скин или плащ. Для скинов вставляй ссылки с сайта https://namemc.com/, " +
                    $"другие URL пока что не поддерживаются. На сайте найди интересный скин -> на странице скина " +
                    $"правой кнопкой мыши по значку загрузи скина -> копировать адрес ссылки. " +
                    $"Установка плащей аналогична, но плащи бери тут: https://minecraftcapes.net/" +
                    $"\n" +
                    $"Установка и смена скинов и плащей платные: {SkinPrice} коинов за скин, {CapePrice} коинов за плащ.");
            }
        );
    }

    private static async Task<bool> IsPlayerBeggar(InteractionContext context, string? skinUrl, string? capeUrl,
        EconomyDto economy)
    {
        if (capeUrl != null && economy.ScamCoins < CapePrice)
        {
            await RespondToInteractionAsync(context, "Ты слишком нищий чтобы купить плащ");
            return true;
        }

        if (skinUrl != null && economy.ScamCoins < SkinPrice)
        {
            await RespondToInteractionAsync(context, "Ты слишком нищий чтобы сменить скин");
            return true;
        }

        if (skinUrl != null && capeUrl != null && economy.ScamCoins < SkinPrice + CapePrice)
        {
            await RespondToInteractionAsync(context, "Ты слишком нищий чтобы купить и скин и плащ");
            return true;
        }

        return false;
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
            _ => throw new ArgumentOutOfRangeException(nameof(statusDto), statusDto,
                "Какой то неприкольный статус регистрации майнкрафт аккаунта, лол.")
        };
    }

    private const int SkinPrice = 2000;
    private const int CapePrice = 10000;
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IUsersCache usersCache;
}