import F1PredictionsSelectCard from "./F1PredictionsSelectCard";
import { FormControl, InputAdornment, OutlinedInput } from "@mui/material";
import React from "react";

interface Props {
  driver: string;
  selectedPosition: number | null;
  setSelectedPosition: (position: number) => void;
}

export default function F1PredictionsDriverPositionSelect({
  driver,
  selectedPosition,
  setSelectedPosition,
}: Props) {
  return (
    <F1PredictionsSelectCard title={`Позиция гонщика ${driver}`}>
      <FormControl fullWidth>
        <OutlinedInput
          id="outlined-adornment-weight"
          endAdornment={
            <InputAdornment position="end">1-22</InputAdornment>
          }
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
