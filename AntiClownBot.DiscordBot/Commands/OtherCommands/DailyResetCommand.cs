﻿using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands;

public class DailyResetCommand : ICommand
{
    public DailyResetCommand(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        Models.Lohotron.Lohotron lohotron,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.lohotron = lohotron;
        this.guildSettingsService = guildSettingsService;
    }

    public async Task Execute(MessageCreateEventArgs e)
    {
        if (e.Author.Id != guildSettingsService.GetGuildSettings().AdminId)
        {
            await discordClientWrapper
                .Messages
                .RespondAsync(
                    e.Message,
                    $"Пошел нахуй {await discordClientWrapper.Emotes.FindEmoteAsync("xdd")}"
                );
            return;
        }

        await apiClient.Users.DailyResetAsync();
        lohotron.Reset();
        await discordClientWrapper.Messages.RespondAsync(e.Message, "done");
    }

    public Task<string> Help()
    {
        return Task.FromResult("Ручной ежедневный сброс");
    }

    public string Name => "dailyreset";

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly Models.Lohotron.Lohotron lohotron;
    private readonly IGuildSettingsService guildSettingsService;
}