﻿using System.ComponentModel;
using System.Globalization;

namespace AntiClownDiscordBotVersion2.Models.Gamble
{
    [TypeConverter(typeof(GambleOptionTypeConverter))]
    public class GambleOption
    {
        public readonly string Name;
        public readonly double Ratio;

        public GambleOption(string name, double ratio)
        {
            Name = name;
            Ratio = ratio;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not GambleOption option) return false;
            return Name == option.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() * 3571;
        }

        public override string ToString()
        {
            return $"{Name} : {Ratio}";
        }
    }

    [TypeConverter(typeof(GambleOptionTypeConverter))]
    public class GambleOptionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context,
            Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is not string serialized) return base.ConvertFrom(context, culture, value);

            var args = serialized.Split(':');
            var name = args[0][..^1];
            var ratio = double.Parse(args[1][1..]);
            return new GambleOption(name, ratio);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(string) ? value?.ToString() : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}