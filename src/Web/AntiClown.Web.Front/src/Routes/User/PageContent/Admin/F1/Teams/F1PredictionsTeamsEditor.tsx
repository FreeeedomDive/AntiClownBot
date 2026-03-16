import React, { useEffect, useState } from "react";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import { RightsDto } from "../../../../../../Dto/Rights/RightsDto";
import { RightsWrapper } from "../../../../../../Components/Rights/RightsWrapper";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import F1PredictionsTeamsEditorRow from "./F1PredictionsTeamsEditorRow";

const HEADER_SX = {
  color: "text.secondary",
  fontWeight: 600,
  fontSize: "0.72rem",
  textTransform: "uppercase",
  letterSpacing: "0.06em",
  py: 1.5,
  px: 2,
} as const;

export default function F1PredictionsTeamsEditor() {
  const [teams, setTeams] = useState<F1TeamDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function loadTeams() {
      const teams = await F1PredictionsApi.getActiveTeams();
      setTeams(teams);
    }

    loadTeams()
      .catch(console.error)
      .finally(() => setIsLoading(false));
  }, []);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
      {isLoading ? (
        <Loader />
      ) : (
        <Paper variant="outlined" sx={{ borderRadius: 2, overflow: "hidden" }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell sx={HEADER_SX}>Команда</TableCell>
                  <TableCell sx={HEADER_SX}>Первый гонщик</TableCell>
                  <TableCell sx={HEADER_SX}>Второй гонщик</TableCell>
                  <TableCell sx={{ ...HEADER_SX, width: 56 }} />
                </TableRow>
              </TableHead>
              <TableBody>
                {teams.map((team) => (
                  <F1PredictionsTeamsEditorRow key={team.name} team={team} />
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      )}
    </RightsWrapper>
  );
}
