﻿namespace AntiClown.Api.Core.Economies.Domain;

public class Economy
{
    // equal to UserId
    public Guid Id { get; set; }
    public int ScamCoins { get; set; }
    public DateTime NextTribute { get; set; }
    public int LootBoxes { get; set; }

    public static readonly Economy Default = new()
    {
        ScamCoins = 1500,
        NextTribute = DateTime.UtcNow,
        LootBoxes = 0,
    };
}