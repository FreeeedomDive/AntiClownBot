using AntiClown.Core.Dto.Exceptions;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Data.Api.Dto.Exceptions;

public class SettingNotFoundException : NotFoundException
{
    public SettingNotFoundException(string category, string name) : base($"Can't find setting {name} in category {category}")
    {
        Category = category;
        Name = name;
    }

    public string Category { get; set; }
    public string Name { get; set; }
}