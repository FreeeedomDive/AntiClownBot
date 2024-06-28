/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.DiscordBot.Client.DiscordMembers;

public interface IDiscordMembersClient
{
    Task<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto> GetDiscordMemberAsync(System.Guid userId);
}
