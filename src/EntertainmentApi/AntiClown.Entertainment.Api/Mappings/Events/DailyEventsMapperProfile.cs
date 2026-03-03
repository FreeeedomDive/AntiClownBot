using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain.Announce;
using AntiClown.Entertainment.Api.Dto.DailyEvents;
using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings.Events;

public class DailyEventsMapperProfile : Profile
{
    public DailyEventsMapperProfile()
    {
        CreateMap<DailyEventType, DailyEventTypeDto>();
        CreateMap<AnnounceEvent, AnnounceEventDto>();
    }
}