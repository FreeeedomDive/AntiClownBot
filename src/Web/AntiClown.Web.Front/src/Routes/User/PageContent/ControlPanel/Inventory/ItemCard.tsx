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
  Rarity,
} from "../../../../../Dto/Inventory/InventoryDto";
import ItemStats from "./ItemStats";

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
