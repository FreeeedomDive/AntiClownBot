import React, { useEffect, useState } from "react";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { RightsWrapper } from "../../../../../../Components/UserRights/RightsWrapper";
import { RightsDto } from "../../../../../../Dto/Rights/RightsDto";
import {
  Checkbox,
  FormControl,
  FormControlLabel,
  MenuItem,
  Select,
  Stack,
} from "@mui/material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1Prediction from "./F1Prediction";

export default function F1PredictionsList() {
  const [f1Races, setF1Races] = useState<F1RaceDto[] | undefined>();
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto | undefined>();
  const [isActive, setIsActive] = useState(true);

  function fetchRaces(onlyActive: boolean) {
    return F1PredictionsApi.find({
      season: new Date().getFullYear(),
      isActive: onlyActive ? true : undefined,
    });
  }

  useEffect(() => {
    fetchRaces(true)
      .then((result) => {
        setF1Races(result);
        setCurrentF1Race(result[0]);
      })
      .catch(console.error);
  }, []);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
      <Stack spacing={1} direction={"column"}>
        {f1Races ? (
          <Stack direction={"row"} spacing={1} alignItems="center">
            <FormControl fullWidth size="small">
              <Select
                labelId="race-select"
                id="race-select"
                value={currentF1Race?.id ?? ""}
                onChange={(e) => {
                  setCurrentF1Race(
                    f1Races.find((r) => r.id === e.target.value),
                  );
                }}
              >
                {f1Races.map((race) => (
                  <MenuItem key={race.id} value={race.id}>
                    {race.name} {race.season}
                    {race.isSprint ? " (спринт)" : ""}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <FormControlLabel
              sx={{ whiteSpace: "nowrap", mr: 0 }}
              control={
                <Checkbox
                  size="small"
                  checked={isActive}
                  onChange={(x) => {
                    const onlyActive = x.target.checked;
                    setIsActive(onlyActive);
                    fetchRaces(onlyActive)
                      .then((result) => {
                        setF1Races(result);
                        setCurrentF1Race(
                          onlyActive ? result[0] : result.at(-1),
                        );
                      })
                      .catch(console.error);
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
          <F1Prediction key={currentF1Race.id} f1Race={currentF1Race} />
        ) : null}
      </Stack>
    </RightsWrapper>
  );
}
