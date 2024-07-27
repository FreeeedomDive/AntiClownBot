import {useParams} from "react-router-dom";
import React, {useCallback, useEffect, useMemo, useState} from "react";
import {
  Alert,
  Button, ButtonGroup,
  Checkbox, Fab,
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
import {F1DriverDto} from "../../../../../Dto/F1Predictions/F1DriverDto";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionDto,
  F1SafetyCarsPredictionObject,
} from "../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import {AddPredictionResultDto} from "../../../../../Dto/F1Predictions/AddPredictionResultDto";
import {DRIVER_PAIRS, DRIVERS} from "../../../../../Dto/F1Predictions/F1DriversHelpers";
import {Save} from "@mui/icons-material";

const isDriver = (driver: string): driver is F1DriverDto => {
  return driver in F1DriverDto;
};

const F1SafetyCarPredictionTexts: Record<F1SafetyCarPredictionDto, string> = {
  Zero: "0",
  One: "1",
  Two: "2",
  ThreePlus: "3+",
};

const firstColumnWidth = 150;
const teamButtonWidth = 150;

type DNFList = [
  F1DriverDto,
  F1DriverDto,
  F1DriverDto,
  F1DriverDto,
  F1DriverDto,
];

const fabStyle = {
  position: 'absolute',
  bottom: 16,
  right: 16,
};

interface Props {
  f1Race: F1RaceDto;
}

export default function F1Prediction({f1Race}: Props) {
  const {userId} = useParams<"userId">();
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto>(f1Race);

  useEffect(() => {
    async function load() {
      const result = await F1PredictionsApi.read(f1Race.id);
      setCurrentF1Race(result);
    }

    load();
  }, [f1Race.id]);

  const userPrediction = useMemo(() => {
    return currentF1Race.predictions.find(
      (prediction) => prediction.userId === userId
    );
  }, [currentF1Race, userId]);

  const [selected10Position, setSelected10Position] = useState<F1DriverDto>(
    userPrediction?.tenthPlacePickedDriver ?? F1DriverDto.Verstappen
  );

  const [isDNFNobody, setIsDNFNobody] = useState(() => {
    return userPrediction?.dnfPrediction?.noDnfPredicted ?? false;
  });

  const [dnfList, setDnfList] = useState<DNFList>(() => {
    if (
      userPrediction?.dnfPrediction.dnfPickedDrivers?.length === 5
    ) {
      return userPrediction.dnfPrediction.dnfPickedDrivers as DNFList;
    }

    return [
      F1DriverDto.Verstappen,
      F1DriverDto.Verstappen,
      F1DriverDto.Verstappen,
      F1DriverDto.Verstappen,
      F1DriverDto.Verstappen,
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
        }) as DNFList
      );
    };

  const [selectedSafetyCarPrediction, setSafetyCarPrediction] =
    useState<F1SafetyCarPredictionDto>(
      userPrediction?.safetyCarsPrediction ?? F1SafetyCarsPredictionObject.Zero
    );
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>(
    String(userPrediction?.firstPlaceLeadPrediction ?? "")
  );
  const [selectedDriversFromTeams, setSelectedDriversFromTeams] = useState<
    Set<F1DriverDto>
  >(() => {
    const initialArray = (() => userPrediction?.teamsPickedDrivers)();

    return new Set(initialArray ?? []);
  });

  const isValid = useMemo(() => {
    if (!isDNFNobody && new Set(dnfList).size !== 5) {
      return false;
    }

    if (!firstPlaceLead || Number(firstPlaceLead) < 0) {
      return false;
    }

    return selectedDriversFromTeams.size === 10;


  }, [dnfList, firstPlaceLead, isDNFNobody, selectedDriversFromTeams.size]);

  const [isSaving, setIsSaving] = useState(false);
  const [savePredictionResult, setSavePredictionResult]
    = useState<AddPredictionResultDto | null>(null);
  const saveF1Prediction = useCallback(async () => {
    if (!userId || !isValid) {
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
    userId,
  ]);

  return (
    <Stack direction={"row"} width={"100%"}>
      <Stack flexGrow={1} padding={1} spacing={4} width={"50%"}>
        <Stack
          spacing={2}
          direction="row"
          alignItems={"center"}
          justifyContent={"space-between"}
        >
          <Typography variant="h6" flexShrink={0} width={firstColumnWidth}>
            10 место:{" "}
          </Typography>
          <FormControl fullWidth>
            <Select
              labelId="10-position"
              id="10-position-select"
              value={selected10Position}
              onChange={(event) => {
                const value = event.target.value;
                if (isDriver(value)) {
                  setSelected10Position(value);
                }
              }}
            >
              {DRIVERS.map((driver) => (
                <MenuItem key={driver} value={driver}>
                  {driver}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Stack>

        <Stack
          spacing={2}
          direction="row"
          alignItems={"flex-start"}
          justifyContent={"space-between"}
        >
          <Typography variant="h6" flexShrink={0} width={firstColumnWidth}>
            DNF
          </Typography>
          <Stack flexGrow={1} spacing={1}>
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
                      {DRIVERS.filter(
                        (driver) =>
                          !dnfList.includes(driver) || dnfItem === driver
                      ).map((driver) => (
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
        </Stack>

        <Stack
          spacing={2}
          direction="row"
          alignItems={"flex-start"}
          justifyContent={"space-between"}
        >
          <Typography variant="h6" flexShrink={0} width={firstColumnWidth}>
            Количество инцидентов (VSC, SC, Red)
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
                  control={<Radio/>}
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
        </Stack>

        <Stack
          spacing={2}
          direction="row"
          alignItems={"flex-start"}
          justifyContent={"space-between"}
        >
          <Typography variant="h6" flexShrink={0} width={firstColumnWidth}>
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
        </Stack>
      </Stack>
      <Stack flexGrow={1} padding={1} spacing={2} width={"50%"}>
        <Stack
          spacing={2}
          direction="row"
          alignItems={"flex-start"}
          justifyContent={"space-between"}
          style={{paddingBottom: "40px"}}
        >
          <Typography variant="h6" flexShrink={0} width={firstColumnWidth}>
            Команды
          </Typography>
          <Stack flexGrow={1} spacing={1}>
            {DRIVER_PAIRS.map(([driver1, driver2]) => (
              <Stack
                key={driver1 + driver2}
                direction={"row"}
                alignItems={"center"}
                justifyContent={"space-between"}
                spacing={3}
              >
                <ButtonGroup size="large">
                  <Button
                    variant={
                      selectedDriversFromTeams.has(driver1)
                        ? "contained"
                        : "outlined"
                    }
                    style={{width: teamButtonWidth}}
                    onClick={() => {
                      selectedDriversFromTeams.delete(driver2);
                      selectedDriversFromTeams.add(driver1);

                      setSelectedDriversFromTeams(
                        new Set(selectedDriversFromTeams)
                      );
                    }}
                  >
                    {driver1}
                  </Button>
                  <Button
                    variant={
                      selectedDriversFromTeams.has(driver2)
                        ? "contained"
                        : "outlined"
                    }
                    style={{width: teamButtonWidth}}
                    onClick={() => {
                      selectedDriversFromTeams.delete(driver1);
                      selectedDriversFromTeams.add(driver2);

                      setSelectedDriversFromTeams(
                        new Set(selectedDriversFromTeams)
                      );
                    }}
                  >
                    {driver2}
                  </Button>
                </ButtonGroup>
              </Stack>
            ))}
          </Stack>
        </Stack>
      </Stack>
      <Snackbar
        open={savePredictionResult !== null}
        autoHideDuration={3000}
        onClose={() => setSavePredictionResult(null)}
      >
        <Alert severity={
          savePredictionResult === AddPredictionResultDto.Success
            ? "success"
            : "warning"
        }>
          {
            savePredictionResult === AddPredictionResultDto.Success
              ? "Предсказания успешно сохранены"
              : "Предсказания на эту гонку уже закрыты"
          }
        </Alert>
      </Snackbar>
      <Fab
        disabled={!isValid || isSaving}
        variant="extended"
        sx={fabStyle}
        onClick={saveF1Prediction}
        color={"success"}
      >
        <Save/>
        {isSaving ? (<>Сохранение...</>) : (<>Сохранить</>)}
      </Fab>
    </Stack>
  )
}
