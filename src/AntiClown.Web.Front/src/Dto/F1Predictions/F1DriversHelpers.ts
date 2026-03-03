import {F1TeamDto} from "./F1TeamDto";

export function getDriversFromTeams(teams: F1TeamDto[]): string[] {
  return teams.flatMap(x => [x.firstDriver, x.secondDriver]);
}