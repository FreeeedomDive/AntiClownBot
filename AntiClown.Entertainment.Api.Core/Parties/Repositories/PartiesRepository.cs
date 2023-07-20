using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AutoMapper;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.Parties.Repositories;

public class PartiesRepository : IPartiesRepository
{
    public PartiesRepository(
        IVersionedSqlRepository<PartyStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<Party> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return JsonConvert.DeserializeObject<Party>(result.SerializedParty)!;
    }

    public async Task<Party[]> ReadOpenedAsync()
    {
        var result = await sqlRepository.FindAsync(x => x.IsOpened);
        return result.Select(x => JsonConvert.DeserializeObject<Party>(x.SerializedParty)!).ToArray();
    }

    public async Task<Party[]> ReadFullPartiesAsync()
    {
        var result = await sqlRepository.FindAsync(x => x.FirstFullPartyAt != null);
        return result.Select(x => JsonConvert.DeserializeObject<Party>(x.SerializedParty)!).ToArray();
    }

    public async Task<Guid> CreateAsync(Party party)
    {
        var storageElement = mapper.Map<PartyStorageElement>(party);
        await sqlRepository.CreateAsync(storageElement);

        return storageElement.Id;
    }

    public async Task UpdateAsync(Party party)
    {
        var current = await sqlRepository.ReadAsync(party.Id);
        var newStorageElement = mapper.Map<PartyStorageElement>(party);
        await sqlRepository.ConcurrentUpdateAsync(
            party.Id, x =>
            {
                x.IsOpened = newStorageElement.IsOpened;
                x.SerializedParty = newStorageElement.SerializedParty;
                x.FirstFullPartyAt = newStorageElement.FirstFullPartyAt;
                x.Version = current.Version;
            }
        );
    }

    private readonly IMapper mapper;
    private readonly IVersionedSqlRepository<PartyStorageElement> sqlRepository;
}