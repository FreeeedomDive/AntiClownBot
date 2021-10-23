namespace AntiClownBotApi.DTO.Responses
{
    public static class Enums
    {
        public enum BuyResult
        {
            Success,
            NotEnoughMoney,
            AlreadyBought,
            ItemDoesntExistInShop
        }

        public enum RevealResult
        {
            Success,
            NotEnoughMoney,
            AlreadyRevealed,
            AlreadyBought,
            ItemDoesntExistInShop
        }

        public enum ReRollResult
        {
            Success,
            NotEnoughMoney
        }

        public enum SetActiveStatusForItemResult
        {
            Success,
            NegativeItemCantBeInactive
        }

        public enum SellItemResult
        {
            Success,
            NotEnoughMoney
        }
    }
}