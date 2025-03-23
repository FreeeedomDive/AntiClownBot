import {
  Avatar,
  Badge,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { convertRaceNameToFlag } from "../../../../../../Helpers/RaceNameToFlagHelper";
import { F1PredictionsStandingsRow } from "./F1PredictionsStandingsRow";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { F1PredictionUserResultDto } from "../../../../../../Dto/F1Predictions/F1PredictionUserResultDto";
import { useParams } from "react-router-dom";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { F1PredictionsStandingsDto } from "../../../../../../Dto/F1Predictions/F1PredictionsStandingsDto";

interface Props {
  finishedRaces: F1RaceDto[];
  sortedStandings: F1PredictionUserResultDto[][];
  members: DiscordMemberDto[];
  standings: F1PredictionsStandingsDto;
}

export default function F1PredictionsStandingsTable({
  finishedRaces,
  sortedStandings,
  members,
  standings,
}: Props) {
  const { userId } = useParams<"userId">();

  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell align={"center"} sx={{ padding: "8px" }} />
            <TableCell align={"left"} sx={{ padding: "8px" }} />
            <TableCell align={"center"} sx={{ padding: "8px" }}>
              Очки за сезон
            </TableCell>
            {finishedRaces.map((race) => {
              const avatar = (
                <Avatar
                  variant={"rounded"}
                  alt={race.name}
                  src={convertRaceNameToFlag(race.name)}
                  sx={{ width: 24, height: 24 }}
                />
              );
              return (
                <TableCell sx={{ padding: "4px" }}>
                  {race.isSprint ? (
                    <Badge variant={"dot"} color="info">
                      {avatar}
                    </Badge>
                  ) : (
                    avatar
                  )}
                </TableCell>
              );
            })}
          </TableRow>
        </TableHead>
        <TableBody>
          {sortedStandings.map((results) => {
            const resultsUserId = results.find(
              (x) => x !== null && x?.userId !== null,
            )!.userId;
            return (
              <F1PredictionsStandingsRow
                key={resultsUserId}
                discordMember={members.find(
                  (member) => member.userId === resultsUserId,
                )}
                results={standings[resultsUserId]}
                isMe={resultsUserId === userId}
                races={finishedRaces}
              />
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
