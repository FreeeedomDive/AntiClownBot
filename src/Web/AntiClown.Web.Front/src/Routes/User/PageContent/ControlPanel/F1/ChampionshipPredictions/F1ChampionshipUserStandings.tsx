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
import { F1ChampionshipUserPointsDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipUserPointsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import DiscordMember from "../../../../../../Components/Users/DiscordMember";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { useParams } from "react-router-dom";

interface Props {
  allPoints: F1ChampionshipUserPointsDto[];
  members: DiscordMemberDto[];
}

export default function F1ChampionshipUserStandings({
  allPoints,
  members,
}: Props) {
  const { userId } = useParams<"userId">();
  const sorted = [...allPoints]
    .map((p) => ({
      ...p,
      total:
        p.preSeasonPoints.reduce((s, x) => s + x, 0) +
        p.midSeasonPoints.reduce((s, x) => s + x, 0),
    }))
    .sort((a, b) => b.total - a.total);

  return (
    <Stack direction="column" spacing={1}>
      <Typography variant="h6">Таблица очков</Typography>
      <TableContainer>
        <Table>
          <TableBody>
            {sorted.map((entry, index) => {
              const member = members.find((m) => m.userId === entry.userId);
              const isMe = entry.userId === userId;
              return (
                <TableRow
                  key={entry.userId}
                  sx={isMe ? { backgroundColor: "#120030" } : {}}
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
                    {member && (
                      <Stack direction="row" spacing={1} alignItems="center">
                        <Avatar
                          alt={member.serverName ?? member.userName}
                          src={member.avatarUrl}
                          sx={{ width: 24, height: 24 }}
                        />
                        <DiscordMember member={member} />
                      </Stack>
                    )}
                  </TableCell>
                  <TableCell sx={{ padding: "4px", textAlign: "right" }}>
                    <Typography>{entry.total}</Typography>
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
