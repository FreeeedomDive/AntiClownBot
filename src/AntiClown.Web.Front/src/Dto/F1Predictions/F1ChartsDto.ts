import { F1UserChartDto } from "./F1UserChartDto";

export interface F1ChartsDto {
  usersCharts: F1UserChartDto[];
  championChart: F1UserChartDto;
}
