import { Box, FormControl, MenuItem, Select, Typography } from "@mui/material";

interface Props {
  season: number;
  setSeason: (season: number) => void;
}

export default function F1PredictionsStandingsSeasonSelect({ season, setSeason }: Props) {
  const firstSeason = 2023;
  const currentSeason = new Date().getFullYear();
  const seasons = Array.from(
    { length: currentSeason - firstSeason + 1 },
    (_, i) => firstSeason + i
  );

  return (
    <Box display="flex" justifyContent="space-between" alignItems="center">
      <Typography variant="h6">Турнирная таблица чемпионата</Typography>

      <FormControl>
        <Select
          labelId="standings-season-select"
          id="standings-season-select"
          value={season}
          onChange={(event) => {
            setSeason(Number(event.target.value));
          }}
        >
          {Object.values(seasons).map((season) => (
            <MenuItem key={season} value={season}>
              {season}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </Box>
  );
}
