using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.SocialRatingCommands
{
    public class RatingCommand : BaseCommand
    {
        public RatingCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var member = await e.Guild.GetMemberAsync(user.DiscordId);
            var embedBuilder = new DiscordEmbedBuilder
            {
                Color = new Optional<DiscordColor>(DiscordColor.MidnightBlue)
            };
            
            embedBuilder.WithThumbnail(e.Author.AvatarUrl);
            embedBuilder.WithTitle($"Паспорт гражданин {member.Username}");
            
            embedBuilder.AddField("Социальный рейтинг", $"{user.SocialRating}");
            embedBuilder.AddField("Общий рейтинг", $"{user.NetWorth}");
            foreach (var (key, value) in user.Items)
            {
                embedBuilder.AddField($"{key.Name}: {value}", $"{key.Perks}: {key.ItemStatsForUser(user)}");
            }
            
            await e.Message.RespondAsync(embedBuilder.Build());
        }

        public override string Help()
        {
            return "Получение своего социального рейтинга";
        }
    }
}