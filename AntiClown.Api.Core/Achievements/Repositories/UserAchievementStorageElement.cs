using System.ComponentModel.DataAnnotations.Schema;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Achievements.Repositories;

public class UserAchievementStorageElement : SqlStorageElement
{
    [Column] public Guid AchievementId { get; set; }
    [Column] public Guid UserId { get; set; }
    [Column] public DateTime GrantedAt { get; set; }
}
