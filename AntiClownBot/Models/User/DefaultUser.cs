using AntiClownBot.Models.User.Inventory;
using AntiClownBot.Models.User.Inventory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User
{
    public class DefaultUser
    {
        public string DiscordUserName;
        public ulong DiscordId;
        public Dictionary<Item, int> Items;
        public DefaultUser()
        {
            Items = new Dictionary<Item, int>
            {
                { new CatWife(), 0 }
            };
            
        }
        public string AddItem(Item item)
        {
            Items[item]++;
            return "";
        }
    }
}
