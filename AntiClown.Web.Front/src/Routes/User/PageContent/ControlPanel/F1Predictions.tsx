import {useParams} from "react-router-dom";
import React, {useState} from "react";
import {
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
  Stack,
  Typography,
} from "@mui/material";
import {F1DriverDto} from "../../../../Dto/F1Predictions/F1DriverDto";
import {F1SafetyCarsPredictionDto} from "../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";

const DRIVER_PAIRS = [
  [F1DriverDto.Verstappen, F1DriverDto.Perez],
  [F1DriverDto.Leclerc, F1DriverDto.Sainz],
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

const firstColumnWidth = 150;
const teamButtonWidth = 150;

export default function F1Predictions() {
  const {userId} = useParams<"userId">();

  const [selected10Position, setSelected10Position] = useState<
    string | undefined
  >();

  const [isDNFNobody, setIsDNFNobody] = useState(false);

  const [dnf1st, setDnf1st] = useState<
    string | undefined
  >();
  const [dnf2nd, setDnf2nd] = useState<
    string | undefined
  >();
  const [dnf3rd, setDnf3rd] = useState<
    string | undefined
  >();
  const [dnf4th, setDnf4th] = useState<
    string | undefined
  >();
  const [dnf5th, setDnf5th] = useState<
    string | undefined
  >();
  const [selectedSafetyCarPrediction, setSafetyCarPrediction] = useState(F1SafetyCarsPredictionDto.Zero)
  const [firstPlaceLead, setFirstPlaceLead] = useState("")
  const [selectedDriversFromTeams] = useState<F1DriverDto[]>([])

  return (
    <Stack spacing={3} direction={"row"} width={"100%"}>
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
                setSelected10Position(event.target.value);
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
                  value={isDNFNobody}
                  onChange={() => {
                    setIsDNFNobody((checked) => !checked);
                  }}
                />
              }
              label="Никто"
            />
            {!isDNFNobody && (
              <>
                <Select
                  labelId="dnf-1st"
                  id="dnf-1st-select"
                  value={dnf1st}
                  onChange={(event) => {
                    setDnf1st(event.target.value);
                  }}
                >
                  {DRIVERS.map((driver) => (
                    <MenuItem key={driver} value={driver}>
                      {driver}
                    </MenuItem>
                  ))}
                </Select>

                <Select
                  labelId="dnf-2nd"
                  id="dnf-2nd-select"
                  value={dnf2nd}
                  onChange={(event) => {
                    setDnf2nd(event.target.value);
                  }}
                >
                  {DRIVERS.map((driver) => (
                    <MenuItem key={driver} value={driver}>
                      {driver}
                    </MenuItem>
                  ))}
                </Select>

                <Select
                  labelId="dnf-3rd"
                  id="dnf-3rd-select"
                  value={dnf3rd}
                  onChange={(event) => {
                    setDnf3rd(event.target.value);
                  }}
                >
                  {DRIVERS.map((driver) => (
                    <MenuItem key={driver} value={driver}>
                      {driver}
                    </MenuItem>
                  ))}
                </Select>

                <Select
                  labelId="dnf-4th"
                  id="dnf-4th-select"
                  value={dnf4th}
                  onChange={(event) => {
                    setDnf4th(event.target.value);
                  }}
                >
                  {DRIVERS.map((driver) => (
                    <MenuItem key={driver} value={driver}>
                      {driver}
                    </MenuItem>
                  ))}
                </Select>

                <Select
                  labelId="dnf-5th"
                  id="dnf-5th-select"
                  value={dnf5th}
                  onChange={(event) => {
                    setDnf5th(event.target.value);
                  }}
                >
                  {DRIVERS.map((driver) => (
                    <MenuItem key={driver} value={driver}>
                      {driver}
                    </MenuItem>
                  ))}
                </Select>
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
              <FormControlLabel
                value="0"
                control={<Radio/>}
                label="0"
                checked={selectedSafetyCarPrediction === F1SafetyCarsPredictionDto.Zero}
                onChange={(_, checked) => {
                  if (checked) {
                    setSafetyCarPrediction(F1SafetyCarsPredictionDto.Zero)
                  }
                }}
              />
              <FormControlLabel
                value="1"
                control={<Radio/>}
                label="1"
                checked={selectedSafetyCarPrediction === F1SafetyCarsPredictionDto.One}
                onChange={(_, checked) => {
                  if (checked) {
                    setSafetyCarPrediction(F1SafetyCarsPredictionDto.One)
                  }
                }}
              />
              <FormControlLabel
                value="2"
                control={<Radio/>}
                label="2"
                checked={selectedSafetyCarPrediction === F1SafetyCarsPredictionDto.Two}
                onChange={(_, checked) => {
                  if (checked) {
                    setSafetyCarPrediction(F1SafetyCarsPredictionDto.Two)
                  }
                }}
              />
              <FormControlLabel
                value="3"
                control={<Radio/>}
                label="3"
                checked={selectedSafetyCarPrediction === F1SafetyCarsPredictionDto.ThreePlus}
                onChange={(_, checked) => {
                  if (checked) {
                    setSafetyCarPrediction(F1SafetyCarsPredictionDto.ThreePlus)
                  }
                }}
              />
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
              type="number"
              placeholder="5.169"
              aria-describedby="outlined-weight-helper-text"
              inputProps={{
                "aria-label": "weight",
              }}
              value={firstPlaceLead}
              onChange={event => setFirstPlaceLead(event.target.value)}
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
        >
          <Typography variant="h6" flexShrink={0} width={firstColumnWidth}>
            Команды
          </Typography>
          <Stack flexGrow={1} spacing={1}>
            {DRIVER_PAIRS.map(([driver1, driver2]) => (
              <Stack
                direction={"row"}
                alignItems={"center"}
                justifyContent={"space-between"}
                spacing={3}
              >
                <Button
                  variant={selectedDriversFromTeams.includes(driver1) ? "contained" : "outlined"}
                  style={{width: teamButtonWidth}}
                  onClick={event => {
                    const index = selectedDriversFromTeams.indexOf(driver2, 0);
                    if (index > -1) {
                      selectedDriversFromTeams.splice(index, 1);
                    }
                    selectedDriversFromTeams.push(driver1)
                  }}
                >
                  {driver1}
                </Button>
                <span>vs</span>
                <Button
                  variant={selectedDriversFromTeams.includes(driver2) ? "contained" : "outlined"}
                  style={{width: teamButtonWidth}}
                  onClick={event => {
                    const index = selectedDriversFromTeams.indexOf(driver1, 0);
                    if (index > -1) {
                      selectedDriversFromTeams.splice(index, 1);
                    }
                    selectedDriversFromTeams.push(driver2)
                  }}
                >
                  {driver2}
                </Button>
              </Stack>
            ))}
          </Stack>
        </Stack>
      </Stack>
    </Stack>
  );
}
