import {
  Avatar,
  Badge,
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { lighten } from "@mui/material/styles";
import { convertRaceNameToFlag } from "../../../../../../Helpers/RaceNameToFlagHelper";
import { F1PredictionsStandingsRow } from "./F1PredictionsStandingsRow";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { useParams } from "react-router-dom";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { F1StandingsDto } from "../../../../../../Dto/F1Predictions/F1StandingsDto";
import { MAIN_COLOR } from "../../../../../../Helpers/Colors";

const RACE_COL_WIDTH = 40;
const LEFT_COL_MIN_WIDTH = 280;
const RIGHT_COL_WIDTH = 80;
const STICKY_BG = lighten(MAIN_COLOR, 0.025);

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
    <Paper
      variant="outlined"
      sx={{
        borderRadius: 2,
        bgcolor: STICKY_BG,
        backgroundImage: "none",
        p: 2,
        minWidth: 0,
        overflow: "hidden",
      }}
    >
      <Box
        sx={{
          width: "100%",
          overflowX: "auto",
          scrollbarWidth: "thin",
          scrollbarColor: "rgba(255,255,255,0.2) transparent",
          "&::-webkit-scrollbar": { height: 5 },
          "&::-webkit-scrollbar-track": { backgroundColor: "transparent" },
          "&::-webkit-scrollbar-thumb": {
            backgroundColor: "rgba(255,255,255,0.2)",
            borderRadius: 3,
          },
        }}
      >
        <Table
          sx={{
            tableLayout: "fixed",
            width: "100%",
            minWidth:
              LEFT_COL_MIN_WIDTH +
              finishedRaces.length * RACE_COL_WIDTH +
              RIGHT_COL_WIDTH,
          }}
        >
          <TableHead>
            <TableRow>
              <TableCell
                sx={{
                  position: "sticky",
                  left: 0,
                  zIndex: 3,
                  bgcolor: STICKY_BG,
                  borderRight: "1px solid rgba(255,255,255,0.12)",
                  p: "8px 12px",
                }}
              >
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ pl: "34px" }}
                >
                  Участник
                </Typography>
              </TableCell>
              {finishedRaces.map((race) => {
                const flag = (
                  <Avatar
                    variant="rounded"
                    alt={race.name}
                    src={convertRaceNameToFlag(race.name)}
                    sx={{ width: 24, height: 24 }}
                  />
                );
                return (
                  <TableCell
                    key={race.id}
                    align="center"
                    sx={{ width: RACE_COL_WIDTH, p: "4px" }}
                  >
                    {race.isSprint ? (
                      <Badge variant="dot" color="info">
                        {flag}
                      </Badge>
                    ) : (
                      flag
                    )}
                  </TableCell>
                );
              })}
              <TableCell
                align="center"
                sx={{
                  position: "sticky",
                  right: 0,
                  zIndex: 3,
                  bgcolor: STICKY_BG,
                  borderLeft: "1px solid rgba(255,255,255,0.12)",
                  width: RIGHT_COL_WIDTH,
                  p: "8px 12px",
                }}
              >
                <Typography variant="body2" color="text.secondary">
                  Сумма
                </Typography>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {standings.standings.map((standingsRow, index) => (
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
            ))}
          </TableBody>
        </Table>
      </Box>
    </Paper>
  );
}
