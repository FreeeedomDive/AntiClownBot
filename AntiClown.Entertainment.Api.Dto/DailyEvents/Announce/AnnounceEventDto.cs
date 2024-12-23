﻿namespace AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;

public class AnnounceEventDto : DailyEventBaseDto
{
    public override DailyEventTypeDto Type => DailyEventTypeDto.Announce;

    public Dictionary<Guid, int> Earnings { get; set; }
}