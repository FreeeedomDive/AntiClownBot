using System;
using System.IO;

namespace AntiClownBot.Transactions
{
    public class TransactionLog
    {
        public static async void AddLog(SocialRatingUser user, string data)
        {
            await using var file = new StreamWriter("transactions.txt", true);
            await file.WriteLineAsync($"{DateTime.Now} | {user.DiscordUsername} --- {data}");
        }
    }
}