using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Voice;

public interface IVoiceClient
{
    Task<DiscordVoiceState[]> GetVoiceStatesInChannelAsync(ulong channelId);
    Task<bool> TryConnectAsync(DiscordChannel channel);
    Task DisconnectAsync();
    Task PlaySoundAsync(string soundName);
    bool IsPlaying();
    Task WaitForPlaybackFinishAsync();
    void ClearQueue();
    VoiceNextExtension GetVoice();
}