using AntiClownApiClient.Dto.Responses.UserCommandResponses;

namespace AntiClownApiClient.UsersClient;

public interface IUsersClient
{
    Task<TributeResponseDto> TributeAsync(ulong userId);
    Task<WhenNextTributeResponseDto> WhenNextTributeAsync(ulong userId);
    Task<RatingResponseDto> RatingAsync(ulong userId);
    Task<ChangeUserBalanceResponseDto> ChangeUserRatingAsync(ulong userId, int ratingDiff, string reason);
    Task<BulkChangeUserBalanceResponseDto> BulkChangeUserBalanceAsync(List<ulong> users, int ratingDiff, string reason);
    Task<AllUsersResponseDto> GetAllUsersAsync();
    Task RemoveCooldownsAsync();
    Task<ulong> GetRichestUserAsync();
    Task<ItemResponseDto> GetItemByIdAsync(ulong userId, Guid itemId);
    Task DailyResetAsync();
}