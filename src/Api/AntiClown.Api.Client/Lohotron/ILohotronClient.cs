/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Api.Client.Lohotron;

public interface ILohotronClient
{
    Task<AntiClown.Api.Dto.Economies.LohotronRewardDto> UseLohotronAsync(System.Guid userId);
    Task ResetAsync();
}
