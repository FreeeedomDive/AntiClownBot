using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace AntiClownBot.Models.User.Inventory
{
    [TypeConverter(typeof(ItemTypeConverter))]
    public abstract class Item
    {
        public abstract string Name { get; }
        public abstract int Price { get; }

        public virtual string Use(SocialRatingUser user)
        {
            return "Это нельзя использовать";
        }

        public virtual void OnItemAddOrRemove(SocialRatingUser user)
        {
            return;
        }
        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            return obj is Item item && item.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            return hash;
        }
    }

    [TypeConverter(typeof(ItemTypeConverter))]
    public class ItemTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string serialized)) return base.ConvertFrom(context, culture, value);

            var allItems = AllItems.GetAllItems().ToList();
            var itemsNames = allItems.Select(item => item.Name).ToList();

            return itemsNames.Contains(serialized) ? allItems.First(item => item.Name == serialized) : null;
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string)
                ? value.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}