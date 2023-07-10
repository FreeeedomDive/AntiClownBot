using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Transfusion;
using AntiClown.EntertainmentApi.Dto.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Lottery;
using AntiClown.EntertainmentApi.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Transfusion;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings.Events;

public class CommonEventsMapperProfile : Profile
{
    public CommonEventsMapperProfile()
    {
        CreateMap<GuessNumberPick, GuessNumberPickDto>();
        CreateMap<GuessNumberEvent, GuessNumberEventDto>();

        CreateMap<LotterySlot, LotterySlotDto>();
        CreateMap<LotteryParticipant, LotteryParticipantDto>();
        CreateMap<LotteryEvent, LotteryEventDto>();

        CreateMap<RemoveCoolDownsEvent, RemoveCoolDownsEventDto>();

        CreateMap<TransfusionEvent, TransfusionEventDto>();
    }
}