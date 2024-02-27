import { useParams } from "react-router-dom";
import React, { useState } from "react";
import {
  Box,
  Button,
  Checkbox,
  FilledInput,
  FormControl,
  FormControlLabel,
  InputAdornment,
  InputLabel,
  MenuItem,
  OutlinedInput,
  Radio,
  RadioGroup,
  Select,
  Stack,
  TextField,
  Typography,
} from "@mui/material";

const DRIVER_PAIRS = [
  ["Verstappen", "Perez"],
  ["Leclerc", "Sainz"],
  ["Hamilton", "Russell"],
  ["Ocon", "Gasly"],
  ["Piastri", "Norris"],
  ["Bottas", "Zhou"],
  ["Stroll", "Alonso"],
  ["Magnussen", "Hulkenberg"],
  ["Ricciardo", "Tsunoda"],
  ["Albon", "Sargeant"],
] as const;

const DRIVERS = DRIVER_PAIRS.flatMap((pair) => pair);

const firstColumnWidth = 150;
const teamButtonWidth = 150;

export default function F1Predictions() {
  const { userId } = useParams<"userId">();

  const [selected10Position, setSelected10Position] = useState<
    string | undefined
  >();

  const [isDNFNobody, setIsDNFNobody] = useState(false);

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
              <FormControlLabel value="0" control={<Radio />} label="0" />
              <FormControlLabel value="1" control={<Radio />} label="1" />
              <FormControlLabel value="2" control={<Radio />} label="2" />
              <FormControlLabel value="3+" control={<Radio />} label="3+" />
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
            Лидирование 1 места
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
                <Button variant="contained" style={{ width: teamButtonWidth }}>
                  {driver1}
                </Button>
                <span>vs</span>
                <Button variant="outlined" style={{ width: teamButtonWidth }}>
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
