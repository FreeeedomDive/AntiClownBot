using System.Text.RegularExpressions;
using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands;

public class ChangeNicknameCommand : ICommand
{
    public ChangeNicknameCommand(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IUserBalanceService userBalanceService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.userBalanceService = userBalanceService;
    }

    public async Task Execute(MessageCreateEventArgs e)
    {
        var args = e.Message.Content.Split();
        if (args.Length < 2)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, "хуйня");
            return;
        }

        const string pattern = "<@!?(\\d+)>";
        var regex = new Regex(pattern);
        var matches = regex.Matches(args[1]);

        if (matches.Count == 0)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, "хуйня");
            return;
        }

        var match = matches.First().Groups[1].Value;
        var newNickname = string.Join(" ", args.Skip(2));
        var id = ulong.Parse(match);
        var member = await discordClientWrapper.Members.GetAsync(id);
        var selfChanging = e.Author.Id == id;
        var cost = selfChanging ? -1000 : -2000;
        var balance = (await apiClient.Users.RatingAsync(e.Author.Id)).ScamCoins;
        if (balance < -cost)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, "Недостаточно скам койнов для совершения операции");
            return;
        }

        try
        {
            await discordClientWrapper.Members.ModifyAsync(member, model => { model.Nickname = newNickname; });
            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(e.Author.Id, cost, $"Изменение никнейма пользователю {id}");
        }
        catch
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, "Произошла неведомая хуйня");
            return;
        }

        await discordClientWrapper.Messages.RespondAsync(e.Message, $"{await discordClientWrapper.Emotes.FindEmoteAsync("YEP")}");
    }

    public Task<string> Help()
    {
        return Task.FromResult(
            "Позволяет поменять никнейм другому человеку за 2к скам койнов или себе за 1к\nИспользование: !nickname [@чел] [новый ник]"
        );
    }

    public string Name => "nickname";
    public bool IsObsolete => false;

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IUserBalanceService userBalanceService;
}