import {
  Box,
  Checkbox,
  FormControl,
  FormControlLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
} from "@mui/material";
import React from "react";
import { getDriversFromTeams } from "../../../../../../../Dto/F1Predictions/F1DriversHelpers";
import { F1TeamDto } from "../../../../../../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsSelectCard from "./F1PredictionsSelectCard";
import F1TeamBadge from "../F1TeamBadge";

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
              <FormControl key={index} fullWidth size="small">
                <Select
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
                        <Box
                          sx={{ display: "flex", alignItems: "center", gap: 1 }}
                        >
                          <F1TeamBadge driver={driver} teams={teams} />
                          {driver}
                        </Box>
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
