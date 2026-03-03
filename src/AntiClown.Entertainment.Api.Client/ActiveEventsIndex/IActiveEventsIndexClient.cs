/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.ActiveEventsIndex;

public interface IActiveEventsIndexClient
{
    Task<Dictionary<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto, bool>> ReadAllEventTypesAsync();
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto[]> ReadActiveEventsAsync();
    Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto);
    Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto);
    Task ActualizeIndexAsync();
}
