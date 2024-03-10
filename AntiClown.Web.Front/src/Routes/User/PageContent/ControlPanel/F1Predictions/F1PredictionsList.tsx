import React, {useEffect, useState} from "react";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";
import {RightsWrapper} from "../../../../../Components/RIghts/RightsWrapper";
import {RightsDto} from "../../../../../Dto/Rights/RightsDto";
import {FormControl, MenuItem, Select, Stack, Typography} from "@mui/material";
import {Loader} from "../../../../../Components/Loader/Loader";
import F1Prediction from "./F1Prediction";

export default function F1PredictionsList() {
  const [f1Races, setF1Races] = useState<F1RaceDto[] | undefined>();
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto | undefined>();
  const [selectedRace, setSelectedRace] = useState("")

  useEffect(() => {
    async function load() {
      const result = await F1PredictionsApi.readAllActive();

      setF1Races(result);
      setCurrentF1Race(result[0]);
      setSelectedRace(result[0]?.name)
    }

    load();
  }, []);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
      <Stack spacing={3} direction={"column"}>
        {
          f1Races && f1Races.length > 0 ?
            <FormControl fullWidth>
              <Select
                labelId="race-select"
                id="race-select"
                value={selectedRace}
                onChange={(event) => {
                  const value = event.target.value;
                  setSelectedRace(value)
                  const race = f1Races?.find(x => x.name === value)
                  setCurrentF1Race(race)
                }}
              >
                {f1Races.map((race) => (
                  <MenuItem key={race.id} value={race.name}>
                    {race.name}{" "}{race.season}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            :
            f1Races && f1Races.length === 0 ?
              (
                <Typography variant={"h5"}>На данный момент нет активных предсказаний</Typography>
              ) : (
                <Loader/>
              )
        }
        {
          currentF1Race
            ? <F1Prediction f1Race={currentF1Race}/>
            : null
        }
      </Stack>
    </RightsWrapper>
  );
}
