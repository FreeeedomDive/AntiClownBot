import {
  Button,
  ButtonGroup,
} from "@mui/material";
import React from "react";
import {F1TeamDto} from "../../../../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsSelectCard from "./F1PredictionsSelectCard";

interface Props {
  selectedDriversFromTeams: Set<string>;
  setSelectedDriversFromTeams: (selectedDriversFromTeams: Set<string>) => void;
  teams: F1TeamDto[];
}

export default function F1PredictionsTeamsSelect({
  selectedDriversFromTeams,
  setSelectedDriversFromTeams,
  teams,
}: Props) {
  return (
    <F1PredictionsSelectCard title="Команды">
      {teams.map((team) => (
        <ButtonGroup size="large" fullWidth>
          <Button
            variant={
              selectedDriversFromTeams.has(team.firstDriver)
                ? "contained"
                : "outlined"
            }
            onClick={() => {
              selectedDriversFromTeams.delete(team.secondDriver);
              selectedDriversFromTeams.add(team.firstDriver);

              setSelectedDriversFromTeams(
                new Set(selectedDriversFromTeams),
              );
            }}
          >
            {team.firstDriver}
          </Button>
          <Button
            variant={
              selectedDriversFromTeams.has(team.secondDriver)
                ? "contained"
                : "outlined"
            }
            onClick={() => {
              selectedDriversFromTeams.delete(team.firstDriver);
              selectedDriversFromTeams.add(team.secondDriver);

              setSelectedDriversFromTeams(
                new Set(selectedDriversFromTeams),
              );
            }}
          >
            {team.secondDriver}
          </Button>
        </ButtonGroup>
      ))}
    </F1PredictionsSelectCard>
  );
}
