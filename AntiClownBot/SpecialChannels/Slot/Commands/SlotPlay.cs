using AntiClownBot.Models.SlotMachine;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.Slot.Commands
{
    public class SlotPlay : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public SlotPlay(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "play";
        private SlotMachine slotMachine = new();

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var splitMessage = e.Message.Content.Split(' ');
            if (splitMessage.Length != 2
                || !int.TryParse(splitMessage[1], out var bet)
                || bet <= 4
                || bet > user.SocialRating)
            {
                return "чел ты кого наебать вздумал";
            }

            var result = slotMachine.Play(bet);
            var textCells = string.Join(" ", result.Cells.Select(c => Utility.StringEmoji(c.Emoji)));

            var resultRatingChange = result.Win - bet;
            user.ChangeRating(resultRatingChange);

            return "Кручу верчу богатство принести хочу:\n" + textCells + "\nВыигрыш: " + result.Win;
        }
    }
}
