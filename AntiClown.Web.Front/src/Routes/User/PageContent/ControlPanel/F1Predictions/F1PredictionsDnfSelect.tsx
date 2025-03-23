import {
  Card,
  CardContent,
  Checkbox,
  FormControl,
  FormControlLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  Stack,
  Typography,
} from "@mui/material";
import React from "react";
import { getDriversFromTeams } from "../../../../../Dto/F1Predictions/F1DriversHelpers";
import { F1TeamDto } from "../../../../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsSelectCard from "./F1PredictionsSelectCard";

interface Props {
  isDNFNobody: boolean;
  setIsDNFNobody: (isDNFNobody: boolean) => void;
  dnfList: DNFList;
  setDnfList: (dnfList: DNFList) => void;
  teams: F1TeamDto[];
}

export type DNFList = [string, string, string, string, string];

export default function F1PredictionsDnfSelect({
  isDNFNobody,
  setIsDNFNobody,
  dnfList,
  setDnfList,
  teams,
}: Props) {
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

  return (
    <F1PredictionsSelectCard title="DNF">
      <FormControlLabel
        control={
          <Checkbox
            checked={isDNFNobody}
            onChange={() => {
              setIsDNFNobody(!isDNFNobody);
            }}
          />
        }
        label="Никто"
        sx={{ width: "100%" }}
      />
      {!isDNFNobody && (
        <>
          {dnfList.map((dnfItem, index) => {
            return (
              <FormControl fullWidth>
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
              </FormControl>
            );
          })}
        </>
      )}
    </F1PredictionsSelectCard>
  );
}
