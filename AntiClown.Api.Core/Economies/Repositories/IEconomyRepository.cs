using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Domain.MassActions;

namespace AntiClown.Api.Core.Economies.Repositories;

public interface IEconomyRepository
{
    Task<Economy> ReadAsync(Guid id);
    Task CreateAsync(Economy economy);
    Task UpdateAsync(Economy economy);
    Task UpdateAllAsync(MassEconomyUpdate massEconomyUpdate);
}