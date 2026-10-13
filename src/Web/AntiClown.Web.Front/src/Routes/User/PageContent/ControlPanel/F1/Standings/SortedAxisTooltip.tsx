import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { Box, Divider, Paper, Stack, Typography } from "@mui/material";

interface SortedAxisTooltipProps {
  dataIndex: number | null;
  series: {
    id: string | number;
    label: string;
    data: (number | null)[];
    color: string;
  }[];
  races: F1RaceDto[];
  mode?: "points" | "gap" | "race";
  referenceSeriesId?: string;
}

export default function SortedAxisTooltip(props: SortedAxisTooltipProps) {
  const {
    dataIndex,
    series,
    races,
    mode = "points",
    referenceSeriesId,
  } = props;
  if (dataIndex == null) return null;

  const raceIndex = mode === "race" ? dataIndex : dataIndex - 1;
  const race = raceIndex < 0 ? null : races[raceIndex];
  const raceLabel =
    raceIndex < 0
      ? "Начало сезона"
      : race
        ? race.name + (race.isSprint ? " (спринт)" : "")
        : null;

  const items = series
    .map((s) => ({
      id: s.id,
      label: s.label,
      value: s.data[dataIndex],
      color: s.color,
    }))
    .filter((item) => mode === "race" || item.value != null)
    .sort((a, b) => {
      if (a.value == null) return b.value == null ? 0 : 1;
      if (b.value == null) return -1;
      return b.value - a.value;
    });
  const participants = items.filter((item) => item.id !== referenceSeriesId);
  const reference = items.find((item) => item.id === referenceSeriesId);

  return (
    <Paper sx={{ p: 1, minWidth: 160 }}>
      {raceLabel && (
        <Typography
          variant="caption"
          sx={{ display: "block", mb: 0.5, opacity: 0.6 }}
        >
          {raceLabel}
        </Typography>
      )}
      <Stack spacing={0.25}>
        {participants.map((item) => (
          <Box
            key={item.id}
            sx={{ display: "flex", alignItems: "center", gap: 1 }}
          >
            <Box
              sx={{
                width: 8,
                height: 8,
                borderRadius: "50%",
                backgroundColor: item.color,
                flexShrink: 0,
              }}
            />
            <Typography variant="caption" sx={{ flex: 1 }}>
              {item.label}
            </Typography>
            <Typography
              variant="caption"
              fontWeight={item.value == null ? 400 : "bold"}
              color={item.value == null ? "text.secondary" : "inherit"}
            >
              {item.value == null
                ? "Нет предсказания"
                : mode === "gap" && item.value === 0
                  ? "0 · лидер"
                  : item.value}
            </Typography>
          </Box>
        ))}
        {reference && (
          <>
            <Divider sx={{ my: 0.5 }} />
            <Stack direction="row" alignItems="center" spacing={1}>
              <Box
                sx={{
                  width: 16,
                  borderTop: `2px dashed ${reference.color}`,
                  flexShrink: 0,
                }}
              />
              <Typography variant="caption" sx={{ flex: 1 }}>
                {reference.label}
              </Typography>
              <Typography variant="caption" fontWeight="bold">
                {reference.value}
              </Typography>
            </Stack>
          </>
        )}
      </Stack>
    </Paper>
  );
}
