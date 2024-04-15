import {DRIVERS} from "../../../../Dto/F1Predictions/F1DriversHelpers";
import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import {Stack, Table, TableBody, TableContainer, TableHead, TableRow} from "@mui/material";
import React, {useState} from "react";

export default function F1RaceClassifications() {
  const [drivers, setDrivers] = useState(DRIVERS);
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
                }}
                onRemoveDnfDriver={() => {
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