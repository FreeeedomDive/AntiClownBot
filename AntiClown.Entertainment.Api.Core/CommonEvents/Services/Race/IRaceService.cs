﻿using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;

public interface IRaceService : IBaseEventService<RaceEvent>
{
    Task AddParticipantAsync(Guid eventId, Guid userId);
    Task FinishAsync(Guid eventId);
}