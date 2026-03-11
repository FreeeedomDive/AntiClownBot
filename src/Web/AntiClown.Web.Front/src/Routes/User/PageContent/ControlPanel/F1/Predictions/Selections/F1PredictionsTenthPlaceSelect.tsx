import { Box, FormControl, MenuItem, Select } from "@mui/material";
import { getDriversFromTeams } from "../../../../../../../Dto/F1Predictions/F1DriversHelpers";
import React from "react";
import { F1TeamDto } from "../../../../../../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsSelectCard from "./F1PredictionsSelectCard";
import F1TeamBadge from "../F1TeamBadge";

interface Props {
  selected10Position: string;
  setSelected10Position: (selected10Position: string) => void;
  teams: F1TeamDto[];
}

export default function F1PredictionsTenthPlaceSelect({
  selected10Position,
  setSelected10Position,
  teams,
}: Props) {
  return (
    <F1PredictionsSelectCard title="10 место">
      <FormControl fullWidth size="small">
        <Select
          labelId="10-position"
          id="10-position-select"
          value={selected10Position}
          onChange={(event) => {
            const value = event.target.value;
            setSelected10Position(value);
          }}
        >
          {getDriversFromTeams(teams).map((driver) => (
            <MenuItem key={driver} value={driver}>
              <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                <F1TeamBadge driver={driver} teams={teams} />
                {driver}
              </Box>
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </F1PredictionsSelectCard>
  );
}
