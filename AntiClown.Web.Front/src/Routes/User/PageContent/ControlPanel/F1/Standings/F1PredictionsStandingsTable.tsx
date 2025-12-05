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
import { useParams } from "react-router-dom";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { F1StandingsDto } from "../../../../../../Dto/F1Predictions/F1StandingsDto";

interface Props {
  finishedRaces: F1RaceDto[];
  members: DiscordMemberDto[];
  standings: F1StandingsDto;
}

export default function F1PredictionsStandingsTable({
  finishedRaces,
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
          {standings.standings.map((standingsRow, index) => {
            return (
              <F1PredictionsStandingsRow
                key={standingsRow.userId}
                discordMember={members.find(
                  (member) => member.userId === standingsRow.userId,
                )}
                results={standingsRow}
                isMe={standingsRow.userId === userId}
                races={finishedRaces}
                position={index + 1}
                pointsLeft={standings.pointsLeft}
                leaderPoints={standings.currentLeaderPoints}
              />
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
