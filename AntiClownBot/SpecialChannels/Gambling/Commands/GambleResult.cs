using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleResult : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleResult(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "result";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble == null)
            {
                return "В данный момент нет активной ставки";
            }

            var message = e.Message.Content;
            var options = message.Split('\n').Skip(1).ToArray();
            var result = Config.CurrentGamble.MakeGambleResult(e.Author.Id, options);
            switch (result)
            {
                case GambleBetResult.OptionDoesntExist:
                    return "Одного из вариантов не существует, он не может быть верным";
                case GambleBetResult.UserIsNotAuthor:
                    user.ChangeRating(-30);
                    return $"Ты кто такой, чел? Держи -30 {Utility.StringEmoji(":PogOff:")}";
                case GambleBetResult.SuccessfulResult:
                    var tempMessage = Config.CurrentGamble.Result;
                    Config.CurrentGamble = null;
                    return tempMessage;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
