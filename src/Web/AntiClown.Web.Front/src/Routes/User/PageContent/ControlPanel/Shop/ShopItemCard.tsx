import React, { useState } from "react";
import {
  Box,
  Button,
  Chip,
  Paper,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import { Rarity, ItemName, BaseItemDto } from "../../../../../Dto/Inventory/InventoryDto";
import { ShopItemDto } from "../../../../../Dto/Shop/ShopDto";
import ItemStats from "../Inventory/ItemStats";

const rarityColors: Record<Rarity, string> = {
  Common: "#95a5a6",
  Rare: "#3498db",
  Epic: "#9b59b6",
  Legendary: "#f1c40f",
  BlackMarket: "#e74c3c",
};

const rarityLabels: Record<Rarity, string> = {
  Common: "Обычная",
  Rare: "Редкая",
  Epic: "Эпическая",
  Legendary: "Легендарная",
  BlackMarket: "С чёрного рынка",
};

const itemNameLabels: Record<ItemName, string> = {
  CatWife: "Кошка-жена",
  DogWife: "Собака-жена",
  Internet: "Интернет",
  RiceBowl: "Рис Миска",
  JadeRod: "Нефритовый Стержень",
  CommunismBanner: "Коммунистический Плакат",
};

interface ShopItemCardProps {
  item: ShopItemDto;
  index: number;
  freeReveals: number;
  purchasedItem?: BaseItemDto;
  onReveal: (item: ShopItemDto) => Promise<void>;
  onBuy: (item: ShopItemDto) => Promise<void>;
}

export default function ShopItemCard({
  item,
  index,
  freeReveals,
  purchasedItem,
  onReveal,
  onBuy,
}: ShopItemCardProps) {
  const [loading, setLoading] = useState(false);
  const [confirmBuy, setConfirmBuy] = useState(false);

  const accentColor = rarityColors[item.rarity];
  const isNameVisible = item.isRevealed || item.isOwned;
  const revealCost = Math.floor(item.price * 0.4);

  const handleReveal = async () => {
    setLoading(true);
    try {
      await onReveal(item);
    } finally {
      setLoading(false);
    }
  };

  const handleBuyClick = () => {
    if (!confirmBuy) {
      setConfirmBuy(true);
      return;
    }
    setLoading(true);
    onBuy(item).finally(() => {
      setLoading(false);
      setConfirmBuy(false);
    });
  };

  const handleBuyCancel = () => setConfirmBuy(false);

  return (
    <Paper
      elevation={2}
      sx={{
        position: "relative",
        overflow: "hidden",
        opacity: item.isOwned ? 0.6 : 1,
        transition: "opacity 0.2s",
        backgroundColor: `${accentColor}22`,
        "&::before": {
          content: '""',
          position: "absolute",
          left: 0,
          top: 0,
          bottom: 0,
          width: "4px",
          backgroundColor: accentColor,
        },
      }}
    >
      <Box sx={{ pl: 2, pr: 2, pt: 1.5, pb: 1.5 }}>
        <Stack spacing={1}>
          <Stack direction="row" alignItems="center" justifyContent="space-between" spacing={1}>
            <Stack direction="row" alignItems="center" spacing={1}>
              <Typography variant="body2" color="text.secondary" sx={{ fontWeight: 600 }}>
                #{index}
              </Typography>
              <Chip
                label={rarityLabels[item.rarity]}
                size="small"
                sx={{
                  backgroundColor: accentColor,
                  color: item.rarity === "Legendary" ? "#000" : "#fff",
                  fontWeight: 600,
                  fontSize: "0.7rem",
                }}
              />
              {item.isOwned && (
                <Chip
                  label="Куплен"
                  size="small"
                  color="success"
                  sx={{ fontSize: "0.7rem", fontWeight: 600 }}
                />
              )}
            </Stack>
            <Typography variant="body2" color="text.secondary" sx={{ whiteSpace: "nowrap" }}>
              💰 {item.price.toLocaleString("ru")}
            </Typography>
          </Stack>

          <Typography variant="body1" sx={{ fontWeight: 500 }}>
            {isNameVisible ? itemNameLabels[item.name] : "Нераспознанный предмет"}
          </Typography>

          {purchasedItem && <ItemStats item={purchasedItem} />}

          {!item.isOwned && (
            <Stack direction="row" spacing={1}>
              {confirmBuy ? (
                <>
                  <Button
                    size="small"
                    variant="contained"
                    color="success"
                    onClick={handleBuyClick}
                    disabled={loading}
                    sx={{ fontSize: "0.75rem" }}
                  >
                    Уверен?
                  </Button>
                  <Button
                    size="small"
                    variant="text"
                    onClick={handleBuyCancel}
                    disabled={loading}
                    sx={{ fontSize: "0.75rem" }}
                  >
                    Отмена
                  </Button>
                </>
              ) : (
                <Button
                  size="small"
                  variant="contained"
                  color="success"
                  onClick={handleBuyClick}
                  disabled={loading}
                  sx={{ fontSize: "0.75rem" }}
                >
                  Купить
                </Button>
              )}
              {!item.isRevealed && !confirmBuy && (
                <Tooltip
                  title={
                    freeReveals > 0
                      ? `Бесплатное распознавание (осталось: ${freeReveals})`
                      : `Стоимость распознавания: 💰 ${revealCost.toLocaleString("ru")} (40% от цены)`
                  }
                >
                  <Button
                    size="small"
                    variant="outlined"
                    color="info"
                    onClick={handleReveal}
                    disabled={loading}
                    sx={{ fontSize: "0.75rem" }}
                  >
                    {freeReveals > 0 ? "Распознать (бесплатно)" : "Распознать"}
                  </Button>
                </Tooltip>
              )}
            </Stack>
          )}
        </Stack>
      </Box>
    </Paper>
  );
}
