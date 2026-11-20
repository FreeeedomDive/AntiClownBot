import React from "react";
import {
  Avatar,
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Typography,
} from "@mui/material";
import { F1ChampionshipParticipantPointsDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipDetailsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import DiscordMember from "../../../../../../Components/Users/DiscordMember";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { useParams } from "react-router-dom";

interface Props {
  allPoints: F1ChampionshipParticipantPointsDto[];
  members: DiscordMemberDto[];
}

export default function F1ChampionshipUserStandings({
  allPoints,
  members,
}: Props) {
  const { userId } = useParams<"userId">();
  return (
    <Stack direction="column" spacing={1}>
      <Typography variant="h6">Таблица очков</Typography>
      <TableContainer>
        <Table>
          <TableBody>
            {allPoints.map((entry, index) => {
              const member = members.find(
                (m) => m.userId === entry.participantId,
              );
              const isMe = entry.participantId === userId;
              return (
                <TableRow
                  key={entry.participantId}
                  sx={isMe ? { backgroundColor: "#200056" } : {}}
                >
                  <TableCell sx={{ padding: "1px", width: "10%" }}>
                    <IconButton size="small" sx={{ visibility: "hidden" }}>
                      <DragIndicatorIcon />
                    </IconButton>
                  </TableCell>
                  <TableCell sx={{ padding: "4px" }}>
                    <Typography>{index + 1}</Typography>
                  </TableCell>
                  <TableCell sx={{ padding: "4px" }}>
                    <Stack direction="row" spacing={1} alignItems="center">
                      <Avatar
                        src={member?.avatarUrl}
                        sx={{ width: 24, height: 24 }}
                      >
                        {entry.displayName.charAt(0).toUpperCase()}
                      </Avatar>
                      {member ? (
                        <DiscordMember member={member} />
                      ) : (
                        <Typography>{entry.displayName}</Typography>
                      )}
                    </Stack>
                  </TableCell>
                  <TableCell sx={{ padding: "4px", textAlign: "right" }}>
                    <Typography>{entry.totalPoints}</Typography>
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>
    </Stack>
  );
}
