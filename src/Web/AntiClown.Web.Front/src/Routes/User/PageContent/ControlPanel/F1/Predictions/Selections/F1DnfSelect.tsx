import React, { useId } from "react";
import {
  Box,
  Checkbox,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
} from "@mui/material";
import { F1DnfPredictionDto } from "../../../../../../../Dto/F1Predictions/F1DnfPredictionDto";
import { F1TeamDto } from "../../../../../../../Dto/F1Predictions/F1TeamDto";
import F1DriverOption from "./F1DriverOption";

interface Props {
  value: F1DnfPredictionDto;
  onChange: (prediction: F1DnfPredictionDto) => void;
  drivers: string[];
  teams: F1TeamDto[];
  qualifyingGrid?: string[];
  label?: string;
  ariaLabel?: string;
  disabled?: boolean;
  summaryMaxWidth?: number;
}

const NOBODY = "__nobody__";

export default function F1DnfSelect({
  value,
  onChange,
  drivers,
  teams,
  qualifyingGrid,
  label,
  ariaLabel = "DNF",
  disabled = false,
  summaryMaxWidth,
}: Props) {
  const labelId = useId();
  const selected = value.noDnfPredicted ? [] : value.dnfPickedDrivers ?? [];
  const values = value.noDnfPredicted ? [NOBODY] : selected;
  const selectedInGridOrder = drivers.filter((driver) =>
    selected.includes(driver),
  );
  const selectedDrivers = [
    ...selectedInGridOrder,
    ...selected.filter((driver) => !drivers.includes(driver)),
  ];

  function changeSelection(next: string[]) {
    const choseNobody = next.includes(NOBODY) && !value.noDnfPredicted;
    onChange(
      choseNobody
        ? {
            noDnfPredicted: true,
            dnfPickedDrivers: null,
          }
        : {
            noDnfPredicted: false,
            dnfPickedDrivers: next
              .filter((driver) => driver !== NOBODY)
              .slice(0, 5),
          },
    );
  }

  return (
    <FormControl
      size="small"
      fullWidth
      disabled={disabled}
      sx={{ minWidth: 0 }}
    >
      {label && (
        <InputLabel id={labelId} shrink>
          {label}
        </InputLabel>
      )}
      <Select
        multiple
        displayEmpty
        labelId={label ? labelId : undefined}
        label={label}
        notched={!!label}
        value={values}
        inputProps={{ "aria-label": ariaLabel }}
        onChange={(event) =>
          changeSelection(
            typeof event.target.value === "string"
              ? event.target.value.split(",")
              : event.target.value,
          )
        }
        renderValue={() => {
          const text = value.noDnfPredicted
            ? "Никто"
            : selectedDrivers.length
              ? `${selectedDrivers.length}/5 · ${selectedDrivers.join(", ")}`
              : "Выберите пилотов или «Никто»";
          return (
            <Box
              title={text}
              sx={{
                overflow: "hidden",
                textOverflow: "ellipsis",
                whiteSpace: "nowrap",
                maxWidth: summaryMaxWidth ?? "100%",
                color:
                  selectedDrivers.length || value.noDnfPredicted
                    ? "text.primary"
                    : "text.secondary",
              }}
            >
              {text}
            </Box>
          );
        }}
        MenuProps={{ PaperProps: { sx: { maxHeight: 440 } } }}
      >
        <MenuItem value={NOBODY}>
          <Checkbox
            size="small"
            checked={value.noDnfPredicted}
            sx={{ mr: 1, p: 0.5 }}
          />
          Никто
        </MenuItem>
        {drivers.map((driver) => (
          <MenuItem
            key={driver}
            value={driver}
            disabled={selected.length >= 5 && !selected.includes(driver)}
          >
            <Checkbox
              size="small"
              checked={selected.includes(driver)}
              sx={{ mr: 1, p: 0.5 }}
            />
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
