﻿namespace AntiClown.Api.Dto.Shops;

public class ShopStatsDto
{
    public Guid Id { get; set; }
    public int TotalReRolls { get; set; }
    public int ItemsBought { get; set; }
    public int TotalReveals { get; set; }
    public int ScamCoinsLostOnReveals { get; set; }
    public int ScamCoinsLostOnReRolls { get; set; }
    public int ScamCoinsLostOnPurchases { get; set; }
}