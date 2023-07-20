using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Entertainment.Api.Dto.Parties;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings;

public class PartiesMapperProfile : Profile
{
    public PartiesMapperProfile()
    {
        CreateMap<CreatePartyDto, CreateParty>();
        CreateMap<PartyDto, Party>().ReverseMap();
    }
}