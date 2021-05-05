using System.Collections.Generic;
using DSharpPlus;

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

        public abstract void Execute();

        protected abstract string BackStory();

        protected void TellBackStory()
        {
            SendMessageToChannel(BackStory());
        }

        protected async void SendMessageToChannel(string content)
        {
            await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(content); 
        }

        public bool HasRelatedEvents() => RelatedEvents.Count != 0;
    }
}