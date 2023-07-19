using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Entertainment.Api.Core.Parties.Repositories;
using AutoMapper;
using Newtonsoft.Json;

namespace AntiClown.Entertainment.Api.Core.Mappings;

public class PartiesMapperProfile : Profile
{
    public PartiesMapperProfile()
    {
        CreateMap<Party, PartyStorageElement>()
            .ForMember(
                storageElement => storageElement.SerializedParty,
                cfg => cfg.MapFrom(party => JsonConvert.SerializeObject(party))
            );
    }
}