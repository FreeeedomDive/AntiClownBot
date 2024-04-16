import {DRIVERS} from "../../../../../Dto/F1Predictions/F1DriversHelpers";
import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import {Stack, Table, TableBody, TableContainer} from "@mui/material";
import React, {useState} from "react";
import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1RaceClassifications({f1Race}: Props) {
  const [drivers, setDrivers] = useState(
    !f1Race.result?.classification ? DRIVERS : f1Race.result.classification
  );
  const [dnfDrivers, setDnfDrivers] = useState(new Set(f1Race.result?.dnfDrivers ?? []));

  const swapDrivers = (firstDriverIndex: number, secondDriverIndex: number) => {
    const updatedDrivers = [...drivers];
    const firstDriver = updatedDrivers[firstDriverIndex];
    updatedDrivers[firstDriverIndex] = updatedDrivers[secondDriverIndex];
    updatedDrivers[secondDriverIndex] = firstDriver;
    setDrivers(updatedDrivers);
  }

  return (
    <Stack width={"30%"}>
      <TableContainer>
        <Table>
          <TableBody>
            {drivers.map((x, i) => (
              <F1RaceClassificationsElement
                f1Driver={x}
                index={i}
                onAddDnfDriver={() => {
                  dnfDrivers.add(x);
                  setDnfDrivers(new Set(dnfDrivers));
                }}
                onRemoveDnfDriver={() => {
                  dnfDrivers.delete(x);
                  setDnfDrivers(new Set(dnfDrivers));
                }}
                moveUp={() => {
                  swapDrivers(i, i - 1);
                }}
                moveDown={() => {
                  swapDrivers(i, i + 1);
                }}
              />
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Stack>
  )
}