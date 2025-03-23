import {
  FormControl,
  InputAdornment,
  OutlinedInput,
} from "@mui/material";
import React from "react";
import F1PredictionsSelectCard from "./F1PredictionsSelectCard";

interface Props {
  firstPlaceLead: string;
  setFirstPlaceLead: (firstPlaceLead: string) => void;
}

export default function F1PredictionsFirstPlaceLeadSelect({
  firstPlaceLead,
  setFirstPlaceLead,
}: Props) {
  return (
    <F1PredictionsSelectCard title="Отрыв 1 места">
      <FormControl fullWidth>
        <OutlinedInput
          id="outlined-adornment-weight"
          endAdornment={
            <InputAdornment position="end">в секундах</InputAdornment>
          }
          placeholder="5.169"
          aria-describedby="outlined-weight-helper-text"
          inputProps={{
            "aria-label": "weight",
          }}
          value={firstPlaceLead}
          onChange={(event) => {
            const fixedValue = event.target.value
              .replace(/[^\d,.]/g, "")
              .replace(",", ".");

            const numericValue = Number(fixedValue);

            if (numericValue >= 0) {
              setFirstPlaceLead(fixedValue);
            }
          }}
        />
      </FormControl>
    </F1PredictionsSelectCard>
  );
}
