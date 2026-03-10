import {F1TeamDto} from "./F1TeamDto";

export interface F1TeamInfo {
  trigram: string;
  color: string;
  textColor?: string;
}

const F1_TEAM_INFO: Record<string, F1TeamInfo> = {
  "McLaren": { trigram: "MCL", color: "#FF8000" },
  "Mercedes": { trigram: "MRC", color: "#00D2BE", textColor: "#000" },
  "Red Bull Racing": { trigram: "RBR", color: "#3671C6" },
  "Ferrari": { trigram: "FER", color: "#E8002D" },
  "Williams": { trigram: "WIL", color: "#005AFF" },
  "Racing Bulls": { trigram: "RCB", color: "#6692FF" },
  "Aston Martin": { trigram: "AMR", color: "#229971" },
  "Haas": { trigram: "HAS", color: "#B6BABD", textColor: "#000" },
  "Audi": { trigram: "AUD", color: "#BF0000" },
  "Alpine": { trigram: "ALP", color: "#FF87BC", textColor: "#000" },
  "Cadillac": { trigram: "CAD", color: "#0053A5" },
};

export function getDriversFromTeams(teams: F1TeamDto[]): string[] {
  return teams.flatMap(x => [x.firstDriver, x.secondDriver]);
}

export function getTeamInfo(teamName: string): F1TeamInfo | undefined {
  const normalized = teamName.replace(/^\d+\s+/, "");
  return F1_TEAM_INFO[normalized];
}

export function getTeamForDriver(driver: string, teams: F1TeamDto[]): F1TeamDto | undefined {
  return teams.find(t => t.firstDriver === driver || t.secondDriver === driver);
}
