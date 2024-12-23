﻿namespace AntiClown.Api.Core.Economies.Domain;

public class Tribute
{
    public Guid UserId { get; set; }
    public DateTime TributeDateTime { get; set; }
    public int ScamCoins { get; set; }
    public int CooldownInMilliseconds { get; set; }
    public bool IsAutomatic { get; set; }
    public bool IsNextAutomatic { get; set; }
    public Dictionary<Guid, int> CooldownModifiers { get; set; }
    public bool HasGiftedLootBox { get; set; }
    public Guid? SharedUserId { get; set; }
}