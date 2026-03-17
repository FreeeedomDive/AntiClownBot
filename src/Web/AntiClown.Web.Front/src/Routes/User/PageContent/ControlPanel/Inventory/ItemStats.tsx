import React from "react";
import { Stack, Tooltip, Typography } from "@mui/material";
import {
  BaseItemDto,
  CatWifeDto,
  CommunismBannerDto,
  DogWifeDto,
  InternetDto,
  JadeRodDto,
  RiceBowlDto,
} from "../../../../../Dto/Inventory/InventoryDto";

export function StatRow({
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

export default function ItemStats({ item }: { item: BaseItemDto }) {
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
