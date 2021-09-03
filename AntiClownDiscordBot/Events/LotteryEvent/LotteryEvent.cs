using System;
using AntiClownBot.Helpers;

namespace AntiClownBot.Events.LotteryEvent
{
    class LotteryEvent : BaseEvent
    {
        public override async void ExecuteAsync()
        {
            if (Config.CurrentLottery != null)
                return;
            Config.CurrentLottery = new Models.Lottery.Lottery();
            Config.Save();
            var text = BackStory();
            var message = await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(text);
            await message.CreateReactionAsync(Utility.Emoji(":NOTED:"));
            Config.CurrentLottery.LotteryMessageId = message.Id;
            Config.Save();
        }

        protected override string BackStory()
        {
            return $"@everyone Начинаем лотерею! Для участия нажмите на смайлик {Utility.StringEmoji(":NOTED:")} под сообщением или через команду !lottery\n" +
                "Здесь можно выиграть много scam-койнов!\n" +
                "Вся необходимая информация о лотерее доступна по команде '!help lottery'\n" +
                "Начнём подводить итоги через 15 минут";
        }
    }
}
