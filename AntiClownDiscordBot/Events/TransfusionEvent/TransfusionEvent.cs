namespace AntiClownBot.Events.TransfusionEvent
{
    public class TransfusionEvent : BaseEvent
    {

        public override async void ExecuteAsync()
        {
            var theRichestUser = ApiWrapper.Wrappers.UsersWrapper.GetRichestUser();

            var exchange = Randomizer.GetRandomNumberBetween(50, 100);
            var exchangeUser = theRichestUser;
            while (exchangeUser == theRichestUser)
            {
                exchangeUser = ApiWrapper.Wrappers.UsersWrapper.GetAllUsers().Users.SelectRandomItem();
            }

            var richestMember = Configuration.GetServerMember(theRichestUser);
            var exchangeMember = Configuration.GetServerMember(exchangeUser);
            
            await Utility.SendMessageToBotChannel("Я решил выделить немного кредитов рандомному челу, " +
                                 "но свой бюджет я тратить не буду, возьму из кармана самого богатого " +
                                 $"{Utility.StringEmoji(":MEGALUL:")} {Utility.StringEmoji(":point_right:")} {richestMember.Nickname}. " +
                                 $"Отдай {exchangeMember.Nickname} {exchange} social credits");

            Config.ChangeBalance(theRichestUser, -exchange, "Эвент перекачки");
            Config.ChangeBalance(exchangeUser, exchange, "Эвент перекачки");
        }

        protected override string BackStory()
        {
            return "";
        }
    }
}