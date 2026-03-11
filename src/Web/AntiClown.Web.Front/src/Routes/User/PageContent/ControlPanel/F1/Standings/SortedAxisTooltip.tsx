// eslint-disable-next-line @typescript-eslint/no-explicit-any
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { Box, Paper, Stack, Typography } from "@mui/material";

export default function SortedAxisTooltip(props: any) {
  const { dataIndex, series, races } = props as {
    dataIndex: number | null;
    series: { label: string; data: (number | null)[]; color: string }[];
    races: F1RaceDto[];
  };
  if (dataIndex == null) return null;

  const race = dataIndex === 0 ? null : races[dataIndex - 1];
  const raceLabel =
    dataIndex === 0
      ? "Начало сезона"
      : race
        ? race.name + (race.isSprint ? " (спринт)" : "")
        : null;

  const items = series
    .map((s) => ({
      label: s.label,
      value: s.data[dataIndex],
      color: s.color
    }))
    .filter((item) => item.value != null)
    .sort((a, b) => b.value! - a.value!);

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
        {items.map((item) => (
          <Box
            key={item.label}
            sx={{ display: "flex", alignItems: "center", gap: 1 }}
          >
            <Box
              sx={{
                width: 8,
                height: 8,
                borderRadius: "50%",
                backgroundColor: item.color,
                flexShrink: 0
              }}
            />
            <Typography variant="caption" sx={{ flex: 1 }}>
              {item.label}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              {item.value}
            </Typography>
          </Box>
        ))}
      </Stack>
    </Paper>
  );
}