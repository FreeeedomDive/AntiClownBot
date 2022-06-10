using AntiClownApiClient.Dto.Models.Items;
using AntiClownApiClient.Dto.Responses.UserCommandResponses;

namespace AntiClownApiClient.ItemsClient;

public interface IItemsClient
{
    Task<List<BaseItem>> AllItemsAsync(ulong userId);
    Task<SetActiveStatusForItemResponseDto> SetActiveStatusForItemAsync(ulong userId, Guid itemId, bool isActive);
    Task<SellItemResponseDto> SellItemAsync(ulong userId, Guid itemId);
    Task<OpenLootBoxResultDto> OpenLootBoxAsync(ulong userId);
    Task<string> AddLootBoxAsync(ulong userId);
    Task<string> RemoveLootBoxAsync(ulong userId);
}