namespace ApiWrapper.Responses
{
    public static class Enums
    {
        public enum BuyResult
        {
            Success,
            NotEnoughMoney,
            AlreadyBought,
            ItemDoesntExistInShop,
            TooManyItemsOfSelectedType
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
            TooManyActiveItems,
            NegativeItemCantBeInactive
        }

        public enum SellItemResult
        {
            Success,
            NotEnoughMoney
        }
    }
}