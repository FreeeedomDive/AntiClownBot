using System.Linq;
using System.Text.RegularExpressions;
using AntiClownBot.Helpers;
using ApiWrapper.Wrappers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.OtherCommands;

public class ChangeNicknameCommand: BaseCommand
{
    public ChangeNicknameCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
    {
    }

    public override async void Execute(MessageCreateEventArgs e)
    {
        var args = e.Message.Content.Split();
        if (args.Length < 2)
        {
            await e.Message.RespondAsync("хуйня");
            return;
        }
        
        const string pattern = "<@!?(\\d+)>";
        var regex = new Regex(pattern);
        var matches = regex.Matches(args[1]);

        if (matches.Count == 0)
        {
            await e.Message.RespondAsync("хуйня");
            return;
        }

        var match = matches.First().Groups[1].Value;
        var newNickname = string.Join(" ", args.Skip(2));
        var id = ulong.Parse(match);
        var member = Configuration.GetServerMember(id);
        var selfChanging = e.Author.Id == id;
        var cost = selfChanging ? -1000 : -2000;
        var balance = UsersApi.Rating(e.Author.Id).ScamCoins;
        if (balance < -cost)
        {
            await e.Message.RespondAsync("Недостаточно скам койнов для совершения операции");
            return;
        }
        try
        {
            await member.ModifyAsync(model => {
                model.Nickname = newNickname;
            });
            Config.ChangeBalance(e.Author.Id, cost, $"Изменение никнейма пользователю {id}");
        }
        catch
        {
            await e.Message.RespondAsync("Произошла неведомая хуйня");
            return;
        }

        await e.Message.RespondAsync($"{Utility.Emoji(":YEP:")}");
    }

    public override string Help()
    {
        return "Позволяет поменять никнейм другому человеку за 2к скам койнов или себе за 1к\nИспользование: !nickname [@чел] [новый ник]";
    }
}