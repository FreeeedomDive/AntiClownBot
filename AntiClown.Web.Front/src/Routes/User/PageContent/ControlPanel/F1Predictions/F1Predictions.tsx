import { useParams } from "react-router-dom";
import React, { useEffect, useMemo, useState } from "react";
import {
  Button,
  Checkbox,
  FormControl,
  FormControlLabel,
  InputAdornment,
  MenuItem,
  OutlinedInput,
  Radio,
  RadioGroup,
  Select,
  SelectChangeEvent,
  Stack,
  Typography,
} from "@mui/material";
import SendIcon from "@mui/icons-material/Send";
import { F1DriverDto } from "../../../../../Dto/F1Predictions/F1DriverDto";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionDto,
  F1SafetyCarsPredictionObject,
} from "../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import F1Prediction from "./F1Prediction";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import { F1RaceDto } from "../../../../../Dto/F1Predictions/F1RaceDto";
import { Loader } from "../../../../../Components/Loader/Loader";

export default function F1Predictions() {
  const { userId } = useParams<"userId">();

  const [f1Races, setF1Races] = useState<F1RaceDto[] | undefined>();
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto | undefined>();

  useEffect(() => {
    async function load() {
      const result = await F1PredictionsApi.readAllActive();

      setF1Races(result);
      setCurrentF1Race(result[0]);
    }

    load();
  }, []);

  return (
    <Stack spacing={3} direction={"column"}>
      {f1Races ? (
        <Typography variant={"h5"}>Тут типа выбор гонки</Typography>
      ) : (
        <Loader />
      )}
      {currentF1Race ? <F1Prediction f1Race={currentF1Race} /> : null}
    </Stack>
  );
}
