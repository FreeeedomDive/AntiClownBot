namespace AntiClownBot.Events.LotteryEvent
{
    class LotteryEvent : BaseEvent
    {
        public override async void ExecuteAsync()
        {
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
                "Но помните, вы можете не только выиграть большое количество кредитов, но и проиграть.\n" +
                "Вся необходимая информация о лотерее доступна по команде '!help lottery'\n" +
                "Начнём подводить итоги через 15 минут";
        }
    }
}
