using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleCancel : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleCancel(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "cancel";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble == null)
            {
                return "В данный момент нет активной ставки";
            }

            if (e.Author.Id != Config.CurrentGamble.CreatorId)
            {
                user.ChangeRating(-30);
                return $"Ты кто такой, чел? Держи -30 {Utility.StringEmoji(":PogOff:")}";
            }

            Config.CurrentGamble = null;
            Config.Save();
            return "Ставка была отменена";
        }
    }
}
