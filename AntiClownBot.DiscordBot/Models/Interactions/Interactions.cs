namespace AntiClownDiscordBotVersion2.Models.Interactions;

public static class Interactions
{
    public static class Buttons
    {
        public const string StartRaceResultInputButton = "StartRaceResultInputButton";

        public const string ShopButtonsPrefix = "Shop_";
        public const string ShopButtonItem1 = ShopButtonsPrefix + "1";
        public const string ShopButtonItem2 = ShopButtonsPrefix + "2";
        public const string ShopButtonItem3 = ShopButtonsPrefix + "3";
        public const string ShopButtonItem4 = ShopButtonsPrefix + "4";
        public const string ShopButtonItem5 = ShopButtonsPrefix + "5";
        public const string ShopButtonReroll = ShopButtonsPrefix + "Reroll";
        public const string ShopButtonChangeTool = ShopButtonsPrefix + "ChangeTool";

        public const string InventoryButtonsPrefix = "Inventory_";
        public const string InventoryButton1 = InventoryButtonsPrefix + "1";
        public const string InventoryButton2 = InventoryButtonsPrefix + "2";
        public const string InventoryButton3 = InventoryButtonsPrefix + "3";
        public const string InventoryButton4 = InventoryButtonsPrefix + "4";
        public const string InventoryButton5 = InventoryButtonsPrefix + "5";
        public const string InventoryButtonLeft = InventoryButtonsPrefix + "Left";
        public const string InventoryButtonRight = InventoryButtonsPrefix + "Right";
        public const string InventoryButtonChangeActiveStatus = InventoryButtonsPrefix + "Active";
        public const string InventoryButtonSell = InventoryButtonsPrefix + "Sell";
    }

    public static class Dropdowns
    {
        public const string DriversSelectDropdown = "DriversSelectDropdown";
        public const string DriversSelectDropdownItemPrefix = "DriversSelectDropdownItem_";
    }

    public static class Commands
    {
        // ReSharper disable InconsistentNaming
        public const string Dev_CreateMessage = "message";

        public const string Dev_DailyReset = "dailyReset";

        public const string Dev_UserEditor_Group = "userEditor";
        public const string Dev_UserEditor_Coins = "coins";
        public const string Dev_UserEditor_Lootbox = "lootbox";

        public const string Party_Group = "party";
        public const string Party_CreateWithOldPrefix = "-g";
        public const string Party_Create = "-c";
        public const string Party_All = "-a";
        public const string Party_Stats = "-s";

        public const string Inventory = "inventory";
        public const string Lootbox = "lootbox";
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