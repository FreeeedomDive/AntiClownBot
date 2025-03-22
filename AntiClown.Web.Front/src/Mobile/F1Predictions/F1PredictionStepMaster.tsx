import { F1RaceDto } from "../../Dto/F1Predictions/F1RaceDto";
import {
  Alert,
  Button,
  ButtonGroup,
  Checkbox,
  Divider,
  FormControl,
  FormControlLabel,
  InputAdornment,
  MenuItem,
  OutlinedInput,
  Radio,
  RadioGroup,
  Select,
  SelectChangeEvent,
  Snackbar,
  Stack,
  Typography,
} from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionDto,
  F1SafetyCarsPredictionObject,
} from "../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import { useStore } from "../../Stores";
import { F1TeamDto } from "../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsApi from "../../Api/F1PredictionsApi";
import { getDriversFromTeams } from "../../Dto/F1Predictions/F1DriversHelpers";
import { AddPredictionResultDto } from "../../Dto/F1Predictions/AddPredictionResultDto";

const F1SafetyCarPredictionTexts: Record<F1SafetyCarPredictionDto, string> = {
  Zero: "0",
  One: "1",
  Two: "2",
  ThreePlus: "3+",
};

type DNFList = [string, string, string, string, string];

interface Props {
  f1Race: F1RaceDto;
}

export default function F1PredictionStepMaster({ f1Race }: Props) {
  const maxSteps = 5;
  const [step, setStep] = useState(1);
  const { mobileUserContextStore } = useStore();
  const userId = mobileUserContextStore.user?.id;
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto>(f1Race);
  const [teams, setTeams] = useState<F1TeamDto[]>([]);
  const [isSaving, setIsSaving] = useState(false);
  const [savePredictionResult, setSavePredictionResult] =
    useState<AddPredictionResultDto | null>(null);

  useEffect(() => {
    async function load() {
      const result = await F1PredictionsApi.read(f1Race.id);
      setCurrentF1Race(result);

      const teams = await F1PredictionsApi.getActiveTeams();
      setTeams(teams);
    }

    load();
  }, [f1Race.id]);

  const userPrediction = useMemo(() => {
    return currentF1Race.predictions.find(
      (prediction) => prediction.userId === userId,
    );
  }, [currentF1Race, userId]);

  const [selected10Position, setSelected10Position] = useState<string>(
    userPrediction?.tenthPlacePickedDriver ?? teams[0]?.firstDriver,
  );

  const [isDNFNobody, setIsDNFNobody] = useState(() => {
    return userPrediction?.dnfPrediction?.noDnfPredicted ?? false;
  });

  const [dnfList, setDnfList] = useState<DNFList>(() => {
    if (userPrediction?.dnfPrediction.dnfPickedDrivers?.length === 5) {
      return userPrediction.dnfPrediction.dnfPickedDrivers as DNFList;
    }

    return [
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
    ];
  });
  const onDnfListChange =
    (changingIndex: number) => (event: SelectChangeEvent) => {
      setDnfList(
        dnfList.map((dnfItem, index) => {
          if (index === changingIndex) {
            return event.target.value;
          }

          return dnfItem;
        }) as DNFList,
      );
    };

  const [selectedSafetyCarPrediction, setSafetyCarPrediction] =
    useState<F1SafetyCarPredictionDto>(
      userPrediction?.safetyCarsPrediction ?? F1SafetyCarsPredictionObject.Zero,
    );
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>(
    String(userPrediction?.firstPlaceLeadPrediction ?? ""),
  );
  const [selectedDriversFromTeams, setSelectedDriversFromTeams] = useState<
    Set<string>
  >(() => {
    const initialArray = (() => userPrediction?.teamsPickedDrivers)();

    return new Set(initialArray ?? []);
  });

  const isValid = useMemo(() => {
    if (step === 1) {
      return !!selected10Position;
    }

    if (step === 2) {
      return isDNFNobody || new Set(dnfList).size === 5;
    }

    if (step === 3) {
      return true;
    }

    if (step === 4) {
      return firstPlaceLead && Number(firstPlaceLead) > 0;
    }

    return selectedDriversFromTeams.size === 10;
  }, [
    dnfList,
    firstPlaceLead,
    isDNFNobody,
    selected10Position,
    selectedDriversFromTeams.size,
    step,
  ]);

  const saveF1Prediction = useCallback(async () => {
    if (!userId || !isValid) {
      return;
    }

    if (step < maxSteps) {
      setStep(step + 1);
      return;
    }

    setIsSaving(true);

    const result = await F1PredictionsApi.addPrediction(f1Race.id, {
      userId,
      raceId: f1Race.id,
      firstPlaceLeadPrediction: Number(firstPlaceLead),
      safetyCarsPrediction: selectedSafetyCarPrediction,
      teamsPickedDrivers: Array.from(selectedDriversFromTeams.values()),
      tenthPlacePickedDriver: selected10Position,
      dnfPrediction: {
        noDnfPredicted: isDNFNobody,
        dnfPickedDrivers: isDNFNobody ? null : dnfList,
      },
    });

    setSavePredictionResult(result);
    setIsSaving(false);
  }, [
    dnfList,
    f1Race.id,
    firstPlaceLead,
    isDNFNobody,
    isValid,
    selected10Position,
    selectedDriversFromTeams,
    selectedSafetyCarPrediction,
    step,
    userId,
  ]);

  return !mobileUserContextStore.isUnknown &&
    !!mobileUserContextStore.user?.id ? (
    <Stack
      spacing={1}
      direction="column"
      alignItems={"center"}
      justifyContent={"space-between"}
    >
      {!currentF1Race.isOpened && (
        <Alert severity="warning">Предсказания закрыты</Alert>
      )}
      {step === 1 && (
        <>
          <Typography variant="body1" flexShrink={0}>
            10 место
          </Typography>
          <FormControl fullWidth>
            <Select
              labelId="10-position"
              id="10-position-select"
              value={selected10Position}
              onChange={(event) => {
                const value = event.target.value;
                setSelected10Position(value);
              }}
            >
              {getDriversFromTeams(teams).map((driver) => (
                <MenuItem key={driver} value={driver}>
                  {driver}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </>
      )}
      {step === 2 && (
        <>
          <Typography variant="body1" flexShrink={0}>
            DNF
          </Typography>
          <Stack flexGrow={1} spacing={1} width={"100%"}>
            <FormControlLabel
              control={
                <Checkbox
                  checked={isDNFNobody}
                  onChange={() => {
                    setIsDNFNobody((checked) => !checked);
                  }}
                />
              }
              label="Никто"
            />
            {!isDNFNobody && (
              <>
                {dnfList.map((dnfItem, index) => {
                  return (
                    <Select
                      key={index}
                      labelId={`dnf-${index}`}
                      id={`dnf-${index}-select`}
                      value={dnfItem}
                      onChange={onDnfListChange(index)}
                    >
                      {getDriversFromTeams(teams)
                        .filter(
                          (driver) =>
                            !dnfList.includes(driver) || dnfItem === driver,
                        )
                        .map((driver) => (
                          <MenuItem key={driver} value={driver}>
                            {driver}
                          </MenuItem>
                        ))}
                    </Select>
                  );
                })}
              </>
            )}
          </Stack>
        </>
      )}
      {step === 3 && (
        <>
          <Typography variant="body1" flexShrink={0}>
            Инциденты (VSC, SC, Red)
          </Typography>
          <FormControl fullWidth>
            <RadioGroup
              aria-labelledby="demo-radio-buttons-group-label"
              defaultValue="female"
              name="radio-buttons-group"
            >
              {F1SafetyCarsPredictionDto.map((F1SafetyCarPredictionDto) => (
                <FormControlLabel
                  key={F1SafetyCarPredictionDto}
                  value={F1SafetyCarPredictionDto}
                  control={<Radio />}
                  label={F1SafetyCarPredictionTexts[F1SafetyCarPredictionDto]}
                  checked={
                    selectedSafetyCarPrediction === F1SafetyCarPredictionDto
                  }
                  onChange={(_, checked) => {
                    if (checked) {
                      setSafetyCarPrediction(F1SafetyCarPredictionDto);
                    }
                  }}
                />
              ))}
            </RadioGroup>
          </FormControl>
        </>
      )}
      {step === 4 && (
        <>
          <Typography variant="body1" flexShrink={0}>
            Отрыв 1 места
          </Typography>
          <FormControl fullWidth>
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
        </>
      )}
      {step === 5 && (
        <>
          <Typography variant="body1" flexShrink={0}>
            Команды
          </Typography>
          {teams.map((team) => (
            <ButtonGroup size="large" fullWidth>
              <Button
                variant={
                  selectedDriversFromTeams.has(team.firstDriver)
                    ? "contained"
                    : "outlined"
                }
                onClick={() => {
                  selectedDriversFromTeams.delete(team.secondDriver);
                  selectedDriversFromTeams.add(team.firstDriver);

                  setSelectedDriversFromTeams(
                    new Set(selectedDriversFromTeams),
                  );
                }}
              >
                {team.firstDriver}
              </Button>
              <Button
                variant={
                  selectedDriversFromTeams.has(team.secondDriver)
                    ? "contained"
                    : "outlined"
                }
                onClick={() => {
                  selectedDriversFromTeams.delete(team.firstDriver);
                  selectedDriversFromTeams.add(team.secondDriver);

                  setSelectedDriversFromTeams(
                    new Set(selectedDriversFromTeams),
                  );
                }}
              >
                {team.secondDriver}
              </Button>
            </ButtonGroup>
          ))}
        </>
      )}
      <Divider />
      <Divider />
      <ButtonGroup size="large" fullWidth>
        <Button
          variant={"outlined"}
          color={"error"}
          onClick={() => {
            setStep(step - 1);
          }}
          disabled={step === 1}
        >
          Назад
        </Button>
        <Button
          variant={step === maxSteps ? "contained" : "outlined"}
          color={"info"}
          onClick={saveF1Prediction}
          disabled={
            !isValid ||
            isSaving ||
            (step === maxSteps && !currentF1Race.isOpened)
          }
        >
          {isSaving
            ? "Сохранение..."
            : step === maxSteps
              ? "Сохранить"
              : "Далее"}
        </Button>
      </ButtonGroup>

      <Snackbar
        open={savePredictionResult !== null}
        autoHideDuration={3000}
        onClose={() => setSavePredictionResult(null)}
      >
        <Alert
          severity={
            savePredictionResult === AddPredictionResultDto.Success
              ? "success"
              : "warning"
          }
        >
          {savePredictionResult === AddPredictionResultDto.Success
            ? "Предсказания успешно сохранены"
            : "Предсказания на эту гонку уже закрыты"}
        </Alert>
      </Snackbar>
    </Stack>
  ) : (
    <></>
  );
}
