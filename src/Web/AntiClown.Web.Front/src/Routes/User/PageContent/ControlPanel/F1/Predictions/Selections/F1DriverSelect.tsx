import React, { useId } from "react";
import {
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Typography,
} from "@mui/material";
import { F1TeamDto } from "../../../../../../../Dto/F1Predictions/F1TeamDto";
import F1DriverOption from "./F1DriverOption";

interface Props {
  value: string;
  onChange: (driver: string) => void;
  drivers: string[];
  teams: F1TeamDto[];
  qualifyingGrid?: string[];
  label?: string;
  ariaLabel?: string;
  disabled?: boolean;
}

export default function F1DriverSelect({
  value,
  onChange,
  drivers,
  teams,
  qualifyingGrid,
  label,
  ariaLabel = "Пилот",
  disabled = false,
}: Props) {
  const labelId = useId();

  return (
    <FormControl size="small" fullWidth disabled={disabled}>
      {label && (
        <InputLabel id={labelId} shrink>
          {label}
        </InputLabel>
      )}
      <Select
        labelId={label ? labelId : undefined}
        label={label}
        notched={!!label}
        value={value}
        displayEmpty
        inputProps={{ "aria-label": ariaLabel }}
        onChange={(event) => onChange(event.target.value)}
        renderValue={(driver) =>
          driver ? (
            <F1DriverOption driver={driver} teams={teams} />
          ) : (
            <Typography variant="body2" color="text.secondary">
              Выберите пилота
            </Typography>
          )
        }
        MenuProps={{ PaperProps: { sx: { maxHeight: 440 } } }}
      >
        {drivers.map((driver) => (
          <MenuItem key={driver} value={driver}>
            <F1DriverOption
              driver={driver}
              teams={teams}
              qualifyingGrid={qualifyingGrid}
            />
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}
