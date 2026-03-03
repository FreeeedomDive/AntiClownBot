/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Api.Client.Tributes;

public interface ITributesClient
{
    Task<AntiClown.Api.Dto.Economies.NextTributeDto> WhenNextTributeAsync(System.Guid userId);
    Task<AntiClown.Api.Dto.Economies.TributeDto> TributeAsync(System.Guid userId);
}
