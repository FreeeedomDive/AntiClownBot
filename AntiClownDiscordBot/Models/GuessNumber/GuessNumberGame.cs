using System.Collections.Generic;
using System.Text;
using AntiClownBot.Helpers;

namespace AntiClownBot.Models.GuessNumber
{
    public class GuessNumberGame
    {
        public readonly ulong GuessNumberGameMessageMessageId;
        public readonly Dictionary<ulong, int> Users;
        public bool IsJoinable;
        private readonly int _generatedNumber;

        public void Join(ulong userId, int number)
        {
            if (Users.ContainsKey(userId) || !IsJoinable)
            {
                return;
            }

            Users.Add(userId, number);
        }

        public GuessNumberGame(ulong messageId)
        {
            _generatedNumber = Randomizer.GetRandomNumberBetween(1, 6);
            Users = new Dictionary<ulong, int>();
            GuessNumberGameMessageMessageId = messageId;
            IsJoinable = true;
        }

        public async void MakeResult()
        {
            IsJoinable = false;
            var count = 0;
            var sb = new StringBuilder($"Правильный ответ {_generatedNumber}!\n");
            foreach (var (userId, userGuess) in Users)
            {
                if (userGuess != _generatedNumber) continue;
                
                //TODO - временное решение, пока нет идей с реализацией лутбоксов
                Configuration.GetConfiguration().ChangeBalance(userId, 500, "Угаданное число в эвенте");
                var member = Configuration.GetServerMember(userId);
                sb.Append($"\n{member.ServerOrUserName()} получает 500 Scam-койнов!");
                count++;
            }

            if (count == 0)
            {
                sb.Append($"\nНикто не угадал {Utility.Emoji(":peepoFinger:")}!");
            }
            
            await Utility.Client
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(sb.ToString());
        }
    }
}
