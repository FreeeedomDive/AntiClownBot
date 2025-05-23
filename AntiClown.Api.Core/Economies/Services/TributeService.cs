using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Domain.Items.Base.Extensions;
using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Dto.Economies;
using AntiClown.Api.Dto.Exceptions.Tribute;
using AntiClown.Core.Schedules;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Tools.Utility.Extensions;
using AntiClown.Tools.Utility.Random;
using Hangfire;

namespace AntiClown.Api.Core.Economies.Services;

public class TributeService(
    IEconomyService economyService,
    IItemsService itemsService,
    ITributeMessageProducer tributeMessageProducer,
    IAntiClownDataApiClient antiClownDataApiClient,
    IScheduler scheduler
)
    : ITributeService
{
    public async Task<DateTime> WhenNextTributeAsync(Guid userId)
    {
        var economy = await economyService.ReadEconomyAsync(userId);
        return economy.NextTribute;
    }

    public async Task<Tribute> MakeTributeAsync(Guid userId)
    {
        return await InnerMakeTributeAsync(userId, false);
    }

    // keep this method public for Hangfire scheduler
    public async Task<Tribute> InnerMakeTributeAsync(Guid userId, bool isAutomatic)
    {
        var userEconomy = await economyService.ReadEconomyAsync(userId);
        var isTributeReady = userEconomy.IsTributeReady();
        switch (isTributeReady)
        {
            case false when isAutomatic:
                throw new AutoTributeWasCancelledByEarlierTributeException(userId);
            case false when !isAutomatic:
                throw new TributeIsOnCooldownException(userId);
        }

        var userItems = await itemsService.ReadAllActiveItemsForUserAsync(userId);
        var (cooldown, modifiers) = await CalculateCooldownAsync(userItems);

        var minTribute = TributeHelpers.MinTributeValue - userItems.RiceBowls().Sum(x => x.NegativeRangeExtend);
        var maxTribute = TributeHelpers.MaxTributeValue + userItems.RiceBowls().Sum(x => x.PositiveRangeExtend);
        var tributeQuality = Randomizer.GetRandomNumberInclusive(minTribute, maxTribute);

        var lootboxChance = userItems.DogWives().Sum(x => x.LootBoxFindChance);
        var lootboxGranted = Randomizer.GetRandomNumberBetween(0, 1000) < lootboxChance;

        var communismChance = userItems.CommunismBanners().Sum(x => x.DivideChance);
        var isCommunism = Randomizer.GetRandomNumberBetween(0, 100) < communismChance;
        var sharedUser = isCommunism ? await GetRandomCommunistAsync(userId) : null;
        tributeQuality = isCommunism && sharedUser is not null ? tributeQuality / 2 : tributeQuality;

        var automaticNextTributeChance = userItems.CatWives().Sum(x => x.AutoTributeChance);
        var isNextTributeAutomatic = Randomizer.GetRandomNumberBetween(0, 100) < automaticNextTributeChance;

        var tribute = new Tribute
        {
            UserId = userId,
            TributeDateTime = DateTime.UtcNow,
            IsAutomatic = isAutomatic,
            CooldownInMilliseconds = cooldown,
            CooldownModifiers = modifiers,
            ScamCoins = tributeQuality,
            IsNextAutomatic = isNextTributeAutomatic,
            SharedUserId = sharedUser,
            HasGiftedLootBox = lootboxGranted,
        };

        await MakeTributeAsync(tribute);
        ScheduleNextTribute(tribute);

        return tribute;
    }

    // keep this method public for Hangfire scheduler
    public async Task<(int Cooldown, Dictionary<Guid, int> CooldownModifiers)> CalculateCooldownAsync(BaseItem[] items)
    {
        var modifiers = new Dictionary<Guid, int>();

        var defaultCooldown = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.Economy, "DefaultTributeCooldown");
        var cooldown = items.Internets()
                            .SelectMany(item => Enumerable.Repeat(item, item.Gigabytes))
                            .Aggregate(
                                defaultCooldown, (currentCooldown, item) =>
                                {
                                    if (Randomizer.GetRandomNumberBetween(0, 100) >= item.Ping)
                                    {
                                        return currentCooldown;
                                    }

                                    if (modifiers.ContainsKey(item.Id))
                                    {
                                        modifiers[item.Id]++;
                                    }
                                    else
                                    {
                                        modifiers.Add(item.Id, 1);
                                    }

                                    return (int)(currentCooldown * (100d - item.Speed) / 100);
                                }
                            );

        cooldown = items.JadeRods()
                        .SelectMany(item => Enumerable.Repeat(item, item.Length))
                        .Aggregate(
                            cooldown, (currentCooldown, item) =>
                            {
                                if (Randomizer.GetRandomNumberBetween(0, 100) >= TributeHelpers.CooldownIncreaseChanceByOneJade)
                                {
                                    return currentCooldown;
                                }

                                if (modifiers.ContainsKey(item.Id))
                                {
                                    modifiers[item.Id]++;
                                }
                                else
                                {
                                    modifiers.Add(item.Id, 1);
                                }

                                return (int)(currentCooldown * (100d + item.Thickness) / 100);
                            }
                        );

        return (cooldown, modifiers);
    }

    // keep this method public for Hangfire scheduler
    public async Task<Guid?> GetRandomCommunistAsync(Guid userId)
    {
        var communists = (await ReadDistributedCommunistsAsync())
                         .Where(x => x != userId)
                         .ToArray();
        return communists.Length == 0 ? null : communists.SelectRandomItem();
    }

    // keep this method public for Hangfire scheduler
    public async Task<Guid[]> ReadDistributedCommunistsAsync()
    {
        var allCommunismBanners = await itemsService.ReadItemsWithNameAsync(ItemName.CommunismBanner);
        return allCommunismBanners
               .CommunismBanners()
               .SelectMany(x => Enumerable.Repeat(x.OwnerId, x.StealChance))
               .ToArray();
    }

    // keep this method public for Hangfire scheduler
    public async Task MakeTributeAsync(Tribute tribute)
    {
        await economyService.UpdateScamCoinsAsync(tribute.UserId, tribute.ScamCoins, "Подношение");
        await economyService.UpdateNextTributeCoolDownAsync(
            tribute.UserId,
            tribute.TributeDateTime.AddMilliseconds(tribute.CooldownInMilliseconds)
        );

        if (tribute.HasGiftedLootBox)
        {
            await economyService.UpdateLootBoxesAsync(tribute.UserId, 1);
        }

        if (tribute.SharedUserId != null)
        {
            await economyService.UpdateScamCoinsAsync(
                tribute.SharedUserId.Value, tribute.ScamCoins,
                "Полученное коммунистическое подношение"
            );
        }
    }

    // keep this method public for Hangfire scheduler
    public void ScheduleNextTribute(Tribute tribute)
    {
        if (!tribute.IsNextAutomatic)
        {
            return;
        }

        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => ExecuteAutoTributeAsync(tribute),
                TimeSpan.FromMilliseconds(tribute.CooldownInMilliseconds + 1000)
            )
        );
    }

    // keep this method public for Hangfire scheduler
    public async Task ExecuteAutoTributeAsync(Tribute tribute)
    {
        try
        {
            var newTribute = await InnerMakeTributeAsync(tribute.UserId, true);
            await tributeMessageProducer.ProduceAsync(newTribute);
        }
        catch (AutoTributeWasCancelledByEarlierTributeException)
        {
        }
        catch (TributeIsOnCooldownException)
        {
        }
    }
}