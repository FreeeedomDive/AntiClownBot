/* Generated file */
namespace AntiClown.Entertainment.Api.Client.ActiveDailyEventsIndex;

public interface IActiveDailyEventsIndexClient
{
    System.Threading.Tasks.Task<Dictionary<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto, System.Boolean>> ReadAllEventTypesAsync();
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto[]> ReadActiveEventsAsync();
    System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto);
    System.Threading.Tasks.Task UpdateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto);
    System.Threading.Tasks.Task ActualizeIndexAsync();
}
