using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class LootBox : Item
    {
        public static new string Name => "Добыча коробка";
        public static new int Price => 750;
        public override string Use(SocialRatingUser user)
        {
            var item = user.Items.SelectRandomItem().Key;
            user.AddCustomItem(item);
            return $"{user.DiscordUsername} получил из \" Добыча коробка \" {item.Name}";
        }
    }
}
