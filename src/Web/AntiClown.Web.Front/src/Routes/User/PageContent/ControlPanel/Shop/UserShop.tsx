import React, { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Collapse,
  Divider,
  IconButton,
  Snackbar,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import { CurrentShopInfoDto, ShopItemDto, ShopStatsDto } from "../../../../../Dto/Shop/ShopDto";
import { BaseItemDto } from "../../../../../Dto/Inventory/InventoryDto";
import { EconomyDto } from "../../../../../Dto/Economy/EconomyDto";
import ShopApi from "../../../../../Api/ShopApi";
import EconomyApi from "../../../../../Api/EconomyApi";
import ShopItemCard from "./ShopItemCard";

export default function UserShop() {
  const { userId } = useParams<"userId">();
  const [loading, setLoading] = useState(true);
  const [shop, setShop] = useState<CurrentShopInfoDto | null>(null);
  const [economy, setEconomy] = useState<EconomyDto | null>(null);
  const [stats, setStats] = useState<ShopStatsDto | null>(null);
  const [purchasedItems, setPurchasedItems] = useState<Record<string, BaseItemDto>>({});
  const [statsOpen, setStatsOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [rerolling, setRerolling] = useState(false);

  const loadShop = useCallback(async () => {
    if (!userId) return;
    const data = await ShopApi.get(userId);
    setShop(data);
  }, [userId]);

  const loadStats = useCallback(async () => {
    if (!userId) return;
    const data = await ShopApi.getStats(userId);
    setStats(data);
  }, [userId]);

  useEffect(() => {
    Promise.all([
      loadShop(),
      userId ? EconomyApi.get(userId).then(setEconomy) : Promise.resolve(),
    ])
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [loadShop, userId]);

  const refreshEconomy = useCallback(async () => {
    if (!userId) return;
    const data = await EconomyApi.get(userId);
    setEconomy(data);
  }, [userId]);

  const handleReveal = async (item: ShopItemDto) => {
    if (!userId) return;
    try {
      const updated = await ShopApi.reveal(userId, item.id);
      setShop((prev) =>
        prev
          ? {
              ...prev,
              freeReveals: prev.freeReveals > 0 ? prev.freeReveals - 1 : 0,
              items: prev.items.map((i) => (i.id === updated.id ? updated : i)),
            }
          : prev
      );
      await refreshEconomy();
    } catch {
      setError("Не удалось распознать предмет");
    }
  };

  const handleBuy = async (item: ShopItemDto) => {
    if (!userId) return;
    try {
      const boughtItem = await ShopApi.buy(userId, item.id);
      setShop((prev) =>
        prev
          ? {
              ...prev,
              items: prev.items.map((i) =>
                i.id === item.id ? { ...i, isOwned: true, isRevealed: true } : i
              ),
            }
          : prev
      );
      setPurchasedItems((prev) => ({ ...prev, [item.id]: boughtItem }));
      await refreshEconomy();
    } catch {
      setError("Не удалось купить предмет");
    }
  };

  const handleReroll = async () => {
    if (!userId) return;
    setRerolling(true);
    try {
      await ShopApi.reroll(userId);
      await Promise.all([loadShop(), refreshEconomy()]);
    } catch {
      setError("Не удалось обновить магазин");
    } finally {
      setRerolling(false);
    }
  };

  const handleStatsToggle = async () => {
    if (!statsOpen && !stats) {
      await loadStats().catch(console.error);
    }
    setStatsOpen((v) => !v);
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (!shop) {
    return <Typography>Не удалось загрузить магазин</Typography>;
  }

  return (
    <Box>
      <Typography variant="h5" sx={{ fontWeight: 600, mb: 3 }}>
        Магазин
      </Typography>

      <Stack direction="row" spacing={3} sx={{ mb: 3 }} flexWrap="wrap">
        {economy && (
          <Typography variant="body2" color="text.secondary">
            💰 Баланс: <strong>{economy.scamCoins.toLocaleString("ru")}</strong>
          </Typography>
        )}
        <Tooltip title="Цена следующего обновления магазина">
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ cursor: "help" }}
          >
            🔄 Реролл: <strong>{shop.reRollPrice.toLocaleString("ru")}</strong>
          </Typography>
        </Tooltip>
        <Tooltip title="Количество бесплатных распознаваний, оставшихся сегодня">
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ cursor: "help" }}
          >
            🔍 Бесплатные распознавания: <strong>{shop.freeReveals}</strong>
          </Typography>
        </Tooltip>
      </Stack>

      <Stack spacing={2} sx={{ mb: 3, maxWidth: 600 }}>
        {shop.items.map((item, idx) => (
          <ShopItemCard
            key={item.id}
            item={item}
            index={idx + 1}
            freeReveals={shop.freeReveals}
            purchasedItem={purchasedItems[item.id]}
            onReveal={handleReveal}
            onBuy={handleBuy}
          />
        ))}
      </Stack>

      <Button
        variant="outlined"
        color="error"
        onClick={handleReroll}
        disabled={rerolling}
        sx={{ mb: 3 }}
      >
        {rerolling
          ? "Обновление..."
          : shop.reRollPrice === 0
            ? "Обновить магазин (бесплатно)"
            : `Обновить магазин (-${shop.reRollPrice.toLocaleString("ru")} 💰)`}
      </Button>

      <Divider sx={{ mb: 1 }} />
      <Stack
        direction="row"
        alignItems="center"
        spacing={1}
        sx={{ cursor: "pointer", mb: 1 }}
        onClick={handleStatsToggle}
      >
        <Typography variant="body2" color="text.secondary">
          Статистика
        </Typography>
        <IconButton size="small">
          {statsOpen ? (
            <ExpandLessIcon fontSize="small" />
          ) : (
            <ExpandMoreIcon fontSize="small" />
          )}
        </IconButton>
      </Stack>
      <Collapse in={statsOpen}>
        {stats ? (
          <Stack spacing={0.5} sx={{ mb: 2 }}>
            <Typography variant="body2" color="text.secondary">
              Куплено предметов: <strong>{stats.itemsBought}</strong>
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Потрачено на покупки:{" "}
              <strong>
                💰 {stats.scamCoinsLostOnPurchases.toLocaleString("ru")}
              </strong>
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Реролов: <strong>{stats.totalReRolls}</strong>
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Потрачено на реролы:{" "}
              <strong>
                💰 {stats.scamCoinsLostOnReRolls.toLocaleString("ru")}
              </strong>
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Распознаваний: <strong>{stats.totalReveals}</strong>
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Потрачено на распознавания:{" "}
              <strong>
                💰 {stats.scamCoinsLostOnReveals.toLocaleString("ru")}
              </strong>
            </Typography>
          </Stack>
        ) : (
          <CircularProgress size={20} sx={{ mb: 2 }} />
        )}
      </Collapse>

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
