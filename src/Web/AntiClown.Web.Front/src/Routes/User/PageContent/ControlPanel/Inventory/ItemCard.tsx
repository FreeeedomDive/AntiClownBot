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
import {
  BaseItemDto,
  CatWifeDto,
  CommunismBannerDto,
  DogWifeDto,
  InternetDto,
  JadeRodDto,
  Rarity,
  RiceBowlDto,
} from "../../../../../Dto/Inventory/InventoryDto";

const rarityColors: Record<Rarity, string> = {
  Common: "#95a5a6",
  Rare: "#3498db",
  Epic: "#9b59b6",
  Legendary: "#f1c40f",
  BlackMarket: "#e74c3c",
};

const rarityLabels: Record<Rarity, string> = {
  Common: "Обычный",
  Rare: "Редкий",
  Epic: "Эпический",
  Legendary: "Легендарный",
  BlackMarket: "Чёрный Рынок",
};

function StatRow({
  label,
  value,
  tooltip,
}: {
  label: string;
  value: string | number;
  tooltip: string;
}) {
  return (
    <Tooltip title={tooltip} placement="top" arrow>
      <Typography
        variant="body2"
        color="text.secondary"
        sx={{
          cursor: "help",
          textDecorationLine: "underline",
          textDecorationStyle: "dashed",
          textDecorationColor: "rgba(255,255,255,0.3)",
          textUnderlineOffset: "3px",
          width: "fit-content",
        }}
      >
        {label}: {value}
      </Typography>
    </Tooltip>
  );
}

function ItemStats({ item }: { item: BaseItemDto }) {
  switch (item.itemName) {
    case "CatWife": {
      const cat = item as CatWifeDto;
      return (
        <StatRow
          label="Шанс авто-трибьюта"
          value={`${cat.autoTributeChance}%`}
          tooltip="Шанс на автоматическое подношение"
        />
      );
    }
    case "DogWife": {
      const dog = item as DogWifeDto;
      return (
        <StatRow
          label="Шанс лутбокса"
          value={`${dog.lootBoxFindChance}%`}
          tooltip="Шанс получить лутбокс во время подношения"
        />
      );
    }
    case "Internet": {
      const net = item as InternetDto;
      return (
        <Stack spacing={0.25}>
          <StatRow
            label="Скорость"
            value={`${net.speed}%`}
            tooltip="Шанс уменьшения кулдауна во время одной попытки"
          />
          <StatRow
            label="Гигабайты"
            value={net.gigabytes}
            tooltip="Попытки уменьшить кулдаун"
          />
          <StatRow
            label="Пинг"
            value={`${net.ping}%`}
            tooltip="Уменьшение кулдауна в процентах"
          />
        </Stack>
      );
    }
    case "RiceBowl": {
      const rice = item as RiceBowlDto;
      return (
        <Stack spacing={0.25}>
          <StatRow
            label="Расширение вверх"
            value={`+${rice.positiveRangeExtend}`}
            tooltip="Расширение границ полученной награды с подношения в положительную сторону"
          />
          <StatRow
            label="Расширение вниз"
            value={`-${rice.negativeRangeExtend}`}
            tooltip="Расширение границ полученной награды с подношения в отрицательную сторону"
          />
        </Stack>
      );
    }
    case "JadeRod": {
      const rod = item as JadeRodDto;
      return (
        <Stack spacing={0.25}>
          <StatRow
            label="Длина"
            value={`${rod.length} попыток`}
            tooltip="Попытки увеличить кулдаун"
          />
          <StatRow
            label="Толщина"
            value={`${rod.thickness}%`}
            tooltip="Увеличение кулдауна в процентах"
          />
        </Stack>
      );
    }
    case "CommunismBanner": {
      const banner = item as CommunismBannerDto;
      return (
        <Stack spacing={0.25}>
          <StatRow
            label="Шанс деления"
            value={`${banner.divideChance}%`}
            tooltip="Шанс разделить награду за подношение с другим владельцем плаката"
          />
          <StatRow
            label="Шанс кражи"
            value={banner.stealChance}
            tooltip="Приоритет стащить чужое подношение (если у него сработал плакат)"
          />
        </Stack>
      );
    }
    default:
      return null;
  }
}

interface ItemCardProps {
  item: BaseItemDto;
  onToggleActive: (item: BaseItemDto) => Promise<void>;
  onSell: (item: BaseItemDto) => Promise<void>;
}

export default function ItemCard({ item, onToggleActive, onSell }: ItemCardProps) {
  const [confirmSell, setConfirmSell] = useState(false);
  const [loading, setLoading] = useState(false);

  const accentColor = rarityColors[item.rarity];
  const isNegative = item.itemType === "Negative";

  const handleToggleActive = async () => {
    setLoading(true);
    try {
      await onToggleActive(item);
    } finally {
      setLoading(false);
    }
  };

  const handleSellClick = () => {
    if (!confirmSell) {
      setConfirmSell(true);
      return;
    }
    setLoading(true);
    onSell(item).finally(() => {
      setLoading(false);
      setConfirmSell(false);
    });
  };

  const handleSellCancel = () => setConfirmSell(false);

  return (
    <Paper
      elevation={2}
      sx={{
        position: "relative",
        overflow: "hidden",
        opacity: item.isActive ? 1 : 0.65,
        transition: "opacity 0.2s",
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
          {/* Header */}
          <Stack direction="row" alignItems="center" justifyContent="space-between" spacing={1}>
            <Stack direction="row" alignItems="center" spacing={1}>
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
              {item.isActive && (
                <Chip
                  label="Активен"
                  size="small"
                  color="success"
                  variant="outlined"
                  sx={{ fontSize: "0.7rem" }}
                />
              )}
              {isNegative && (
                <Chip
                  label="Негативный"
                  size="small"
                  color="error"
                  variant="outlined"
                  sx={{ fontSize: "0.7rem" }}
                />
              )}
            </Stack>
            <Typography variant="body2" color="text.secondary" sx={{ whiteSpace: "nowrap" }}>
              💰 {item.price.toLocaleString("ru")}
            </Typography>
          </Stack>

          {/* Stats */}
          <ItemStats item={item} />

          {/* Actions */}
          <Stack direction="row" spacing={1}>
            {!isNegative && (
              <Tooltip title={item.isActive ? "Деактивировать предмет" : "Активировать предмет"}>
                <Button
                  size="small"
                  variant={item.isActive ? "outlined" : "contained"}
                  color={item.isActive ? "warning" : "primary"}
                  onClick={handleToggleActive}
                  disabled={loading}
                  sx={{ fontSize: "0.75rem" }}
                >
                  {item.isActive ? "Деактивировать" : "Активировать"}
                </Button>
              </Tooltip>
            )}
            {confirmSell ? (
              <>
                <Button
                  size="small"
                  variant="contained"
                  color="error"
                  onClick={handleSellClick}
                  disabled={loading}
                  sx={{ fontSize: "0.75rem" }}
                >
                  Уверен?
                </Button>
                <Button
                  size="small"
                  variant="text"
                  onClick={handleSellCancel}
                  disabled={loading}
                  sx={{ fontSize: "0.75rem" }}
                >
                  Отмена
                </Button>
              </>
            ) : (
              <Button
                size="small"
                variant="outlined"
                color="error"
                onClick={handleSellClick}
                disabled={loading}
                sx={{ fontSize: "0.75rem" }}
              >
                Продать
              </Button>
            )}
          </Stack>
        </Stack>
      </Box>
    </Paper>
  );
}
