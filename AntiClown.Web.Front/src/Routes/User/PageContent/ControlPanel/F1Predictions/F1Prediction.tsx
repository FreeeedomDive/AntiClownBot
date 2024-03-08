import {useParams} from "react-router-dom";
import React, {useCallback, useMemo, useState} from "react";
import {
  Alert,
  Button,
  Checkbox,
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
import SendIcon from "@mui/icons-material/Send";
import {F1DriverDto} from "../../../../../Dto/F1Predictions/F1DriverDto";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionDto,
  F1SafetyCarsPredictionObject,
} from "../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import {LoadingButton} from "@mui/lab";
import {AddPredictionResultDto} from "../../../../../Dto/F1Predictions/AddPredictionResultDto";

const DRIVER_PAIRS = [
  [F1DriverDto.Verstappen, F1DriverDto.Perez],
  [F1DriverDto.Leclerc, F1DriverDto.Bearman],
  [F1DriverDto.Hamilton, F1DriverDto.Russell],
  [F1DriverDto.Ocon, F1DriverDto.Gasly],
  [F1DriverDto.Piastri, F1DriverDto.Norris],
  [F1DriverDto.Bottas, F1DriverDto.Zhou],
  [F1DriverDto.Stroll, F1DriverDto.Alonso],
  [F1DriverDto.Magnussen, F1DriverDto.Hulkenberg],
  [F1DriverDto.Ricciardo, F1DriverDto.Tsunoda],
  [F1DriverDto.Albon, F1DriverDto.Sargeant],
] as const;

const DRIVERS = DRIVER_PAIRS.flatMap((pair) => pair);

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

export default function F1Prediction({f1Race}: { f1Race: F1RaceDto }) {
  const {userId} = useParams<"userId">();

  const userPrediction = useMemo(() => {
    return f1Race.predictions.find(
      (prediction) => prediction.userId === userId
    );
  }, [f1Race, userId]);

  const [selected10Position, setSelected10Position] = useState<F1DriverDto>(
    userPrediction?.tenthPlacePickedDriver ?? F1DriverDto.Verstappen
  );

  const [isDNFNobody, setIsDNFNobody] = useState(() => {
    console.log("dnfPrediction", userPrediction?.dnfPrediction);

    return userPrediction?.dnfPrediction?.noDnfPredicted ?? false;
  });

  console.log({
    isDNFNobody,
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
            Количество машин безопасности
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
                <span>vs</span>
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
              </Stack>
            ))}
          </Stack>
        </Stack>
        <LoadingButton
          loading={isSaving}
          disabled={!isValid || isSaving}
          size="large"
          variant="contained"
          endIcon={<SendIcon/>}
          style={{margin: "auto", marginBottom: "0", width: "50%"}}
          onClick={saveF1Prediction}
        >
          Сохранить
        </LoadingButton>
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
    </Stack>
  );
}
