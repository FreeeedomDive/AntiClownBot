using System.Linq;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels.Dev.Commands
{
    public class GiveRating : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        
        public GiveRating(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "give";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var messageArgs = e.Message.Content.Split();
            if (messageArgs.Length != 3) return "хуйня";
            var userIdParsed = ulong.TryParse(messageArgs[1], out var userId);
            if (!userIdParsed) return "хуйня";
            var member = e.Guild.GetMemberAsync(userId).Result;
            var rewardedUser = Config.Users.ContainsKey(userId)
                ? Config.Users[userId]
                : new SocialRatingUser(userId, member.Username);
            var reward = int.Parse(messageArgs[2]);
            rewardedUser.ChangeRating(reward);

            return "done";
        }
    }
}