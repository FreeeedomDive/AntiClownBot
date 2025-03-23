import { Box, FormControl, MenuItem, Select, Typography } from "@mui/material";

interface Props {
  season: number;
  setSeason: (season: number) => void;
}

const seasons = [2023, 2024, 2025];

export default function F1PredictionsStandingsSeasonSelect({ season, setSeason }: Props) {
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
