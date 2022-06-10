using NLog;

namespace AntiClownBot
{
    public static class NLogWrapper
    {
        private static Logger transactionsLogger;
        private static Logger defaultLogger;

        public static Logger GetTransactionsLogger() => transactionsLogger ??= LogManager.GetLogger("Transactions");
        public static Logger GetDefaultLogger() => defaultLogger ??= LogManager.GetLogger("Default");
    }
}