/* Generated file */
namespace AntiClown.Api.Client.Lohotron;

public interface ILohotronClient
{
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.LohotronRewardDto> UseLohotronAsync(System.Guid userId);
    System.Threading.Tasks.Task ResetAsync();
}
