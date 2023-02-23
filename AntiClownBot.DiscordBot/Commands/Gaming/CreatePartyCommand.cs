using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.Gaming
{
    public class CreatePartyCommand : ICommand
    {
        public CreatePartyCommand(
            IDiscordClientWrapper discordClientWrapper,
            IPartyService partyService,
            IGuildSettingsService guildSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.partyService = partyService;
            this.guildSettingsService = guildSettingsService;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var guildSettings = guildSettingsService.GetGuildSettings();
            if (e.Channel.Id != guildSettings.PartyChannelId && e.Channel.Id != guildSettings.HiddenTestChannelId)
            {
                await discordClientWrapper.Messages.RespondAsync(
                    e.Message,
                    $"{await discordClientWrapper.Emotes.FindEmoteAsync("Madge")} " +
                    $"{await discordClientWrapper.Emotes.FindEmoteAsync("point_right")} " +
                    $"{(await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.PartyChannelId)).Mention}"
                );
                return;
            }

            var args = e.Message.Content.Split(" ");
            if (args.Length == 1)
            {
                await discordClientWrapper.Messages.RespondAsync(
                    e.Message,
                    $"Не вижу название игры {await discordClientWrapper.Emotes.FindEmoteAsync("modCheck")}"
                );
                return;
            }

            switch (args[1].ToLower())
            {
                case "all":
                    await partyService.AddPartyObserverMessage(e.Message);
                    return;
                case "dota":
                    await partyService.CreateNewParty(e.Author.Id, "Dota", 5, guildSettings.DotaRoleId);
                    break;
                case "csgo":
                    await partyService.CreateNewParty(e.Author.Id, "CS GO", 5, guildSettings.CsRoleId);
                    break;
                case "sigame":
                    await partyService.CreateNewParty(e.Author.Id, "ДЕРЖУ ИГРУ", 7, guildSettings.SiGameRoleId);
                    break;
                case "test":
                    await partyService.CreateNewParty(e.Author.Id, "тест", 2, 277847491959980032);
                    break;
                default:
                    if (args.Length < 3 || !int.TryParse(args[^1], out var count))
                    {
                        await discordClientWrapper.Messages.RespondAsync(e.Message,
                            $"Не вижу количества игроков {await discordClientWrapper.Emotes.FindEmoteAsync("modCheck")}");
                        return;
                    }

                    if (count < 1)
                    {
                        await discordClientWrapper.Messages.RespondAsync(e.Message,
                            $"Ага как скажешь {await discordClientWrapper.Emotes.FindEmoteAsync("Agakakskagesh")}");
                        return;
                    }

                    await partyService.CreateNewParty(e.Author.Id, string.Join(" ", args.Skip(1).Take(args.Length - 2)), count);
                    await discordClientWrapper.Messages.DeleteAsync(e.Message);
                    break;
            }
        }

        public async Task<string> Help()
        {
            return "Сбор пати на игру\nИспользование:\n!party [all | cs | dota | sigame | custom] " +
                   "[количество игроков (если custom)]" +
                   "\nПараметр all - показать все текущие открытые пати" +
                   "\nВсе пати удаляются при перезапуске бота" +
                   $"\n{await discordClientWrapper.Emotes.FindEmoteAsync("YEP")} для присоединения к пати, " +
                   $"{await discordClientWrapper.Emotes.FindEmoteAsync("MEGALUL")} для закрытия (только для создателя пати)";
        }

        public string Name => "party";
        public bool IsObsolete => true;

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IPartyService partyService;
        private readonly IGuildSettingsService guildSettingsService;
    }
}