import React, { useEffect, useState } from "react";
import {
  Alert,
  Box,
  Chip,
  Divider,
  IconButton,
  InputAdornment,
  Paper,
  Snackbar,
  Stack,
  Tab,
  Tabs,
  TextField,
  Typography,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { Add, Block, Done, Remove, Save } from "@mui/icons-material";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { Loader } from "../../../../../../Components/Loader/Loader";
import { getDriversFromTeams } from "../../../../../../Dto/F1Predictions/F1DriversHelpers";
import { F1PredictionDto } from "../../../../../../Dto/F1Predictions/F1PredictionDto";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1RaceClassifications from "./F1RaceClassifications";
import F1UserPredictionsTable from "./F1UserPredictionsTable";

interface Props {
  f1Race: F1RaceDto;
}

const PANEL_SX = {
  borderRadius: 2,
  bgcolor: "rgba(255,255,255,0.025)",
  backgroundImage: "none",
  minWidth: 0,
} as const;

export default function F1PredictionAdmin({ f1Race }: Props) {
  const [data, setData] = useState<{ race: F1RaceDto; teams: F1TeamDto[] }>();
  const [error, setError] = useState(false);

  useEffect(() => {
    let cancelled = false;
    Promise.all([
      F1PredictionsApi.read(f1Race.id),
      F1PredictionsApi.getActiveTeams(),
    ])
      .then(([race, teams]) => {
        if (!cancelled) setData({ race, teams });
      })
      .catch(() => {
        if (!cancelled) setError(true);
      });
    return () => {
      cancelled = true;
    };
  }, [f1Race.id]);

  if (error)
    return <Alert severity="error">Не удалось загрузить данные гонки</Alert>;
  if (!data) return <Loader />;

  return (
    <F1PredictionAdminEditor
      key={data.race.id}
      race={data.race}
      teams={data.teams}
    />
  );
}

function F1PredictionAdminEditor({
  race,
  teams,
}: {
  race: F1RaceDto;
  teams: F1TeamDto[];
}) {
  const [savedRace, setSavedRace] = useState(race);
  const initialGrid = race.qualifyingGrid?.length
    ? race.qualifyingGrid
    : race.result.classification.length
      ? race.result.classification
      : getDriversFromTeams(teams);
  const [qualifyingGrid, setQualifyingGrid] = useState(initialGrid);
  const [drivers, setDrivers] = useState(
    race.result.classification.length
      ? race.result.classification
      : initialGrid,
  );
  const [dnfDrivers, setDnfDrivers] = useState(new Set(race.result.dnfDrivers));
  const [incidents, setIncidents] = useState(race.result.safetyCars);
  const [firstPlaceLead, setFirstPlaceLead] = useState(
    String(race.result.firstPlaceLead),
  );
  const [action, setAction] = useState<
    "qualifying" | "result" | "close" | "finish" | null
  >(null);
  const [isSavingPrediction, setIsSavingPrediction] = useState(false);
  const [dirtyPredictions, setDirtyPredictions] = useState<Set<string>>(
    new Set(),
  );
  const [needsRecalculation, setNeedsRecalculation] = useState(false);
  const [tab, setTab] = useState(0);
  const [notification, setNotification] = useState<{
    severity: "success" | "error";
    message: string;
  } | null>(null);

  const qualifyingDirty =
    JSON.stringify(qualifyingGrid) !==
    JSON.stringify(savedRace.qualifyingGrid ?? []);
  const result = {
    raceId: race.id,
    classification: drivers,
    dnfDrivers: Array.from(dnfDrivers).sort(),
    safetyCars: incidents,
    firstPlaceLead: Number(firstPlaceLead),
  };
  const resultDirty =
    JSON.stringify(result) !==
      JSON.stringify({
        raceId: race.id,
        classification: savedRace.result.classification,
        dnfDrivers: [...savedRace.result.dnfDrivers].sort(),
        safetyCars: savedRace.result.safetyCars,
        firstPlaceLead: savedRace.result.firstPlaceLead,
      }) || firstPlaceLead.trim() === "";
  const validLead =
    firstPlaceLead.trim() !== "" &&
    Number.isFinite(Number(firstPlaceLead)) &&
    Number(firstPlaceLead) >= 0;
  const isBusy = action !== null || isSavingPrediction;
  const hasUnsavedChanges =
    qualifyingDirty || resultDirty || dirtyPredictions.size > 0;
  const hasClassification = savedRace.result.classification.length > 0;
  const positionDriver = race.conditions?.positionPredictionDriver;
  const driverOptions = Array.from(
    new Set([
      ...initialGrid,
      ...race.result.classification,
      ...race.result.dnfDrivers,
      ...(positionDriver ? [positionDriver] : []),
      ...race.predictions.flatMap((prediction) => [
        prediction.tenthPlacePickedDriver,
        ...(prediction.dnfPrediction.dnfPickedDrivers ?? []),
        ...(prediction.teamsPickedDrivers ?? []),
      ]),
    ]),
  ).filter(Boolean);

  async function saveQualifying() {
    setAction("qualifying");
    try {
      await F1PredictionsApi.saveQualifyingGrid(race.id, qualifyingGrid);
      setSavedRace((previous) => ({
        ...previous,
        qualifyingGrid: [...qualifyingGrid],
      }));
      setNotification({
        severity: "success",
        message: "Стартовая решётка сохранена",
      });
    } catch {
      setNotification({
        severity: "error",
        message: "Не удалось сохранить стартовую решётку",
      });
    } finally {
      setAction(null);
    }
  }

  async function saveResult() {
    setAction("result");
    try {
      await F1PredictionsApi.addResult(race.id, result);
      setSavedRace((previous) => ({ ...previous, result }));
      if (!savedRace.isActive) setNeedsRecalculation(true);
      setNotification({
        severity: "success",
        message: "Результат гонки сохранён",
      });
    } catch {
      setNotification({
        severity: "error",
        message: "Не удалось сохранить результат гонки",
      });
    } finally {
      setAction(null);
    }
  }

  async function closePredictions() {
    setAction("close");
    try {
      await F1PredictionsApi.close(race.id);
      setSavedRace((previous) => ({ ...previous, isOpened: false }));
      setNotification({
        severity: "success",
        message: "Приём предсказаний закрыт",
      });
    } catch {
      setNotification({
        severity: "error",
        message: "Не удалось закрыть предсказания",
      });
    } finally {
      setAction(null);
    }
  }

  async function finishRace() {
    setAction("finish");
    try {
      await F1PredictionsApi.finish(race.id);
      setSavedRace((previous) => ({
        ...previous,
        isOpened: false,
        isActive: false,
      }));
      setNeedsRecalculation(false);
      setNotification({
        severity: "success",
        message: "Результаты предсказаний рассчитаны",
      });
    } catch {
      setNotification({
        severity: "error",
        message: "Не удалось рассчитать результаты",
      });
    } finally {
      setAction(null);
    }
  }

  function predictionSaved(prediction: F1PredictionDto) {
    setSavedRace((previous) => ({
      ...previous,
      predictions: previous.predictions.map((item) =>
        item.userId === prediction.userId ? prediction : item,
      ),
    }));
    if (!savedRace.isActive) setNeedsRecalculation(true);
  }

  function predictionDirtyChanged(userId: string, dirty: boolean) {
    setDirtyPredictions((previous) => {
      const next = new Set(previous);
      if (dirty) next.add(userId);
      else next.delete(userId);
      return next;
    });
  }

  return (
    <Stack spacing={2}>
      <Stack direction="row" gap={1} flexWrap="wrap" alignItems="center">
        <Typography variant="h6" sx={{ mr: 1 }}>
          {race.name}
          {race.isSprint ? " · Спринт" : ""}
        </Typography>
        <Chip
          size="small"
          variant="outlined"
          color={savedRace.isOpened ? "success" : "default"}
          label={
            savedRace.isOpened ? "Предсказания открыты" : "Предсказания закрыты"
          }
        />
        <Chip
          size="small"
          variant="outlined"
          color={savedRace.isActive ? "primary" : "default"}
          label={savedRace.isActive ? "Гонка активна" : "Гонка завершена"}
        />
        {needsRecalculation && (
          <Chip
            size="small"
            color="warning"
            variant="outlined"
            label="Нужен пересчёт"
          />
        )}
        {savedRace.isOpened && (
          <LoadingButton
            size="small"
            loading={action === "close"}
            disabled={isBusy}
            startIcon={<Block />}
            onClick={closePredictions}
            sx={{ ml: { sm: "auto" }, textTransform: "none" }}
          >
            Закрыть предсказания
          </LoadingButton>
        )}
      </Stack>
      <Tabs
        value={tab}
        onChange={(_, value) => setTab(value)}
        variant="scrollable"
        scrollButtons="auto"
        sx={{ minHeight: 40, borderBottom: 1, borderColor: "divider" }}
      >
        <Tab
          label="Результаты и решётка"
          sx={{ textTransform: "none", minHeight: 40 }}
        />
        <Tab
          label={`Предсказания · ${savedRace.predictions.length}`}
          sx={{ textTransform: "none", minHeight: 40 }}
        />
      </Tabs>
      <Box hidden={tab !== 0}>
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: {
              xs: "minmax(0, 1fr)",
              md: "repeat(2, minmax(0, 1fr))",
              xl: "minmax(0, 1fr) minmax(0, 1fr) 320px",
            },
            gap: 2,
            alignItems: "start",
          }}
        >
          <Paper variant="outlined" sx={PANEL_SX}>
            <Stack
              direction="row"
              justifyContent="space-between"
              alignItems="center"
              gap={1}
              sx={{ p: 2 }}
            >
              <Typography fontWeight={600}>Стартовая решётка</Typography>
              {qualifyingDirty && (
                <Chip
                  label="Есть изменения"
                  size="small"
                  variant="outlined"
                  color="warning"
                />
              )}
            </Stack>
            <F1RaceClassifications
              drivers={qualifyingGrid}
              setDrivers={setQualifyingGrid}
              dnfDrivers={new Set()}
              setDnfDrivers={() => {}}
              teams={teams}
              showDnf={false}
              disabled={isBusy}
            />
            <Box sx={{ p: 2 }}>
              <LoadingButton
                variant="outlined"
                loading={action === "qualifying"}
                disabled={
                  isBusy || !qualifyingDirty || qualifyingGrid.length === 0
                }
                startIcon={<Save />}
                onClick={saveQualifying}
                sx={{ textTransform: "none" }}
              >
                Сохранить решётку
              </LoadingButton>
            </Box>
          </Paper>
          <Paper variant="outlined" sx={PANEL_SX}>
            <Stack
              direction="row"
              justifyContent="space-between"
              alignItems="center"
              gap={1}
              sx={{ p: 2 }}
            >
              <Typography fontWeight={600}>Результат гонки</Typography>
              {resultDirty && (
                <Chip
                  label="Есть изменения"
                  size="small"
                  variant="outlined"
                  color="warning"
                />
              )}
            </Stack>
            <F1RaceClassifications
              drivers={drivers}
              setDrivers={setDrivers}
              dnfDrivers={dnfDrivers}
              setDnfDrivers={setDnfDrivers}
              teams={teams}
              disabled={isBusy}
              highlightedDriver={positionDriver}
            />
            <Box sx={{ p: 2 }}>
              <LoadingButton
                variant="outlined"
                loading={action === "result"}
                disabled={
                  isBusy || !resultDirty || !validLead || drivers.length === 0
                }
                startIcon={<Save />}
                onClick={saveResult}
                sx={{ textTransform: "none" }}
              >
                Сохранить результат
              </LoadingButton>
            </Box>
          </Paper>
          <Paper
            variant="outlined"
            sx={{ ...PANEL_SX, p: 2, gridColumn: { md: "1 / -1", xl: "auto" } }}
          >
            <Stack spacing={2}>
              <Typography fontWeight={600}>Параметры гонки</Typography>
              <Stack spacing={0.5}>
                <Typography variant="body2">Инциденты</Typography>
                <Stack direction="row" alignItems="center" spacing={2}>
                  <IconButton
                    size="small"
                    aria-label="Уменьшить число инцидентов"
                    disabled={isBusy || incidents === 0}
                    onClick={() => setIncidents(incidents - 1)}
                    sx={{
                      flex: 1,
                      minHeight: 32,
                      border: 1,
                      borderColor: "divider",
                      borderRadius: 1,
                    }}
                  >
                    <Remove fontSize="small" />
                  </IconButton>
                  <Typography
                    sx={{
                      minWidth: 24,
                      textAlign: "center",
                      fontVariantNumeric: "tabular-nums",
                    }}
                  >
                    {incidents}
                  </Typography>
                  <IconButton
                    size="small"
                    aria-label="Увеличить число инцидентов"
                    disabled={isBusy}
                    onClick={() => setIncidents(incidents + 1)}
                    sx={{
                      flex: 1,
                      minHeight: 32,
                      border: 1,
                      borderColor: "divider",
                      borderRadius: 1,
                    }}
                  >
                    <Add fontSize="small" />
                  </IconButton>
                </Stack>
                <Typography variant="caption" color="text.secondary">
                  VSC, SC и красные флаги
                </Typography>
              </Stack>
              <TextField
                label="Отрыв победителя"
                size="small"
                fullWidth
                value={firstPlaceLead}
                disabled={isBusy}
                error={!validLead}
                helperText={
                  !validLead
                    ? "Укажите неотрицательное число"
                    : "Сохраняется вместе с результатом гонки"
                }
                inputProps={{ inputMode: "decimal" }}
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">сек</InputAdornment>
                  ),
                }}
                onChange={(event) => {
                  const value = event.target.value.replace(",", ".");
                  if (/^\d*(\.\d*)?$/.test(value)) setFirstPlaceLead(value);
                }}
              />
              <Divider />
              <Typography fontWeight={600}>
                {savedRace.isActive
                  ? "Расчёт предсказаний"
                  : "Пересчёт предсказаний"}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {hasUnsavedChanges
                  ? "Сначала сохраните изменения в решётке, результате и предсказаниях пользователей."
                  : !hasClassification
                    ? "Сохраните результат гонки перед расчётом."
                    : "Баллы рассчитываются по сохранённому результату гонки и предсказаниям пользователей."}
              </Typography>
              <LoadingButton
                variant="contained"
                loading={action === "finish"}
                disabled={isBusy || hasUnsavedChanges || !hasClassification}
                startIcon={<Done />}
                onClick={finishRace}
                sx={{ textTransform: "none" }}
              >
                {savedRace.isActive
                  ? "Завершить и рассчитать"
                  : "Пересчитать результаты"}
              </LoadingButton>
            </Stack>
          </Paper>
        </Box>
      </Box>
      <Box hidden={tab !== 1}>
        <F1UserPredictionsTable
          race={savedRace}
          teams={teams}
          drivers={driverOptions}
          disabled={isBusy}
          onSavingChange={setIsSavingPrediction}
          onDirtyChange={predictionDirtyChanged}
          onSaved={predictionSaved}
        />
      </Box>
      <Snackbar
        open={notification !== null}
        autoHideDuration={4000}
        onClose={() => setNotification(null)}
      >
        <Alert
          severity={notification?.severity}
          onClose={() => setNotification(null)}
          variant="filled"
        >
          {notification?.message}
        </Alert>
      </Snackbar>
    </Stack>
  );
}
