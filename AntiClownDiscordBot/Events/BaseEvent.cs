using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownBot.Events
{
    public abstract class BaseEvent
    {
        protected readonly Configuration Config;
        protected static DiscordClient DiscordClient;
        public List<BaseEvent> RelatedEvents;

        protected BaseEvent()
        {
            Config = Configuration.GetConfiguration();
            RelatedEvents = new List<BaseEvent>();
        }

        public static void SetDiscordClient(DiscordClient client)
        {
            DiscordClient = client;
        }

        public abstract void ExecuteAsync();

        protected abstract string BackStory();

        protected async Task<DiscordMessage> TellBackStory()
        {
            return await Utility.SendMessageToBotChannel(BackStory());
        }

        public bool HasRelatedEvents() => RelatedEvents.Count != 0;
    }
}