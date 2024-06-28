/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.ActiveDailyEventsIndex;

public interface IActiveDailyEventsIndexClient
{
    Task<Dictionary<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto, bool>> ReadAllEventTypesAsync();
    Task<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto[]> ReadActiveEventsAsync();
    Task CreateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto);
    Task UpdateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto);
    Task ActualizeIndexAsync();
}
