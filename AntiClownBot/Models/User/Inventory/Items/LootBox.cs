using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory.Items
{
    public class LootBox : Item
    {
        public override string Name => "Добыча коробка";
        public override int Price => 750;
        
        public override string Use(SocialRatingUser user)
        {
            var item = user.Items.Keys.SelectRandomItem();
            user.AddCustomItem(item);
            return $"{user.DiscordUsername} получил из \" Добыча коробка \" {item.Name}";
        }
    }
}
