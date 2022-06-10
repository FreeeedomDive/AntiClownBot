using System;
using System.Collections.Generic;

namespace ApiWrapper.Models.Items
{
    public abstract class BaseItem
    {
        public BaseItem(Guid id)
        {
            Id = id;
        }
        
        public Guid Id { get; }
        public abstract string Name { get; }
        public abstract ItemType ItemType { get; } 
        public Rarity Rarity { get; init; }
        public int Price { get; set; }
        public bool IsActive { get; set; }

        public abstract Dictionary<string, string> Description();
    }
}