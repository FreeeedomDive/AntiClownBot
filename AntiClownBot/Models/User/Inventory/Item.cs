using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.User.Inventory
{
    public abstract class Item
    {
        public string Name => "DefaultItem";
        public int Price => 0;
        public virtual string Use(SocialRatingUser user)
        {
            return "Это нельзя использовать";
        }
        public override int GetHashCode()
        {
            return Price.GetHashCode() + Name.GetHashCode();
        }
    }
}
