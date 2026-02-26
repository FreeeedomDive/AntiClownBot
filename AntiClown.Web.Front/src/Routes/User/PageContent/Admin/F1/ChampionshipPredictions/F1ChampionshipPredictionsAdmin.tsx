import React, { useCallback, useEffect, useState } from "react";
import {
  Checkbox,
  FormControl,
  FormControlLabel,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Typography,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { Save } from "@mui/icons-material";
import { RightsWrapper } from "../../../../../../Components/RIghts/RightsWrapper";
import { RightsDto } from "../../../../../../Dto/Rights/RightsDto";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1ChampionshipPredictionsApi from "../../../../../../Api/F1ChampionshipPredictionsApi";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { F1ChampionshipPredictionTypeDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipPredictionTypeDto";
import { getDriversFromTeams } from "../../../../../../Dto/F1Predictions/F1DriversHelpers";
import F1ChampionshipDriverDnDList from "../../../ControlPanel/F1/ChampionshipPredictions/F1ChampionshipDriverDnDList";

const SEASON = new Date().getFullYear();

export default function F1ChampionshipPredictionsAdmin() {
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  const [standings, setStandings] = useState<string[]>([]);
  const [stage, setStage] = useState<F1ChampionshipPredictionTypeDto>(
    F1ChampionshipPredictionTypeDto.PreSeason,
  );
  const [isOpen, setIsOpen] = useState(false);

  useEffect(() => {
    async function load() {
      const [results, teams] = await Promise.all([
        F1ChampionshipPredictionsApi.readResults(SEASON),
        F1PredictionsApi.getActiveTeams(),
      ]);

      const defaultDrivers = getDriversFromTeams(teams);
      setStandings(results.hasData ? results.standings : defaultDrivers);
      setStage(results.hasData ? results.stage : F1ChampionshipPredictionTypeDto.PreSeason);
      setIsOpen(results.hasData ? results.isOpen : false);
      setIsLoading(false);
    }

    load().catch(console.error);
  }, []);

  const save = useCallback(async () => {
    setIsSaving(true);
    await F1ChampionshipPredictionsApi.writeResults(SEASON, {
      hasData: true,
      stage,
      isOpen,
      standings,
    });
    setIsSaving(false);
  }, [stage, isOpen, standings]);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
      {isLoading ? (
        <Loader />
      ) : (
        <Grid
          container
          spacing={2}
          sx={{ width: "100%", margin: "auto" }}
        >
          <Grid
            item
            xs={12}
            sm={12}
            md={12}
            lg={5}
            sx={{ display: "flex", flexDirection: "column" }}
          >
            <F1ChampionshipDriverDnDList
              title="Текущий чемпионат"
              description="Актуальный порядок пилотов в чемпионате"
              droppableId="championship-admin-dnd"
              drivers={standings}
              setDrivers={setStandings}
              editable={true}
              disabled={false}
            />
          </Grid>
          <Grid
            item
            xs={12}
            sm={12}
            md={12}
            lg={5}
            sx={{ display: "flex", flexDirection: "column" }}
          >
            <Stack direction="column" spacing={2}>
              <Typography variant="h6">Управление</Typography>
              <FormControl fullWidth>
                <InputLabel>Этап</InputLabel>
                <Select
                  label="Этап"
                  value={stage}
                  onChange={(e) =>
                    setStage(e.target.value as F1ChampionshipPredictionTypeDto)
                  }
                >
                  <MenuItem value={F1ChampionshipPredictionTypeDto.PreSeason}>
                    Предсезонный
                  </MenuItem>
                  <MenuItem value={F1ChampionshipPredictionTypeDto.MidSeason}>
                    Летние каникулы
                  </MenuItem>
                </Select>
              </FormControl>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={isOpen}
                    onChange={(e) => setIsOpen(e.target.checked)}
                  />
                }
                label="Предсказания открыты"
              />
              <LoadingButton
                loading={isSaving}
                disabled={isSaving}
                color="success"
                size="large"
                variant="contained"
                startIcon={<Save />}
                onClick={save}
              >
                Сохранить
              </LoadingButton>
            </Stack>
          </Grid>
        </Grid>
      )}
    </RightsWrapper>
  );
}
