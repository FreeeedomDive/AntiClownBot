using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Messages.Dto.Events.Common;
using AutoMapper;
using Newtonsoft.Json;

namespace AntiClown.Entertainment.Api.Core.Mappings;

public class CommonEventsMapperProfile : Profile
{
    public CommonEventsMapperProfile()
    {
        CreateMap<CommonEventBase, CommonEventStorageElement>()
            .ForMember(
                se => se.Type,
                cfg => cfg.MapFrom(commonEvent => commonEvent.Type.ToString())
            )
            .ForMember(
                se => se.Details,
                cfg => cfg.MapFrom(commonEvent => JsonConvert.SerializeObject(commonEvent, Formatting.Indented))
            );
        CreateMap<CommonEventType, CommonEventTypeDto>();
        CreateMap<CommonEventBase, CommonEventMessageDto>();
    }
}