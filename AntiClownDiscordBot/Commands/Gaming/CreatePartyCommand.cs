using System;
using System.Linq;
using AntiClownBot.Helpers;
using AntiClownBot.Models.Gaming;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.Gaming
{
    public class BaseGamingCommand : BaseCommand
    {
        public BaseGamingCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            if (e.Channel.Id != Constants.PartyChannelId && e.Channel.Id != 879784704696549498 /* чат для тестов */)
            {
                await e.Message.RespondAsync($"{Utility.Emoji(":Madge:")} {Utility.Emoji(":point_right:")} {e.Guild.GetChannel(878002609217220619).Mention}");
                return;
            }
            var args = e.Message.Content.Split(" ");
            if (args.Length == 1)
            {
                await e.Message.RespondAsync($"Не вижу название игры {Utility.Emoji(":modCheck:")}");
                return;
            }

            var party = new GameParty()
            {
                CreatorId = e.Author.Id,
                CreationDate = DateTime.Now
            };

            switch (args[1].ToLower())
            {
                case "all":
                    Config.AddPartyObserverMessage(e);
                    return;
                case "dota":
                    party.Description = "Dota";
                    party.MaxPlayersCount = 5;
                    party.AttachedRoleId = 785512028931489802;
                    break;
                case "csgo":
                    party.Description = "CS GO";
                    party.MaxPlayersCount = 5;
                    party.AttachedRoleId = 747723060441776238;
                    break;
                case "sigame":
                    party.Description = "ДЕРЖУ ИГРУ";
                    party.MaxPlayersCount = 7;
                    party.AttachedRoleId = 739082045275504710;
                    break;
                default:
                    if (args.Length < 3 || !int.TryParse(args[^1], out var count))
                    {
                        await e.Message.RespondAsync($"Не вижу количества игроков {Utility.Emoji(":modCheck:")}");
                        return;
                    }

                    if (count < 1)
                    {
                        await e.Message.RespondAsync($"Ага как скажешь {Utility.Emoji(":Agakakskagesh:")}");
                        return;
                    }

                    party.Description = string.Join(" ", args.Skip(1).Take(args.Length - 2));
                    party.MaxPlayersCount = count;
                    break;
            }

            await party.CreateMessage(e);
            await party.Message.CreateReactionAsync(Utility.Emoji(":YEP:"));
            await party.Message.CreateReactionAsync(Utility.Emoji(":MEGALUL:"));
            Config.OpenParties.Add(party.MessageId, party);
            Configuration.GetConfiguration().UpdatePartyObservers();
        }

        public override string Help() =>
            "Сбор пати на игру\nИспользование:\n!party [all | cs | dota | custom] [количество игроков (если custom)]\nПараметр all - показать все текущие открытые пати\n" +
            "Все пати удаляются при перезапуске бота\n" +
            $"{Utility.StringEmoji(":YEP:")} для присоединения к пати, {Utility.StringEmoji(":MEGALUL:")} для закрытия (только для создателя пати)";
    }
}