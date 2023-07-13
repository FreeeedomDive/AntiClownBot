using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;
using AntiClown.Messages.Dto.Events.Daily;
using AutoMapper;
using Newtonsoft.Json;

namespace AntiClown.Entertainment.Api.Core.Mappings;

public class DailyEventsMapperProfile : Profile
{
    public DailyEventsMapperProfile()
    {
        CreateMap<DailyEventBase, DailyEventStorageElement>()
            .ForMember(
                se => se.Type,
                cfg => cfg.MapFrom(dailyEvent => dailyEvent.Type.ToString())
            )
            .ForMember(
                se => se.Details,
                cfg => cfg.MapFrom(dailyEvent => JsonConvert.SerializeObject(dailyEvent, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    }))
            );
        CreateMap<DailyEventType, DailyEventTypeDto>();
        CreateMap<DailyEventBase, DailyEventMessageDto>()
            .ForMember(
                dto => dto.EventId,
                cfg => cfg.MapFrom(eventModel => eventModel.Id)
            )
            .ForMember(
                dto => dto.EventType,
                cfg => cfg.MapFrom(eventModel => eventModel.Type)
            );
    }
}