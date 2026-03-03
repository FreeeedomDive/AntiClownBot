using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Data.Api.Core.SettingsStoring.Repositories;

[Index(nameof(Category))]
[Index(nameof(Name))]
public class SettingStorageElement : SqlStorageElement
{
    public string Category { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}