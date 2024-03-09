using AntiClown.Data.Api.Core.Rights.Domain;
using AntiClown.Data.Api.Dto.Rights;
using AutoMapper;

namespace AntiClown.Data.Api.Mappings;

public class RightsMapperProfile : Profile
{
    public RightsMapperProfile()
    {
        CreateMap<RightsDto, Rights>().ReverseMap();
    }
}