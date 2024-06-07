/* Generated file */
namespace AntiClown.Entertainment.Api.Client.ActiveEventsIndex;

public interface IActiveEventsIndexClient
{
    System.Threading.Tasks.Task<Dictionary<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto, System.Boolean>> ReadAllEventTypesAsync();
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto[]> ReadActiveEventsAsync();
    System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto);
    System.Threading.Tasks.Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto);
    System.Threading.Tasks.Task ActualizeIndexAsync();
}
