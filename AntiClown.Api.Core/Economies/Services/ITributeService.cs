using AntiClown.Api.Core.Economies.Domain;

namespace AntiClown.Api.Core.Economies.Services;

public interface ITributeService
{
    Task<Tribute> MakeTributeAsync(Guid userId);
    Task<DateTime> WhenNextTributeAsync(Guid userId);
}