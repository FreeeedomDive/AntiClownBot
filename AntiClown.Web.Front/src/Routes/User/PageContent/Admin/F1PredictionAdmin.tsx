import {F1RaceDto} from "../../../../Dto/F1Predictions/F1RaceDto";
import React from "react";
import {Stack, Typography} from "@mui/material";
import F1RaceClassifications from "./F1RaceClassifications";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1PredictionAdmin({f1Race}: Props) {
  return (
    <Stack direction={"column"}>
      <Typography variant={"h5"}>Админка на гонку {f1Race.name} {f1Race.season}</Typography>
      <F1RaceClassifications />
    </Stack>
  )
}