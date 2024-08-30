import {F1DriverDto} from "./F1DriverDto";

export const DRIVER_PAIRS = [
  [F1DriverDto.Verstappen, F1DriverDto.Perez],
  [F1DriverDto.Leclerc, F1DriverDto.Sainz],
  [F1DriverDto.Hamilton, F1DriverDto.Russell],
  [F1DriverDto.Ocon, F1DriverDto.Gasly],
  [F1DriverDto.Piastri, F1DriverDto.Norris],
  [F1DriverDto.Bottas, F1DriverDto.Zhou],
  [F1DriverDto.Stroll, F1DriverDto.Alonso],
  [F1DriverDto.Magnussen, F1DriverDto.Hulkenberg],
  [F1DriverDto.Ricciardo, F1DriverDto.Tsunoda],
  [F1DriverDto.Albon, F1DriverDto.Colapinto],
] as const;

export const DRIVERS = DRIVER_PAIRS.flatMap((pair) => pair);