using DSharpPlus;
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
            await e.Message.RespondAsync($"Ты иметь {user.SocialRating} социальный рейтинг, " +
                                         $"{user.UserItems[InventoryItem.CatWife]} {Utility.ItemToString(InventoryItem.CatWife)}, " +
                                         $"{user.UserItems[InventoryItem.DogWife]} {Utility.ItemToString(InventoryItem.DogWife)}, " +
                                         $"{user.UserItems[InventoryItem.RiceBowl]} {Utility.ItemToString(InventoryItem.RiceBowl)}, " +
                                         $"{user.UserItems[InventoryItem.Gigabyte]} {Utility.ItemToString(InventoryItem.Gigabyte)}, " +
                                         $"{user.UserItems[InventoryItem.JadeRod]} {Utility.ItemToString(InventoryItem.JadeRod)}");
        }

        public override string Help()
        {
            return $"Получение своего социального рейтинга";
        }
    }
}