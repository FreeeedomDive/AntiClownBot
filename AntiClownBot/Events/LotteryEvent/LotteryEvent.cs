namespace AntiClownBot.Events.LotteryEvent
{
    class LotteryEvent : BaseEvent
    {
        public override void Execute()
        {
            TellBackStory();
            Config.CurrentLottery = new Models.Lottery.Lottery();
        }

        protected override string BackStory()
        {
            return "Начинаем Лотерею! Присоединяйтесь через !lottery\n" +
                "Но помните, вы можете не только выиграть большое количество кредитов, но и проиграть.\n" +
                "Начнём подводить итоги через 20 минут";
        }
    }
}
