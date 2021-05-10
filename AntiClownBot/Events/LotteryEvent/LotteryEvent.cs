using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiClownBot.Events.LotteryEvent
{
    class LotteryEvent : BaseEvent
    {
        public override void Execute()
        {
            Config.CurrentLottery = new Models.Lottery.Lottery();
        }

        protected override string BackStory()
        {
            return "Начинаем Лотерею! Присоединяйтесь через !lottery, стоимость билета 250.\n" +
                "Но помните, вы можете не только выиграть большое количество кредитов, но и проиграть.\n" +
                "Начнём подводить итоги через 20 минут";
        }
    }
}
