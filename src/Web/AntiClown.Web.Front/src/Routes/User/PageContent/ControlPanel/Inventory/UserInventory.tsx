import React, { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Alert,
  Box,
  CircularProgress,
  Snackbar,
  Tab,
  Tabs,
  Typography,
} from "@mui/material";
import {
  BaseItemDto,
  InventoryDto,
  ItemName,
  Rarity,
} from "../../../../../Dto/Inventory/InventoryDto";
import InventoryApi from "../../../../../Api/InventoryApi";
import ItemCard from "./ItemCard";

const itemNameLabels: Record<ItemName, string> = {
  CatWife: "Кошка-жена",
  DogWife: "Собака-жена",
  Internet: "Интернет",
  RiceBowl: "Рис Миска",
  JadeRod: "Нефритовый Стержень",
  CommunismBanner: "Коммунистический Плакат",
};

const tabOrder: ItemName[] = [
  "CatWife",
  "DogWife",
  "Internet",
  "RiceBowl",
  "JadeRod",
  "CommunismBanner",
];

const rarityOrder: Record<Rarity, number> = {
  BlackMarket: 0,
  Legendary: 1,
  Epic: 2,
  Rare: 3,
  Common: 4,
};

function getItemsForTab(inventory: InventoryDto, tab: ItemName): BaseItemDto[] {
  const map: Record<ItemName, BaseItemDto[]> = {
    CatWife: inventory.catWives,
    DogWife: inventory.dogWives,
    Internet: inventory.internets,
    RiceBowl: inventory.riceBowls,
    JadeRod: inventory.jadeRods,
    CommunismBanner: inventory.communismBanners,
  };
  return map[tab] ?? [];
}

function sortItems(items: BaseItemDto[]): BaseItemDto[] {
  return [...items].sort((a, b) => {
    if (a.isActive !== b.isActive) return a.isActive ? -1 : 1;
    return rarityOrder[a.rarity] - rarityOrder[b.rarity];
  });
}

export default function UserInventory() {
  const { userId } = useParams<"userId">();
  const [loading, setLoading] = useState(true);
  const [inventory, setInventory] = useState<InventoryDto | null>(null);
  const [activeTab, setActiveTab] = useState<ItemName>("CatWife");
  const [error, setError] = useState<string | null>(null);

  const loadInventory = useCallback(async () => {
    if (!userId) return;
    const data = await InventoryApi.get(userId);
    setInventory(data);
  }, [userId]);

  useEffect(() => {
    loadInventory()
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [loadInventory]);

  const handleToggleActive = async (item: BaseItemDto) => {
    if (!userId) return;
    try {
      await InventoryApi.changeActiveStatus(userId, item.id, !item.isActive);
      await loadInventory();
    } catch {
      setError("Не удалось изменить статус предмета");
    }
  };

  const handleSell = async (item: BaseItemDto) => {
    if (!userId) return;
    try {
      await InventoryApi.sell(userId, item.id);
      await loadInventory();
    } catch {
      setError("Не удалось продать предмет");
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (!inventory) {
    return <Typography>Не удалось загрузить инвентарь</Typography>;
  }

  const allItems = getItemsForTab(inventory, activeTab);
  const sortedItems = sortItems(allItems);

  return (
    <Box>
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          mb: 2,
          flexWrap: "wrap",
          gap: 1,
        }}
      >
        <Typography variant="h5">Инвентарь</Typography>
        <Typography variant="body2" color="text.secondary">
          Стоимость: 💰 {inventory.netWorth.toLocaleString("ru")}
        </Typography>
      </Box>

      <Tabs
        value={activeTab}
        onChange={(_, val) => setActiveTab(val)}
        variant="scrollable"
        scrollButtons="auto"
        sx={{ mb: 3, borderBottom: 1, borderColor: "divider" }}
      >
        {tabOrder.map((name) => {
          const count = getItemsForTab(inventory, name).length;
          return (
            <Tab
              key={name}
              value={name}
              label={`${itemNameLabels[name]}${count > 0 ? ` (${count})` : ""}`}
              sx={{ textTransform: "none", fontSize: "0.85rem" }}
            />
          );
        })}
      </Tabs>

      {sortedItems.length === 0 ? (
        <Typography color="text.secondary">Предметов нет</Typography>
      ) : (
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(400px, 1fr))",
            gap: 2,
          }}
        >
          {sortedItems.map((item) => (
            <ItemCard
              key={item.id}
              item={item}
              onToggleActive={handleToggleActive}
              onSell={handleSell}
            />
          ))}
        </Box>
      )}

      <Snackbar
        open={!!error}
        autoHideDuration={4000}
        onClose={() => setError(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert severity="error" onClose={() => setError(null)}>
          {error}
        </Alert>
      </Snackbar>
    </Box>
  );
}
