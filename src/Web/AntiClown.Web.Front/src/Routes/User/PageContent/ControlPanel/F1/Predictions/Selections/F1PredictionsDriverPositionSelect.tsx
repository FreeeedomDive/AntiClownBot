import F1PredictionsSelectCard from "./F1PredictionsSelectCard";
import { Box, FormControl, InputAdornment, OutlinedInput } from "@mui/material";
import React from "react";
import { F1TeamDto } from "../../../../../../../Dto/F1Predictions/F1TeamDto";
import F1TeamBadge from "../F1TeamBadge";

interface Props {
  driver: string;
  teams: F1TeamDto[];
  selectedPosition: number | null;
  setSelectedPosition: (position: number) => void;
}

export default function F1PredictionsDriverPositionSelect({
  driver,
  teams,
  selectedPosition,
  setSelectedPosition,
}: Props) {
  return (
    <F1PredictionsSelectCard
      title={
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <span>Позиция гонщика {driver}</span>
          <F1TeamBadge driver={driver} teams={teams} />
        </Box>
      }
    >
      <FormControl fullWidth>
        <OutlinedInput
          id="outlined-adornment-weight"
          endAdornment={<InputAdornment position="end">1-22</InputAdornment>}
          aria-describedby="outlined-weight-helper-text"
          inputProps={{
            "aria-label": "weight",
          }}
          value={selectedPosition}
          onChange={(event) => {
            const fixedValue = event.target.value.replace(/[^\d,.]/g, "");

            const numericValue = Number(fixedValue);

            if (numericValue >= 0) {
              setSelectedPosition(numericValue);
            }
          }}
        />
      </FormControl>
    </F1PredictionsSelectCard>
  );
}
