using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Models.Classes.Items;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.Classes
{
    public class Inventory: IEnumerable<BaseItem>
    {
        private readonly List<BaseItem> _items;

        public Inventory()
        {
            _items = new List<BaseItem>();
        }

        public Inventory(List<BaseItem> items)
        {
            _items = items;
        }

        public bool TryAdd<T>(T item, out AddItemToInventoryOperationResult operationResult) where T: BaseItem
        {
            if (OfType<T>().Count() >= NumericConstants.MaximumItemsOfOneType)
            {
                operationResult = AddItemToInventoryOperationResult.TooManyItemsOfType;
                return false;
            }
            _items.Add(item);
            operationResult = AddItemToInventoryOperationResult.Success;
            return true;
        }

        public bool RemoveRandomItemOfType<T>()
        {
            var itemsOfType = OfType<T>().ToList();
            return itemsOfType.Any() && _items.Remove(itemsOfType.SelectRandomItem() as BaseItem);
        }

        private IEnumerable<T> OfType<T>() => _items.OfType<T>();

        public IEnumerator<BaseItem> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public enum AddItemToInventoryOperationResult
    {
        Success,
        TooManyItemsOfType
    }
}