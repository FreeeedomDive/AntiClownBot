﻿using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Transfusion;
using AntiClown.Entertainment.Api.Dto.CommonEvents;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings.Events;

public class CommonEventsMapperProfile : Profile
{
    public CommonEventsMapperProfile()
    {
        CreateMap<CommonEventType, CommonEventTypeDto>().ReverseMap();
        CreateMap<GuessNumberPick, GuessNumberPickDto>();
        CreateMap<GuessNumberEvent, GuessNumberEventDto>();

        CreateMap<LotterySlot, LotterySlotDto>();
        CreateMap<LotteryParticipant, LotteryParticipantDto>();
        CreateMap<LotteryEvent, LotteryEventDto>();

        CreateMap<RemoveCoolDownsEvent, RemoveCoolDownsEventDto>();

        CreateMap<TransfusionEvent, TransfusionEventDto>();

        CreateMap<RaceTrack, RaceTrackDto>().ReverseMap();
        CreateMap<RaceDriver, RaceDriverDto>().ReverseMap();
        CreateMap<RaceParticipant, RaceParticipantDto>();
        CreateMap<RaceSectorType, RaceSectorTypeDto>();
        CreateMap<RaceSnapshotForDriverOnSector, RaceSnapshotForDriverOnSectorDto>();
        CreateMap<RaceSnapshotOnSector, RaceSnapshotOnSectorDto>();
        CreateMap<RaceEvent, RaceEventDto>();
    }
}