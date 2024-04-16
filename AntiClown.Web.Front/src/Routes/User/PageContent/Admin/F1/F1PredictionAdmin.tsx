import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";
import React, {useCallback, useState} from "react";
import {Button, FormControl, InputAdornment, OutlinedInput, Stack, Typography} from "@mui/material";
import F1RaceClassifications from "./F1RaceClassifications";
import SendIcon from "@mui/icons-material/Send";
import {LoadingButton} from "@mui/lab";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1PredictionAdmin({f1Race}: Props) {
  const [incidents, setIncidents] = useState(f1Race.result?.safetyCars ?? 0)
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>(
    String(f1Race.result?.firstPlaceLead ?? "0")
  );
  const [isSaving, setIsSaving] = useState(false);
  const [isFinishing, setIsFinishing] = useState(false);

  const saveRaceResults = useCallback(async () => {
    setIsSaving(true);

    setIsSaving(false);
  }, []);

  const finishRace = useCallback(async () => {
    setIsFinishing(true);

    setIsFinishing(false);
  }, []);

  return (
    <Stack direction={"row"} spacing={16}>
      <F1RaceClassifications f1Race={f1Race}/>
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