using System;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.GamblingCommands
{
    public class CancelGambleCommand: BaseCommand
    {
        public CancelGambleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble == null)
            {
                await e.Message.RespondAsync("В данный момент нет активной ставки");
                return;
            }

            if (e.Author.Id != Config.CurrentGamble.CreatorId)
            {
                await e.Message.RespondAsync($"Ты кто такой, чел? Держи -30 {Utility.StringEmoji(":PogOff:")}");
                user.ChangeRating(-30);
                return;
            }

            Config.CurrentGamble = null;
            Config.Save();
            await e.Message.RespondAsync("Ставка была отменена");
        }

        public override string Help()
        {
            return "Отмена текущей ставки без оглашения победившего исхода событий\n" +
                   "Закрыть ставку может только создавший ее пользователь";
        }
    }
}