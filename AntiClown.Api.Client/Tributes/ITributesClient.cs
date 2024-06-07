/* Generated file */
namespace AntiClown.Api.Client.Tributes;

public interface ITributesClient
{
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.NextTributeDto> WhenNextTributeAsync(System.Guid userId);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.TributeDto> TributeAsync(System.Guid userId);
}
