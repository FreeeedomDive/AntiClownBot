using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

[Index(nameof(Season))]
public class F1BingoCardStorageElement : SqlStorageElement
{
    public int Season { get; set; }
    public string Description { get; set; }
    public string? Explanation { get; set; }
    public string Probability { get; set; }
    public int TotalRepeats { get; set; }
    public int CompletedRepeats { get; set; }
    public bool IsCompleted { get; set; }
}