import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import {Stack, Table, TableBody, TableContainer} from "@mui/material";
import {F1DriverDto} from "../../../../../Dto/F1Predictions/F1DriverDto";

interface Props {
  drivers: F1DriverDto[];
  setDrivers: (x: F1DriverDto[]) => void;
  dnfDrivers: Set<F1DriverDto>;
  setDnfDrivers: (x: Set<F1DriverDto>) => void;
}

export default function F1RaceClassifications({drivers, setDrivers, dnfDrivers, setDnfDrivers}: Props) {
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
                isDnf={dnfDrivers.has(x)}
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