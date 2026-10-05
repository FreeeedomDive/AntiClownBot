import React, { useEffect, useState } from "react";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
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
import F1PredictionAdmin from "./F1PredictionAdmin";

export default function F1PredictionsAdminList({ season }: { season: number }) {
  const currentYear = new Date().getFullYear();
  const [f1Races, setF1Races] = useState<F1RaceDto[] | undefined>();
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto | undefined>();
  const [isActive, setIsActive] = useState(season === currentYear);

  useEffect(() => {
    let cancelled = false;
    F1PredictionsApi.find({ season, isActive: isActive ? true : undefined })
      .then((result) => {
        if (cancelled) return;
        setF1Races(result);
        setCurrentF1Race(isActive ? result[0] : result.at(-1));
      })
      .catch(console.error);
    return () => {
      cancelled = true;
    };
  }, [season, isActive]);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
      <Stack spacing={2} direction={"column"}>
        <Stack direction={"row"} spacing={1} alignItems="center">
          {f1Races ? (
            <>
              <FormControl fullWidth size="small">
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
                    // @ts-expect-error - necessary to load object into value
                    <MenuItem key={race.id} value={race}>
                      {race.name}
                      {race.isSprint ? " (спринт) " : " "}
                      {race.season}
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
                    onChange={(event) => {
                      setF1Races(undefined);
                      setCurrentF1Race(undefined);
                      setIsActive(event.target.checked);
                    }}
                  />
                }
                label={"Только текущие гонки"}
              />
            </>
          ) : (
            <Loader />
          )}
        </Stack>
        {currentF1Race ? (
          <F1PredictionAdmin key={currentF1Race.id} f1Race={currentF1Race} />
        ) : null}
      </Stack>
    </RightsWrapper>
  );
}
