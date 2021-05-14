using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Lohotron
{
    public class ItemLohotronPrize : ILohotronPrize
    {
        public string Name { get => "Item";}
        public InventoryItem Item;
        public ItemLohotronPrize(InventoryItem item)
        {
            Item = item;
        }
    }
}
