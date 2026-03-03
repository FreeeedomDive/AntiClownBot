using System.ComponentModel.DataAnnotations.Schema;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Achievements.Repositories;

public class AchievementStorageElement : SqlStorageElement
{
    [Column] public string Name { get; set; } = string.Empty;
    [Column] public byte[] Logo { get; set; } = [];
}
