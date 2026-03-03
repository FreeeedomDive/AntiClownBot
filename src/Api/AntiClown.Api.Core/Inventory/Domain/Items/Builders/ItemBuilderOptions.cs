using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public class ItemBuilderOptions
{
    public Rarity? Rarity { get; set; }
    public int? CustomPrice { get; set; }

    /// <summary>
    ///     Явное указание Name имеет больший приоритет, чем явное указание Type
    ///     Если указать Name, то будет создан предмет именно с именем Name, Type будет проигнорирован
    ///     Если указать Type, будет создан случайный предмет с типом Type
    /// </summary>
    public ItemType? Type { get; set; }

    /// <summary>
    ///     Явное указание Name имеет больший приоритет, чем явное указание Type
    ///     Если указать Name, то будет создан предмет именно с именем Name, Type будет проигнорирован
    ///     Если указать Type, будет создан случайный предмет с типом Type
    /// </summary>
    public ItemName? Name { get; set; }
}