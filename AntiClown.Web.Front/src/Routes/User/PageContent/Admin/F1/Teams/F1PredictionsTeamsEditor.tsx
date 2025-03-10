import React, { useEffect, useState } from "react";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import { RightsDto } from "../../../../../../Dto/Rights/RightsDto";
import { RightsWrapper } from "../../../../../../Components/RIghts/RightsWrapper";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { Table, TableBody, TableContainer } from "@mui/material";
import F1PredictionsTeamsEditorRow from "./F1PredictionsTeamsEditorRow";

export default function F1PredictionsTeamsEditor() {
  const [teams, setTeams] = useState<F1TeamDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function loadTeams() {
      const teams = await F1PredictionsApi.getActiveTeams();
      setTeams(teams);
      setIsLoading(false);
    }

    loadTeams();
  }, []);

  return (
    <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
      {isLoading ? (
        <Loader />
      ) : (
        <TableContainer>
          <Table>
            <TableBody>
              {teams.map((team) => (
                <F1PredictionsTeamsEditorRow team={team} />
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </RightsWrapper>
  );
}
