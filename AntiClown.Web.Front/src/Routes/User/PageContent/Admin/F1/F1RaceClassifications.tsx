import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import {Stack, Table, TableBody, TableContainer} from "@mui/material";

interface Props {
  drivers: string[];
  setDrivers: (x: string[]) => void;
  dnfDrivers: Set<string>;
  setDnfDrivers: (x: Set<string>) => void;
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