import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";
import React, {useCallback, useState} from "react";
import {Button, FormControl, InputAdornment, OutlinedInput, Stack, Typography} from "@mui/material";
import F1RaceClassifications from "./F1RaceClassifications";
import SendIcon from "@mui/icons-material/Send";
import {LoadingButton} from "@mui/lab";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import {DRIVERS} from "../../../../../Dto/F1Predictions/F1DriversHelpers";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1PredictionAdmin({f1Race}: Props) {
  const [drivers, setDrivers] = useState(
    f1Race.result.classification.length === 0 ? DRIVERS : f1Race.result.classification
  );
  const [dnfDrivers, setDnfDrivers] = useState(new Set(f1Race.result?.dnfDrivers ?? []));
  const [incidents, setIncidents] = useState(f1Race.result?.safetyCars ?? 0)
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>(
    String(f1Race.result?.firstPlaceLead ?? "0")
  );
  const [isSaving, setIsSaving] = useState(false);
  const [isFinishing, setIsFinishing] = useState(false);

  const saveRaceResults = useCallback(async () => {
    setIsSaving(true);
    await F1PredictionsApi.addResult(f1Race.id, {
      raceId: f1Race.id,
      firstPlaceLead: Number(firstPlaceLead),
      safetyCars: incidents,
      dnfDrivers: Array.from(dnfDrivers.values()),
      classification: drivers
    })
    setIsSaving(false);
  }, [dnfDrivers, drivers, f1Race.id, firstPlaceLead, incidents]);

  const finishRace = useCallback(async () => {
    setIsFinishing(true);
    await F1PredictionsApi.finish(f1Race.id)
    setIsFinishing(false);
  }, [f1Race.id]);

  return (
    <Stack direction={"row"} spacing={16}>
      <F1RaceClassifications
        drivers={drivers}
        setDrivers={setDrivers}
        dnfDrivers={dnfDrivers}
        setDnfDrivers={setDnfDrivers}
      />
      <Stack direction={"column"} spacing={4}>
        <Stack direction={"row"} spacing={4} height={"45px"}>
          <Button
            variant="contained"
            color="error"
            disabled={incidents === 0}
            onClick={() => setIncidents(incidents - 1)}
          >
            <Typography variant="h4">-</Typography>
          </Button>
          <Typography variant="h6">Инциденты (VSC, SC, RedFlag): {incidents}</Typography>
          <Button
            variant="contained"
            color="success"
            onClick={() => setIncidents(incidents + 1)}
          >
            <Typography variant="h4">+</Typography>
          </Button>
        </Stack>
        <Stack
          spacing={4}
          direction="row"
          alignItems={"flex-start"}
          justifyContent={"space-between"}
        >
          <Typography variant="h6">
            Отрыв 1 места
          </Typography>
          <FormControl>
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
        </Stack>
        <LoadingButton
          loading={isSaving}
          disabled={isSaving}
          color="success"
          size="large"
          variant="contained"
          endIcon={<SendIcon/>}
          onClick={saveRaceResults}
        >
          Сохранить
        </LoadingButton>
        <LoadingButton
          loading={isFinishing}
          disabled={isFinishing}
          color="primary"
          size="large"
          variant="contained"
          endIcon={<SendIcon/>}
          onClick={finishRace}
        >
          Завершить гонку и рассчитать результаты
        </LoadingButton>
      </Stack>
    </Stack>
  )
}