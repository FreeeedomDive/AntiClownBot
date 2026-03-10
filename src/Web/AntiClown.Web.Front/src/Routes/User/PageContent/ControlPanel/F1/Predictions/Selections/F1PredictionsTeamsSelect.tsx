import { Box, Button, ButtonGroup } from "@mui/material";
import React from "react";
import { F1TeamDto } from "../../../../../../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsSelectCard from "./F1PredictionsSelectCard";
import F1TeamBadge from "../F1TeamBadge";

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
        <ButtonGroup key={team.name} size="large" fullWidth>
          <Button
            key={team.firstDriver}
            variant={
              selectedDriversFromTeams.has(team.firstDriver)
                ? "contained"
                : "outlined"
            }
            onClick={() => {
              selectedDriversFromTeams.delete(team.secondDriver);
              selectedDriversFromTeams.add(team.firstDriver);
              setSelectedDriversFromTeams(new Set(selectedDriversFromTeams));
            }}
          >
            <Box sx={{ display: "flex", alignItems: "center", gap: 0.75 }}>
              <F1TeamBadge driver={team.firstDriver} teams={teams} />
              {team.firstDriver}
            </Box>
          </Button>
          <Button
            key={team.secondDriver}
            variant={
              selectedDriversFromTeams.has(team.secondDriver)
                ? "contained"
                : "outlined"
            }
            onClick={() => {
              selectedDriversFromTeams.delete(team.firstDriver);
              selectedDriversFromTeams.add(team.secondDriver);

              setSelectedDriversFromTeams(new Set(selectedDriversFromTeams));
            }}
          >
            <Box sx={{ display: "flex", alignItems: "center", gap: 0.75 }}>
              <F1TeamBadge driver={team.secondDriver} teams={teams} />
              {team.secondDriver}
            </Box>
          </Button>
        </ButtonGroup>
      ))}
    </F1PredictionsSelectCard>
  );
}
