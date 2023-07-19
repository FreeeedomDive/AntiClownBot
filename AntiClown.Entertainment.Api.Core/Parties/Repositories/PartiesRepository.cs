using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Entertainment.Api.Core.Parties.Services.Messages;
using AutoMapper;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.Parties.Repositories;

public class PartiesRepository : IPartiesRepository
{
    public PartiesRepository(
        IVersionedSqlRepository<PartyStorageElement> sqlRepository,
        IPartiesMessageProducer partiesMessageProducer,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.partiesMessageProducer = partiesMessageProducer;
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
        return mapper.Map<Party[]>(result);
    }

    public async Task<Guid> CreateAsync(Party party)
    {
        var storageElement = mapper.Map<PartyStorageElement>(party);
        await sqlRepository.CreateAsync(storageElement);
        await partiesMessageProducer.ProduceAsync(party);

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
                x.Version = current.Version;
            }
        );
        await partiesMessageProducer.ProduceAsync(party);
    }

    private readonly IMapper mapper;
    private readonly IVersionedSqlRepository<PartyStorageElement> sqlRepository;
    private readonly IPartiesMessageProducer partiesMessageProducer;
}