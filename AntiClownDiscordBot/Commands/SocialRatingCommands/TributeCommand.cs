using System;
using System.Linq;
using System.Threading.Tasks;
using AntiClownBot.Models;
using ApiWrapper.Models.Items;
using ApiWrapper.Responses.UserCommandResponses;
using ApiWrapper.Wrappers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class TributeCommand : BaseCommand
    {
        public TributeCommand(DiscordClient discord, Configuration configuration) : base(
            discord, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            if (e.Channel.Id != Constants.TributeChannelId && e.Channel.Id != 879784704696549498 /* чат для тестов */)
            {
                await e.Message.RespondAsync($"{Utility.Emoji(":Madge:")} {Utility.Emoji(":point_right:")} {e.Guild.GetChannel(877994939240292442).Mention}");
                return;
            }
            
            var tributeResult = UsersApi.Tribute(e.Author.Id);
            var embed = Tribute.MakeEmbedForTribute(tributeResult);
            await e.Message.RespondAsync(embed);
        }

        public override string Help()
        {
            return
                "Преподношение императору XI для увеличения (или уменьшения) своего баланса скам койнов\nДефолтный кулдаун 1 час, понижается наличием интернета";
        }
    }
}