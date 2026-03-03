import {
  FormControl,
  FormControlLabel,
  Radio,
  RadioGroup,
} from "@mui/material";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionDto,
} from "../../../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import React from "react";
import F1PredictionsSelectCard from "./F1PredictionsSelectCard";

interface Props {
  selectedSafetyCarPrediction: F1SafetyCarPredictionDto;
  setSelectedSafetyCarPrediction: (
    selectedSafetyCarPrediction: F1SafetyCarPredictionDto,
  ) => void;
}

const F1SafetyCarPredictionTexts: Record<F1SafetyCarPredictionDto, string> = {
  Zero: "0",
  One: "1",
  Two: "2",
  ThreePlus: "3+",
};

export default function F1PredictionsIncidentsSelect({
  selectedSafetyCarPrediction,
  setSelectedSafetyCarPrediction,
}: Props) {
  return (
    <F1PredictionsSelectCard title="Инциденты (VSC, SC, RedFlag)">
      <FormControl fullWidth>
        <RadioGroup
          aria-labelledby="demo-radio-buttons-group-label"
          name="radio-buttons-group"
        >
          {F1SafetyCarsPredictionDto.map((F1SafetyCarPredictionDto) => (
            <FormControlLabel
              key={F1SafetyCarPredictionDto}
              value={F1SafetyCarPredictionDto}
              control={<Radio />}
              label={F1SafetyCarPredictionTexts[F1SafetyCarPredictionDto]}
              checked={selectedSafetyCarPrediction === F1SafetyCarPredictionDto}
              onChange={(_, checked) => {
                if (checked) {
                  setSelectedSafetyCarPrediction(F1SafetyCarPredictionDto);
                }
              }}
            />
          ))}
        </RadioGroup>
      </FormControl>
    </F1PredictionsSelectCard>
  );
}
