using System.Text;
using AntiClownDiscordBotVersion2.Utils.Extensions;

namespace AntiClownDiscordBotVersion2.Utils
{
    public static class Utility
    {
        public static string NormalizeTime(DateTime dateTime)
        {
            return $"{Normalize(dateTime.Hour)}:{Normalize(dateTime.Minute)}:{Normalize(dateTime.Second)}";
        }

        public static string NormalizeTime(int totalTime)
        {
            var ms = AddLeadingZeros(3, totalTime % 1000);
            totalTime /= 1000;
            var sec = AddLeadingZeros(2, totalTime % 60);
            totalTime /= 60;
            return $"{totalTime}:{sec}.{ms}";
        }

        public static string AddLeadingZeros(int totalNumbers, int time)
        {
            var leadingZerosCount = totalNumbers - time.ToString().Length;
            return $"{"0".Multiply(leadingZerosCount)}{time}";
        }

        private static string Normalize(int number)
        {
            return number < 10 ? $"0{number}" : $"{number}";
        }

        public static string GetTimeDiff(DateTime dateTime)
        {
            return GetTimeDiff(GetTimeSpan(dateTime));
        }

        public static TimeSpan GetTimeSpan(DateTime dateTime)
        {
            return dateTime > DateTime.Now ? dateTime - DateTime.Now : DateTime.Now - dateTime;
        }

        public static string GetTimeDiff(TimeSpan timeSpan)
        {
            var sb = new StringBuilder();
            if (timeSpan.Hours != 0)
                sb.Append(PluralizeString(timeSpan.Hours, "час", "часа", "часов")).Append(' ');
            if (timeSpan.Minutes != 0)
                sb.Append(PluralizeString(timeSpan.Minutes, "минуту", "минуты", "минут")).Append(' ');
            if (timeSpan.Seconds != 0)
                sb.Append(PluralizeString(timeSpan.Seconds, "секунду", "секунды", "секунд"));
            
            return sb.ToString();
        }

        private static string PluralizeString(int count, string singleForm, string severalForm, string manyForm)
        {
            var correctCount = count % 100;
            if (correctCount is >= 10 and <= 20 || correctCount % 10 >= 5 && correctCount % 10 <= 9 ||
                correctCount % 10 == 0)
                return $"{count} {manyForm}";
            return correctCount % 10 == 1 ? $"{count} {singleForm}" : $"{count} {severalForm}";
        }
    }
}