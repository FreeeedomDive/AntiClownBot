﻿namespace AntiClown.DiscordBot.Models.Interactions;

public static class InteractionsIds
{
    public static class F1PredictionsButtons
    {
        public const string DriversSelectDropdown = "DriversSelectDropdown";
        public const string DriversSelectDropdownItemPrefix = "DriversSelectDropdownItem_";
        public const string StartRaceResultInputButton = "StartRaceResultInputButton";
    }

    public static class ShopButtons
    {
        public const string Prefix = "Shop_";
        public const string ShopButtonItem1 = Prefix + "1";
        public const string ShopButtonItem2 = Prefix + "2";
        public const string ShopButtonItem3 = Prefix + "3";
        public const string ShopButtonItem4 = Prefix + "4";
        public const string ShopButtonItem5 = Prefix + "5";
        public const string ShopButtonReroll = Prefix + "Reroll";
        public const string ShopButtonChangeTool = Prefix + "ChangeTool";
    }

    public static class InventoryButtons
    {
        public const string Prefix = "Inventory_";
        public const string InventoryButton1 = Prefix + "1";
        public const string InventoryButton2 = Prefix + "2";
        public const string InventoryButton3 = Prefix + "3";
        public const string InventoryButton4 = Prefix + "4";
        public const string InventoryButton5 = Prefix + "5";
        public const string InventoryButtonLeft = Prefix + "Left";
        public const string InventoryButtonRight = Prefix + "Right";
        public const string InventoryButtonChangeActiveStatus = Prefix + "Active";
        public const string InventoryButtonSell = Prefix + "Sell";
    }

    public static class EventsButtons
    {
        public static class Race
        {
            public static string BuildId(Guid raceId)
            {
                return $"{Prefix}_{raceId}";
            }

            public const string Prefix = "Race_Join";
        }

        public static class Lottery
        {
            public static string BuildId(Guid lotteryId)
            {
                return $"{Prefix}_{lotteryId}";
            }

            public const string Prefix = "Lottery_Join";
        }

        public static class GuessNumber
        {
            public static string BuildId(Guid guessNumberId, int pick)
            {
                return $"{Prefix}_{guessNumberId}_{pick}";
            }

            public const string Prefix = "GuessNumber";
        }
    }

    public static class CommandsNames
    {
        // ReSharper disable InconsistentNaming
        public const string Dev_CreateMessage = "message";

        public const string Dev_DailyReset = "dailyReset";

        public const string Dev_UserEditor_Group = "userEditor";
        public const string Dev_UserEditor_Coins = "coins";
        public const string Dev_UserEditor_LootBox = "lootbox";

        public const string Party_Group = "party";
        public const string Party_CreateWithOldPrefix = "-g";
        public const string Party_Create = "-c";
        public const string Party_All = "-a";
        public const string Party_Stats = "-s";

        public const string Inventory = "inventory";
        public const string LootBox = "lootbox";
        public const string Shop = "shop";

        public const string Lohotron = "lohotron";

        public const string F1Admin_Group = "f1admin";
        public const string F1Admin_Predict = "predict";
        public const string F1Admin_Results = "results";
        public const string F1Admin_Dnf = "dnf";

        public const string F1_Group = "f1";
        public const string F1_Predict = "predict";
        public const string F1_List = "list";

        public const string Ip = "ip";

        public const string ChangeNickname = "nickname";

        public const string EmojiStats = "emoji";

        public const string Lottery = "lottery";

        public const string Race_Group = "race";
        public const string Race_Drivers = "drivers";
        public const string Race_Standings = "standings";

        public const string Roll = "roll";

        public const string Select = "select";

        public const string Roles_Group = "roles";
        public const string Roles_All = "-a";
        public const string Roles_New = "-n";
        public const string Roles_Grant = "-g";
        public const string Roles_Revoke = "-r";

        public const string ScamCoins = "scamCoins";

        public const string Rating = "rating";

        public const string Tribute = "tribute";

        public const string When = "when";
    }
}