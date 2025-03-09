using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

[Index(nameof(UserId), nameof(Season))]
public class F1BingoBoardStorageElement : SqlStorageElement
{
    public Guid UserId { get; set; }
    public int Season { get; set; }
    public Guid[] Cards { get; set; }
}