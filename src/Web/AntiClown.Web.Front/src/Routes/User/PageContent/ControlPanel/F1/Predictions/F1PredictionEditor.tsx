import React, { useEffect, useState } from "react";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Box,
  Chip,
  Divider,
  FormControl,
  InputAdornment,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Skeleton,
  Snackbar,
  Stack,
  TextField,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { CheckCircleOutline, ExpandMore, Save } from "@mui/icons-material";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { Loader } from "../../../../../../Components/Loader/Loader";
import { AddPredictionResultDto } from "../../../../../../Dto/F1Predictions/AddPredictionResultDto";
import { F1DnfPredictionDto } from "../../../../../../Dto/F1Predictions/F1DnfPredictionDto";
import {
  getPredictionDrivers,
  getPredictionMaxPosition,
  isValidDnfPrediction,
} from "../../../../../../Dto/F1Predictions/F1PredictionHelpers";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionDto,
} from "../../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1QualifyingGridView from "./F1QualifyingGridView";
import F1RaceSeasonContext from "./F1RaceSeasonContext";
import F1TeamBadge from "./F1TeamBadge";
import F1DnfSelect from "./Selections/F1DnfSelect";
import F1DriverSelect from "./Selections/F1DriverSelect";

interface Props {
  f1Race: F1RaceDto;
  userId: string;
  mobile?: boolean;
}

interface PredictionDraft {
  tenthPlacePickedDriver: string;
  dnfPrediction: F1DnfPredictionDto;
  safetyCarsPrediction: F1SafetyCarPredictionDto | "";
  firstPlaceLeadPrediction: string;
  driverPositionPrediction: number | null;
}

const PANEL_SX = {
  borderRadius: 2,
  bgcolor: "rgba(255,255,255,0.025)",
  backgroundImage: "none",
  minWidth: 0,
} as const;

const INCIDENT_LABELS: Record<F1SafetyCarPredictionDto, string> = {
  Zero: "0",
  One: "1",
  Two: "2",
  ThreePlus: "3+",
};

export default function F1PredictionEditor({
  f1Race,
  userId,
  mobile = false,
}: Props) {
  const [loaded, setLoaded] = useState<{
    raceId: string;
    userId: string;
    data?: { race: F1RaceDto; teams: F1TeamDto[] };
    error?: boolean;
  }>();

  useEffect(() => {
    let cancelled = false;
    Promise.all([
      F1PredictionsApi.read(f1Race.id, userId),
      F1PredictionsApi.getActiveTeams(),
    ])
      .then(([race, teams]) => {
        if (!cancelled) {
          setLoaded({ raceId: f1Race.id, userId, data: { race, teams } });
        }
      })
      .catch(() => {
        if (!cancelled) setLoaded({ raceId: f1Race.id, userId, error: true });
      });
    return () => {
      cancelled = true;
    };
  }, [f1Race.id, userId]);

  if (!loaded || loaded.raceId !== f1Race.id || loaded.userId !== userId) {
    return <Loader />;
  }
  if (loaded.error || !loaded.data) {
    return <Alert severity="error">Не удалось загрузить предсказание</Alert>;
  }

  return (
    <F1RaceSeasonContext.Provider value={loaded.data.race.season}>
      <F1PredictionForm
        key={`${loaded.data.race.id}-${userId}`}
        f1Race={loaded.data.race}
        teams={loaded.data.teams}
        userId={userId}
        mobile={mobile}
      />
    </F1RaceSeasonContext.Provider>
  );
}

function F1PredictionForm({
  f1Race,
  teams,
  userId,
  mobile = false,
}: Props & {
  teams: F1TeamDto[];
}) {
  const userPrediction = f1Race.predictions.find(
    (prediction) => prediction.userId === userId,
  );
  const [draft, setDraft] = useState<PredictionDraft>(() => ({
    tenthPlacePickedDriver: userPrediction?.tenthPlacePickedDriver ?? "",
    dnfPrediction: userPrediction?.dnfPrediction ?? {
      noDnfPredicted: false,
      dnfPickedDrivers: [],
    },
    safetyCarsPrediction: userPrediction?.safetyCarsPrediction ?? "",
    firstPlaceLeadPrediction: String(
      userPrediction?.firstPlaceLeadPrediction ?? "",
    ),
    driverPositionPrediction: userPrediction?.driverPositionPrediction ?? null,
  }));
  const [savedDraft, setSavedDraft] = useState<PredictionDraft | null>(
    userPrediction ? draft : null,
  );
  const [isOpened, setIsOpened] = useState(f1Race.isOpened);
  const [isSaving, setIsSaving] = useState(false);
  const [gridExpanded, setGridExpanded] = useState(false);
  const [notification, setNotification] = useState<{
    severity: "success" | "warning" | "error";
    message: string;
  } | null>(null);

  const drivers = getPredictionDrivers(f1Race, teams);
  const maxPosition = getPredictionMaxPosition(f1Race, teams);
  const positionDriver = f1Race.conditions?.positionPredictionDriver;
  const disabled = !isOpened || isSaving;
  const dirty =
    !savedDraft || JSON.stringify(draft) !== JSON.stringify(savedDraft);
  const validDnf = isValidDnfPrediction(draft.dnfPrediction);
  const validLead =
    draft.firstPlaceLeadPrediction.trim() !== "" &&
    Number.isFinite(Number(draft.firstPlaceLeadPrediction)) &&
    Number(draft.firstPlaceLeadPrediction) >= 0;
  const validPosition =
    !positionDriver ||
    (draft.driverPositionPrediction !== null &&
      Number.isInteger(draft.driverPositionPrediction) &&
      draft.driverPositionPrediction >= 1 &&
      draft.driverPositionPrediction <= maxPosition);
  const valid =
    !!draft.tenthPlacePickedDriver &&
    validDnf &&
    validLead &&
    !!draft.safetyCarsPrediction &&
    validPosition;
  const dnfCount = draft.dnfPrediction.dnfPickedDrivers?.length ?? 0;

  let saveHint = "Есть несохранённые изменения";
  if (!dirty) saveHint = "Предсказание сохранено";
  else if (!draft.tenthPlacePickedDriver)
    saveHint = "Выберите пилота на 10-е место";
  else if (!validDnf)
    saveHint =
      dnfCount > 0 && dnfCount < 5
        ? `Выберите ещё ${5 - dnfCount} пилотов для DNF или «Никто»`
        : "Выберите 5 разных пилотов для DNF или «Никто»";
  else if (!draft.safetyCarsPrediction) saveHint = "Выберите число инцидентов";
  else if (!validLead) saveHint = "Укажите неотрицательный отрыв победителя";
  else if (!validPosition) saveHint = `Выберите позицию ${positionDriver}`;

  function update(patch: Partial<PredictionDraft>) {
    setDraft((previous) => ({ ...previous, ...patch }));
  }

  async function savePrediction() {
    if (
      !userId ||
      !isOpened ||
      isSaving ||
      !dirty ||
      !valid ||
      !draft.safetyCarsPrediction
    )
      return;
    setIsSaving(true);
    try {
      const result = await F1PredictionsApi.addPrediction(f1Race.id, {
        ...userPrediction,
        userId,
        raceId: f1Race.id,
        tenthPlacePickedDriver: draft.tenthPlacePickedDriver,
        dnfPrediction: draft.dnfPrediction,
        safetyCarsPrediction: draft.safetyCarsPrediction,
        firstPlaceLeadPrediction: Number(draft.firstPlaceLeadPrediction),
        driverPositionPrediction: draft.driverPositionPrediction ?? 0,
      });
      if (result === AddPredictionResultDto.PredictionsClosed) {
        setIsOpened(false);
        if (savedDraft) setDraft(savedDraft);
        setNotification({
          severity: "warning",
          message: "Приём предсказаний уже закрыт. Изменения не сохранены.",
        });
      } else if (result === AddPredictionResultDto.Success) {
        setSavedDraft(draft);
        setNotification({
          severity: "success",
          message: "Предсказание сохранено",
        });
      } else {
        throw new Error("Prediction was not saved");
      }
    } catch {
      setNotification({
        severity: "error",
        message: "Не удалось сохранить предсказание. Попробуйте ещё раз.",
      });
    } finally {
      setIsSaving(false);
    }
  }

  const qualifyingGridContent = f1Race.qualifyingGrid?.length ? (
    <F1QualifyingGridView
      grid={f1Race.qualifyingGrid}
      teams={teams}
      showTitle={false}
      tenthPlaceDriver={draft.tenthPlacePickedDriver}
      dnfDrivers={
        draft.dnfPrediction.noDnfPredicted
          ? []
          : draft.dnfPrediction.dnfPickedDrivers ?? []
      }
      positionDriver={positionDriver}
      predictedPosition={validPosition ? draft.driverPositionPrediction : null}
    />
  ) : (
    <Stack spacing={0.75} sx={{ px: 2, pb: 2, opacity: 0.4 }}>
      {Array.from({ length: maxPosition || 22 }, (_, index) => (
        <Skeleton
          key={index}
          variant="rectangular"
          height={28}
          sx={{ borderRadius: 0.5 }}
        />
      ))}
    </Stack>
  );

  return (
    <Stack
      spacing={2}
      sx={{ maxWidth: mobile ? 640 : 996, width: "100%", minWidth: 0 }}
    >
      <Stack direction="row" alignItems="center" flexWrap="wrap" gap={1}>
        <Typography variant="h6" sx={{ mr: 1 }}>
          {f1Race.name} · {f1Race.season}
          {f1Race.isSprint ? " · Спринт" : ""}
        </Typography>
        <Chip
          size="small"
          variant="outlined"
          color={isOpened ? "success" : "default"}
          label={isOpened ? "Приём открыт" : "Приём закрыт, только просмотр"}
        />
        {savedDraft && (
          <Chip
            size="small"
            variant="outlined"
            color={dirty && isOpened ? "warning" : "default"}
            label={
              dirty && isOpened ? "Есть изменения" : "Предсказание сохранено"
            }
          />
        )}
      </Stack>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: mobile
            ? "minmax(0, 1fr)"
            : {
                xs: "minmax(0, 1fr)",
                lg: "340px minmax(0, 1fr)",
              },
          alignItems: "start",
          gap: 2,
        }}
      >
        {mobile ? (
          <Accordion
            variant="outlined"
            disableGutters
            expanded={gridExpanded}
            onChange={(_, expanded) => setGridExpanded(expanded)}
            sx={{
              ...PANEL_SX,
              m: 0,
              "&:before": { display: "none" },
              "&.Mui-expanded": { m: 0 },
            }}
          >
            <AccordionSummary expandIcon={<ExpandMore />}>
              <Typography fontWeight={600}>Стартовая решётка</Typography>
            </AccordionSummary>
            <AccordionDetails sx={{ p: 0 }}>
              {qualifyingGridContent}
            </AccordionDetails>
          </Accordion>
        ) : (
          <Paper
            variant="outlined"
            sx={{ ...PANEL_SX, order: { xs: 2, lg: 0 } }}
          >
            <Typography fontWeight={600} sx={{ p: 2 }}>
              Стартовая решётка
            </Typography>
            {qualifyingGridContent}
          </Paper>
        )}
        <Stack
          spacing={2}
          sx={{
            minWidth: 0,
            width: "100%",
            maxWidth: 640,
            order: mobile ? 0 : { xs: 1, lg: 0 },
          }}
        >
          {!isOpened && !savedDraft ? (
            <Alert severity="info">
              Вы не оставили предсказание на эту гонку. Приём уже закрыт.
            </Alert>
          ) : (
            <Paper variant="outlined" sx={{ ...PANEL_SX, p: 2 }}>
              <Stack spacing={3}>
                <Box>
                  <F1DriverSelect
                    label="10-е место"
                    ariaLabel="Пилот на 10-е место"
                    value={draft.tenthPlacePickedDriver}
                    onChange={(driver) =>
                      update({ tenthPlacePickedDriver: driver })
                    }
                    drivers={drivers}
                    teams={teams}
                    qualifyingGrid={f1Race.qualifyingGrid}
                    disabled={disabled}
                  />
                </Box>
                <F1DnfSelect
                  label="DNF"
                  value={draft.dnfPrediction}
                  onChange={(dnfPrediction) => update({ dnfPrediction })}
                  drivers={drivers}
                  teams={teams}
                  qualifyingGrid={f1Race.qualifyingGrid}
                  disabled={disabled}
                />
                <Stack spacing={3}>
                  <Stack spacing={0.75}>
                    <Typography variant="body2">Инциденты</Typography>
                    <ToggleButtonGroup
                      value={draft.safetyCarsPrediction}
                      exclusive
                      fullWidth
                      size="small"
                      color="primary"
                      disabled={disabled}
                      aria-label="Число инцидентов"
                      onChange={(_, value: F1SafetyCarPredictionDto | null) => {
                        if (value !== null)
                          update({ safetyCarsPrediction: value });
                      }}
                    >
                      {F1SafetyCarsPredictionDto.map((value) => (
                        <ToggleButton
                          key={value}
                          value={value}
                          sx={{ flex: 1 }}
                        >
                          {INCIDENT_LABELS[value]}
                        </ToggleButton>
                      ))}
                    </ToggleButtonGroup>
                    <Typography variant="caption" color="text.secondary">
                      VSC, SC и красные флаги
                    </Typography>
                  </Stack>
                  <TextField
                    label="Отрыв победителя"
                    size="small"
                    fullWidth
                    value={draft.firstPlaceLeadPrediction}
                    disabled={disabled}
                    inputProps={{ inputMode: "decimal" }}
                    InputProps={{
                      endAdornment: (
                        <InputAdornment position="end">сек</InputAdornment>
                      ),
                    }}
                    onChange={(event) => {
                      const value = event.target.value.replace(",", ".");
                      if (/^\d*(\.\d*)?$/.test(value))
                        update({ firstPlaceLeadPrediction: value });
                    }}
                  />
                  {positionDriver && (
                    <Stack spacing={1}>
                      <Box
                        sx={{ display: "flex", alignItems: "center", gap: 1 }}
                      >
                        <Typography variant="body2">
                          Позиция {positionDriver}
                        </Typography>
                        <F1TeamBadge driver={positionDriver} teams={teams} />
                      </Box>
                      <FormControl fullWidth size="small" disabled={disabled}>
                        <InputLabel
                          id="prediction-driver-position-label"
                          shrink
                        >
                          Место на финише
                        </InputLabel>
                        <Select
                          labelId="prediction-driver-position-label"
                          label="Место на финише"
                          notched
                          displayEmpty
                          value={
                            validPosition
                              ? draft.driverPositionPrediction ?? ""
                              : ""
                          }
                          onChange={(event) =>
                            update({
                              driverPositionPrediction: Number(
                                event.target.value,
                              ),
                            })
                          }
                        >
                          <MenuItem value="" disabled>
                            Выберите позицию
                          </MenuItem>
                          {Array.from(
                            { length: maxPosition },
                            (_, index) => index + 1,
                          ).map((position) => (
                            <MenuItem key={position} value={position}>
                              {position}
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    </Stack>
                  )}
                </Stack>
                {isOpened && (
                  <>
                    <Divider />
                    <Stack
                      direction={
                        mobile ? "column" : { xs: "column", sm: "row" }
                      }
                      spacing={2}
                      alignItems={mobile ? "stretch" : { sm: "center" }}
                      justifyContent="space-between"
                    >
                      <Stack direction="row" spacing={1} alignItems="center">
                        {!dirty && (
                          <CheckCircleOutline
                            fontSize="small"
                            color="success"
                          />
                        )}
                        <Typography
                          variant="body2"
                          color={!dirty ? "success.main" : "text.secondary"}
                        >
                          {isSaving ? "Сохраняем предсказание…" : saveHint}
                        </Typography>
                      </Stack>
                      <LoadingButton
                        variant="contained"
                        loading={isSaving}
                        disabled={disabled || !dirty || !valid || !userId}
                        startIcon={<Save />}
                        onClick={savePrediction}
                        fullWidth={mobile}
                        sx={{
                          textTransform: "none",
                          flexShrink: 0,
                          minHeight: mobile ? 44 : undefined,
                        }}
                      >
                        Сохранить предсказание
                      </LoadingButton>
                    </Stack>
                  </>
                )}
              </Stack>
            </Paper>
          )}
        </Stack>
      </Box>
      <Snackbar
        open={notification !== null}
        autoHideDuration={4000}
        onClose={() => setNotification(null)}
      >
        <Alert
          severity={notification?.severity}
          variant="filled"
          onClose={() => setNotification(null)}
        >
          {notification?.message}
        </Alert>
      </Snackbar>
    </Stack>
  );
}
