using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.SpecialChannels
{
    public interface ICommand
    {
        string Name { get; }
        string Execute(MessageCreateEventArgs e, SocialRatingUser user);
    }
}
