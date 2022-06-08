using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Party;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownDiscordBotVersion2.SlashCommands.ChoiceProviders
{
    public class JoinableRoleChoiceProvider : IChoiceProvider
    {
        public JoinableRoleChoiceProvider(IDiscordClientWrapper discordClientWrapper, IPartyService partyService)
        {
            this.discordClientWrapper = discordClientWrapper;
            this.partyService = partyService;
        }

        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
        {
            var roles = (await discordClientWrapper.Guilds.GetGuildAsync()).Roles;
            return partyService.PartiesInfo.JoinableRoles.Select(joinableRole =>
            {
                var role = roles[joinableRole];
                return new DiscordApplicationCommandOptionChoice(role.Name, role);
            });
        }


        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IPartyService partyService;
    }
}
