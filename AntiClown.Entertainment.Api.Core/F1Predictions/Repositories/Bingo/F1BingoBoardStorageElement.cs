using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

// ID == UserId
public class F1BingoBoardStorageElement : SqlStorageElement
{
    public Guid[] Cards { get; set; }
}