using ApiWrapper.Models.Items;

namespace AntiClownBot.Models.Inventory
{
    public class UserInventoryPage
    {
        public string ItemType { get; }
        public string PageDescription { get; }
        public BaseItem[] PageItems { get; }
        
        public UserInventoryPage(string itemType, string pageDescription, BaseItem[] items)
        {
            ItemType = itemType;
            PageDescription = pageDescription;
            PageItems = items;
        }
    }
}