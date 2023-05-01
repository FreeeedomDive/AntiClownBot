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
}