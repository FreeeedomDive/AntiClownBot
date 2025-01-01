import React, { useEffect, useState } from "react";
import { F1RaceDto } from "../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import { RightsWrapper } from "../../../../../Components/RIghts/RightsWrapper";
import { RightsDto } from "../../../../../Dto/Rights/RightsDto";
import {
  Checkbox,
  FormControl,
  FormControlLabel,
  MenuItem,
  Select,
  Stack,
} from "@mui/material";
import { Loader } from "../../../../../Components/Loader/Loader";
import F1PredictionAdmin from "./F1PredictionAdmin";

export default function F1PredictionsAdminList() {
  const [f1Races, setF1Races] = useState<F1RaceDto[] | undefined>();
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto | undefined>();
  const [isActive, setIsActive] = useState(true);

  async function loadRaces(onlyActive: boolean) {
    const result = await F1PredictionsApi.find({
      season: new Date().getFullYear(),
      isActive: onlyActive ? true : undefined,
    });

    setF1Races(result);
    setCurrentF1Race(result[0]);
  }

  useEffect(() => {
    loadRaces(isActive);
  }, []);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
      <Stack spacing={3} direction={"column"}>
        {f1Races ? (
          <Stack direction={"row"} spacing={1}>
            <FormControl fullWidth>
              <Select
                labelId="race-select"
                id="race-select"
                key={currentF1Race?.id ?? ""}
                value={currentF1Race}
                onChange={(selectedRace) => {
                  setCurrentF1Race(selectedRace.target.value as F1RaceDto);
                }}
              >
                {f1Races.map((race) => (
                  //@ts-ignore - necessary to load object into value
                  <MenuItem key={race.id} value={race}>
                    {race.name}
                    {race.isSprint ? " (спринт) " : " "}
                    {race.season}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <FormControlLabel
              control={
                <Checkbox
                  checked={isActive}
                  onChange={(x) => {
                    const onlyActive = x.target.checked;
                    setIsActive(onlyActive);
                    loadRaces(onlyActive);
                  }}
                />
              }
              label={"Только текущие гонки"}
            />
          </Stack>
        ) : (
          <Loader />
        )}
        {currentF1Race ? (
          <F1PredictionAdmin key={currentF1Race.id} f1Race={currentF1Race} />
        ) : null}
      </Stack>
    </RightsWrapper>
  );
}
